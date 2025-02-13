using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Common
{
    public class AdressablesLoader : MonoBehaviour
    {
        public static async Task<Sprite> LoadSpriteAsync(string name)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>("cut/" + name + ".png");
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var sprite = handle.Result;

                Addressables.Release(handle);
                return sprite;
            }
            else
            {
                Debug.LogError("Failed to load Addressable: " + handle.DebugName);
                return null;
            }
        }
    }
}
