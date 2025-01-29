using UnityEngine;

namespace Game.UI
{
    public class UISceneTransitionButton : UICustomButtonBase
    {
      
        [SerializeField]
        private int _sceneIndex;

        private void Start()
        {
           Init();
        }

        protected override void OnClick()
        {
            _sceneDataProvider.Publish(SelectedEnumValue, _sceneIndex);
        }
    }
}