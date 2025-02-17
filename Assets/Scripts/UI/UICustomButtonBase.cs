using UnityEngine;
using UnityEngine.UI;
namespace Game.UI
{
    [RequireComponent(typeof(Button))]
    public class UICustomButtonBase : EnumProvider
    {
        protected private Button _btn;

        private void Awake()
        {
            _btn = GetComponent<Button>();

            if (_btn != null)
                _btn.onClick.AddListener(OnClick);
        }

        protected virtual void OnClick() { }
    }
}