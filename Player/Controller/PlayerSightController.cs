using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerController))]
public class PlayerSightController : MonoBehaviour
{
    public static PlayerSightController Instance { get; private set; }

    [SerializeField] private float maxBrightness = 100f;
    public float MaxBrightness => maxBrightness;
    [SerializeField] private float brightness = 100f;
    public float Brightness => brightness;
    [SerializeField] private float brightnessDecreaseRate = 0.2f;
    [SerializeField] private float minSightRange = 1f;
    [SerializeField] private float maxSightRange = 8f;

    [SerializeField] private int circleSegments = 180;
    [SerializeField] private MeshFilter sightMeshFilter;
    [SerializeField] private GameObject sightMask;

    [SerializeField] private float zOffsetMin = 0.5f;
    [SerializeField] private float zOffsetMax = 1.5f;

    public static bool HasInstance => Instance != null;

    private List<LightSource> lightSources = new List<LightSource>();

    public LayerMask blockLayer;

    Camera mainCam;

    public LayerMask hideLayers;
    [SerializeField] private float checkRange = 30f;
    private List<Renderer> _cachedRenderers = new List<Renderer>();

    public float SightRange => Mathf.Lerp(minSightRange, maxSightRange, brightness / maxBrightness);

    public bool IsSightOn { get; private set; } = true;

    private float CurrentZOffset =>
    IsSightOn
        ? Mathf.Lerp(zOffsetMin, zOffsetMax, brightness / maxBrightness)
        : zOffsetMin;

    public float CurrentSightRange =>
    IsSightOn
        ? Mathf.Lerp(minSightRange, maxSightRange, brightness / maxBrightness)
        : minSightRange;


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //bool isDungeon = SceneManager.GetActiveScene().name.Contains("DungeonScene");
        //sightMask.SetActive(isDungeon);

        //if (!isDungeon)
        //{
        //    return;
        //}

        if (!SceneManager.GetActiveScene().name.Contains("Dungeon"))
        {
            enabled = false;
            if (sightMask != null) sightMask.SetActive(false);
            return;
        }

        mainCam = Camera.main;
        var mr = sightMeshFilter.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "Default";
            mr.sortingOrder = 1000;
        }
    }

    void Update()
    {
        if (IsSightOn)
        {
            brightness -= brightnessDecreaseRate * Time.deltaTime;
            brightness = Mathf.Clamp(brightness, 0f, maxBrightness);
        }

        if (!sightMask.activeSelf)
            return;

        if (sightMask != null && sightMask.activeSelf)
            UpdateSightPolygon();

        UpdateVisibility();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ToggleSight()
    {
        IsSightOn = !IsSightOn;
    }


    public void AddBrightness(float amount)
    {
        brightness = Mathf.Clamp(brightness + amount, 0f, maxBrightness);
    }

    public void SetBrightness(float value)
    {
        brightness = Mathf.Clamp(value, 0f, maxBrightness);
    }

    public void RegisterLightSource(LightSource src)
    {
        if (!lightSources.Contains(src))
            lightSources.Add(src);
    }
    public void UnregisterLightSource(LightSource src)
    {
        lightSources.Remove(src);
    }

    // 외부에서 강제로 시야 갱신 필요할 때 호출
    public void UpdateSight()
    {
        UpdateSightPolygon();
    }

    private void UpdateSightPolygon()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vOffset = 0;

        // 1. 플레이어 시야(원)
        float radius = CurrentSightRange;
        float zOffset = CurrentZOffset;

        vertices.Add(new Vector3(0, 0, zOffset));

        int segmentCount = circleSegments;
        float step = 360f / segmentCount;

        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = Mathf.Deg2Rad * (i * step);
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            // Raycast를 씁니다!
            Ray ray = new Ray(transform.position + Vector3.up * 1.5f, dir);
            float hitDist = radius;
            if (Physics.Raycast(ray, out RaycastHit hit, radius, blockLayer))
            {
                hitDist = hit.distance;
            }
            // 플레이어 local로 변환(기존대로 유지)
            Vector3 point = dir * hitDist + new Vector3(0, 0, zOffset);
            vertices.Add(point);
        }
        for (int i = 1; i <= circleSegments; i++)
        {
            triangles.Add(vOffset);
            triangles.Add(vOffset + i);
            triangles.Add(vOffset + i + 1);
        }
        vOffset += (circleSegments + 2);

        // 2. LightSource 시야(원)
        foreach (var src in lightSources)
        {
            if (src == null || src.state != LightState.On)
                continue;

            // 월드 → 플레이어 기준 local 변환
            Vector3 localPos = transform.InverseTransformPoint(src.transform.position);
            vertices.Add(localPos);

            for (int i = 0; i <= src.segments; i++)
            {
                float angle = i * Mathf.PI * 2f / src.segments;
                Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                vertices.Add(localPos + dir * src.range);
            }
            for (int i = 1; i <= src.segments; i++)
            {
                triangles.Add(vOffset);
                triangles.Add(vOffset + i);
                triangles.Add(vOffset + i + 1);
            }
            vOffset += (src.segments + 2);
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        sightMeshFilter.mesh = mesh;
    }

    void UpdateVisibility()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, checkRange, hideLayers);
        HashSet<Renderer> seen = new HashSet<Renderer>();

        float playerRange = CurrentSightRange;

        foreach (var col in targets)
        {
            Vector3 objPos = col.transform.position;
            float distToPlayer = Vector3.Distance(objPos, transform.position);

            bool visible = (distToPlayer <= playerRange);

            // 🔹 광원 판정 추가
            if (!visible)
            {
                foreach (var src in lightSources)
                {
                    if (src == null || src.state != LightState.On)
                        continue;

                    float distToLight = Vector3.Distance(objPos, src.transform.position);
                    if (distToLight <= src.range)
                    {
                        visible = true;
                        break;
                    }
                }
            }

            foreach (var rend in col.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = visible;
                if (visible) seen.Add(rend);
            }
        }

        // 기존 켜져있던 렌더러 중 이번에 안 보이는 건 끔
        foreach (var rend in _cachedRenderers)
        {
            if (rend != null && !seen.Contains(rend))
                rend.enabled = false;
        }
        _cachedRenderers.Clear();
        _cachedRenderers.AddRange(seen);
    }
}
