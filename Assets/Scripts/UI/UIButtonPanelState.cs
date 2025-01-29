using Core.Data;
using Game.Enums;
using UnityEngine;

namespace Game.UI
{
    public class UIButtonPanelState : UICustomButtonBase
    {
        [SerializeField]
        private EventNames _sendName;

        private void Start()
        {
            Init();
        }

        protected override void OnClick()
        {
            _sceneDataProvider.Publish(SelectedEnumValue, _sendName);
        }
    }
}