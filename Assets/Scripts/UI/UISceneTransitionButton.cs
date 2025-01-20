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
    public class UILevelButton : UICustomButtonBase
    {
        private LevelConfigSO _levelConfig;
        private LevelObjectsSO _levelObjects;
        private SceneDataProvider _sceneDataProvider;
       
        [SerializeField]
        private Image _lockImage;

        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            GetLevelConfig();
            GetLevelObjects();
        }

        protected override void OnClick()
        {
            _sceneDataProvider.Publish(SelectedEnumValue, _levelConfig);
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, _levelConfig);
        }

        private void GetLevelConfig()
        {
            var configs = (List<LevelConfigSO>)_sceneDataProvider.GetValue(SaveSlotNames.LevelsData) ?? new List<LevelConfigSO>();
            if (configs.Count <= 0) return;
            var config=configs.FirstOrDefault(c=>c.levelId==_levelConfig.levelId);
            _levelConfig = config;
        }

        private void GetLevelObjects()
        {
            var objects = (List<LevelObjectsSO>)_sceneDataProvider.GetValue(SaveSlotNames.LevelsData) ?? new List<LevelObjectsSO>();
            if (objects.Count <= 0) return;
            var gObject = objects.FirstOrDefault(c => c.levelId == _levelConfig.levelId);
            _levelObjects = gObject;
        }

        private void UpdateButton()
        {
            if(_levelConfig.isLevelPassed)
                _lockImage.enabled=false; 
            else 
                _lockImage.enabled=true;
        }
    }
}