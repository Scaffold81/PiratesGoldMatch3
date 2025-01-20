using Core.Data;
using Game.Enums;
using Game.Structures;
using UnityEngine;

namespace Game.UI
{
    public class UISceneTransitionButton : UICustomButtonBase
    {
        private SceneDataProvider _sceneDataProvider;
        private EventNames _eventNames;
        [SerializeField]
        private int _sceneIndex;
        
        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
        }

        protected override void OnClick()
        {
            _sceneDataProvider.Publish(SelectedEnumValue, _sceneIndex);
        }
    }
}