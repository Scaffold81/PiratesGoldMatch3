using UnityEngine;
using UnityEngine.UI;
namespace Game.UI
{
    [RequireComponent(typeof(Button))]
    public class UIUICustomButtonBase : MonoBehaviour
    {
        private Button _btn;

        private void Awake()
        {
            _btn = GetComponent<Button>();

            if (_btn != null)
                _btn.onClick.AddListener(OnClick);
            else
                Debug.LogError("Button not found. Check component Button in game object " + name);
        }

        protected virtual void OnClick() { }
    }
}