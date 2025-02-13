using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using System.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Reactive.Disposables;
using RxExtensions;

namespace Game.UI
{
    public class UILevelButton : UICustomButtonBase
    {
        [SerializeField]
        private LevelConfigSO _levelConfig;
        [SerializeField]
        private Image _lockImage;
        [SerializeField]
        private TMP_Text _levelText;

        public LevelConfigSO LevelConfig { get => _levelConfig; private set => _levelConfig = value; }

        private void Start()
        {
            Init();
            Subscribe();
        }

        protected override void Subscribe()
        {
            _sceneDataProvider.Receive<LevelConfigRepositorySO>(SaveSlotNames.LevelsConfig).Subscribe(newValue =>
            {
                GetLevelConfig(newValue);

            }).AddTo(_disposables);
        }

        protected override void OnClick()
        {
            if (!LevelConfig.isLevelOpen) return;

            LevelConfig.currentSublevelIndex = 0;
            _sceneDataProvider.Publish(EventNames.SetLevel, LevelConfig);
            _sceneDataProvider.Publish(SaveSlotNames.PreviosLevelConfig, LevelConfig);//сохраням уровень как предидущий
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, LevelConfig);
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.PlayLevelPanel);
        }

        private void GetLevelConfig(LevelConfigRepositorySO levelsConfig)
        {
            var config = levelsConfig.levelConfigs.FirstOrDefault(c => c.levelId == LevelConfig.levelId);

            if (config != null)
            {
                LevelConfig = ScriptableObject.CreateInstance<LevelConfigSO>();
                LevelConfig = config;

                UpdateButton(config);
            }
            else
            {
                Debug.LogWarning("Level config not found for level ID: " + LevelConfig.levelId);
            }
        }

        private void UpdateButton(LevelConfigSO levelConfig)
        {
            _levelText.text = levelConfig.levelId.ToString();
            _lockImage.enabled = !levelConfig.isLevelOpen;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}