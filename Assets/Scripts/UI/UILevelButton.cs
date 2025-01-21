﻿using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UILevelButton : UICustomButtonBase
    {
        [SerializeField]
        private LevelConfigSO _levelConfig;
        private SceneDataProvider _sceneDataProvider;
       
        [SerializeField]
        private Image _lockImage;

        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            GetLevelConfig();
        }

        protected override void OnClick()
        {
            _sceneDataProvider.Publish(SelectedEnumValue, _levelConfig.config);
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, _levelConfig.config);
        }

        private void GetLevelConfig()
        {
            var configs = (List<LevelConfigSO>)_sceneDataProvider.GetValue(SaveSlotNames.LevelsData) ?? new List<LevelConfigSO>();
            if (configs.Count <= 0) return;
            var config=configs.FirstOrDefault(c=>c.config.levelId==_levelConfig.config.levelId);
            _levelConfig = config;
        }
        
        private void UpdateButton()
        {
            if(_levelConfig.config.isLevelPassed)
                _lockImage.enabled=false; 
            else 
                _lockImage.enabled=true;
        }
    }
}