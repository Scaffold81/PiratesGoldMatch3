using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UISceneTransitionButton : UICustomButtonBase
    {
        private SceneDataProvider _sceneDataProvider;
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