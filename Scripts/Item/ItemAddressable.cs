using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class ItemAddressable
{
    public static async Task<GameObject> LoadPrefabAsync(string address)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;

        return null;
    }
}
