using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class SceneAssetData
{
    public string sceneName;
    public List<string> assetAddresses;
}

public class LoadingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI tipText;

    [Header("씬별 에셋 주소 매핑")]
    [SerializeField] private SceneAssetData[] sceneAssetDataList;

    [Header("팁")]
    [SerializeField] private List<string> tipList;


    //public GameObject LodingPanel;
    private Dictionary<string, List<string>> sceneAssetMap = new();
    private List<AsyncOperationHandle> assetHandles = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // Inspector용 Scriptable 방식에서 Dictionary로 변환
        foreach (var data in sceneAssetDataList)
        {
            if (!sceneAssetMap.ContainsKey(data.sceneName))
            {
                sceneAssetMap[data.sceneName] = new List<string>(data.assetAddresses);
            }
        }

        if(loadingPanel==null)
        {
            Instantiate(loadingPanel);
        }

    }

    public void StartLoad(string sceneName)
    {
        loadingPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();

        Debug.Log($"[StartLoad] sceneName = {sceneName}");
        Debug.Log($"loadingPanel: {loadingPanel}, progressBar: {progressBar}, progressText: {progressText}, loadingText: {loadingText}");

        StartCoroutine(DelayLoad(sceneName));
    }
    private IEnumerator DelayLoad(string sceneName)
    {
        yield return null;
        yield return LoadAddressableAsset(sceneName);
    }

    private IEnumerator LoadAddressableAsset(string sceneName)
    {

        progressBar.fillAmount = 0f;
        progressText.text = "0%";
        tipText.text = tipList[Random.Range(0, tipList.Count)];


        // 해당 씬에 필요한 에셋 로딩
        if (sceneAssetMap.TryGetValue(sceneName, out List<string> assetAddresses) && assetAddresses.Count > 0)
        {
            AsyncOperationHandle<IList<GameObject>> assetHandle =
                Addressables.LoadAssetsAsync<GameObject>(
                    assetAddresses,
                    obj => { },
                    Addressables.MergeMode.Union
                );

            assetHandles.Add(assetHandle);
            while (!assetHandle.IsDone)
            {
                float progress = assetHandle.PercentComplete * 0.5f; 
                progressBar.fillAmount = progress;
                progressText.text = $"{Mathf.RoundToInt(progress * 200)}%";
                yield return null;
            }

            if (assetHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"에셋 {assetHandle.Result.Count}개 로딩 완료");
            }
            else
            {
                Debug.LogError($"에셋 로딩 실패: {sceneName}");
            }
        }

        // 씬 비동기 로딩
        AsyncOperationHandle<SceneInstance> sceneHandle =
            Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        assetHandles.Add(sceneHandle);

        while (!sceneHandle.IsDone)
        {
            float progress = 0.5f + sceneHandle.PercentComplete * 0.5f;
            progressBar.fillAmount = progress;
            progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        loadingPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var handle in assetHandles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
    }
}
