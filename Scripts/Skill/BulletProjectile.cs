using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    float speed, maxRange, traveled;
    float damageParam;
    ICombatEntity shooter;

    [Header("VFX & 연출")]
    public GameObject muzzlePrefab;        // 발사 이펙트
    public GameObject hitPrefab;           // 명중 이펙트
    public List<GameObject> trails;        // 트레일(파티클 등)
    public bool rotate = false;            // 회전 연출 여부
    public float rotateSpeed = 360f;       // 회전 속도 (deg/sec)

    private Rigidbody rb;
    private bool destroyed = false;

    public void Init(ICombatEntity shooter, Vector3 dir, float speed, float maxRange, float dmgParam)
    {
        this.shooter = shooter;
        this.speed = speed;
        this.maxRange = maxRange;
        this.damageParam = dmgParam;

        transform.forward = dir.normalized;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = dir.normalized * speed;
        traveled = 0f;

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = dir.normalized;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else if (muzzleVFX.transform.childCount > 0)
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    void Update()
    {
        //if (rotate)
        //    transform.Rotate(0, 0, rotateSpeed * Time.deltaTime, Space.Self);

        traveled += speed * Time.deltaTime;
        if (traveled > maxRange && !destroyed)
            StartCoroutine(DestroyProjectile());
    }

    private void OnTriggerEnter(Collider col)
    {
        if (destroyed) return; // 중복 파괴 방지

        var target = col.GetComponent<ICombatEntity>();
        LayerMask envLayers = LayerMask.GetMask("Ground", "Wall", "Obstacle");
        if (target != null && target != shooter && target.IsAlive)
        {
            float dmg = shooter.GetAttackPower() * damageParam;
            target.TakeDamage(dmg, transform.forward);

            // --- 명중 이펙트 생성 ---
            SpawnHitEffect(col.ClosestPoint(transform.position), col.transform);
            StartCoroutine(DestroyProjectile());
        }
        // 환경 오브젝트에 닿았을 때도 명중 이펙트(필요하다면)
        else if (((1 << col.gameObject.layer) & envLayers) != 0)
        {
            SpawnHitEffect(col.ClosestPoint(transform.position), col.transform);
            StartCoroutine(DestroyProjectile());
        }
    }

    void SpawnHitEffect(Vector3 pos, Transform normalTr)
    {
        if (hitPrefab != null)
        {
            var rot = Quaternion.LookRotation(normalTr.up);
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null && hitVFX.transform.childCount > 0)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else if (ps != null)
            {
                Destroy(hitVFX, ps.main.duration);
            }
        }
    }

    IEnumerator DestroyProjectile()
    {
        destroyed = true; // 중복 파괴 방지

        // 트레일이 있다면 분리 후 천천히 소멸
        if (trails != null && trails.Count > 0)
        {
            foreach (var trail in trails)
            {
                if (trail == null) continue;
                trail.transform.parent = null;
                var ps = trail.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }

        // 본체(탄환)는 바로 비활성화/파괴
        yield return null; // 바로 파괴해도 되지만, 잔상 남기려면 WaitForSeconds(0.1f) 등 추가 가능
        Destroy(gameObject);
    }

}
