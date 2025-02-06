using UnityEngine;
using UnityEngine.Events;

namespace RxExtensions
{
    [DisallowMultipleComponent]
    public class DestroyTrigger : MonoBehaviour
    {
        public UnityEvent Destroyed = new();

        private void OnDestroy()
        {
            Destroyed.Invoke();
        }
    }
}
