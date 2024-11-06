using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Common
{
    public class LoadAddressables
    {
        public static async Task<Sprite> GetSpriteForNameAsync(string spriteName)
        {
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(spriteName);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Sprite sprite = handle.Result;
                Addressables.Release(handle);
                return sprite;
            }
            else
            {
                Debug.LogError("Failed to load Addressable: " + handle.DebugName);
                Addressables.Release(handle);
                return null;
            }
        }
    }
}
