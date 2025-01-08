using Core.Data;
using Game.Enums;
using UnityEngine;

namespace Game.UI
{
    public class UIButtonPanelState : UIUICustomButtonBase
    {
        [SerializeField]
        private EventNames _targetName = EventNames.UIPanelStateChange;
        [SerializeField]
        private EventNames _sendName;

        private SceneDataProvider _sceneDataProvider;

        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
        }

        protected override void OnClick()
        {
            _sceneDataProvider.Publish(_targetName, _sendName);
        }
    }
}