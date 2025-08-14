using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class JobSORepository : MonoBehaviour
{
    [SerializeField] private List<PlayerJobSO> _allJobs;
    private Dictionary<string, PlayerJobSO> _jobMap;

    private async void Awake()
    {
        // Addressables에서 모든 에셋 로드
        _jobMap = new Dictionary<string, PlayerJobSO>();
        //foreach (PlayerJobSO so in _allJobs)
        //{
        //    PlayerJobSO loaded = await so.JobAsset.LoadAssetAsync<PlayerJobSO>().Task;
        //    _jobMap[loaded.JobId] = loaded;
        //}
        foreach (var so in _allJobs)
            _jobMap[so.JobId] = so;
    }

    public PlayerJobSO Get(string jobId)
    {
        if (!_jobMap.TryGetValue(jobId, out PlayerJobSO so))
            throw new KeyNotFoundException($"Unknown JobId: {jobId}");
        return so;
    }
}