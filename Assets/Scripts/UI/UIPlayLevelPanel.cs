using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using TMPro;
using System;
using RxExtensions;
using UnityEngine;

namespace Game.UI
{
    public class UIPlayLevelPanel : GetDataProvider
    {
        [SerializeField]
        private TMP_Text _header;

        private void Start()
        {
            Init();
        }

        protected override void Subscribe()
        {
            _sceneDataProvider.Receive<LevelConfigSO>(SaveSlotNames.LevelConfig).Subscribe(newValue =>
            {
                UpdatePanel(newValue);

            }).AddTo(_disposables);

            base.Subscribe();
        }

        private void UpdatePanel(LevelConfigSO newValue)
        {
            _header.text = "Level " + newValue.levelId.ToString();
        }
    }
}
