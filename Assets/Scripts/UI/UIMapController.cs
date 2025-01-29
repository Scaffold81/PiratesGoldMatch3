using Core.Data;
using DG.Tweening;
using Game.Enums;
using Game.ScriptableObjects;
using RxExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.UI
{
    public class UIMap : GetDataProvider
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _ship;
        private List<UILevelButton> _levelButtons;
        private LevelConfigSO _targetLevel;


        private Vector2 _levelButtonPositionOffset=new Vector2(0,200);

        private void Awake()
        {
            FindLevelButtons();
        }
        private void Start()
        {
            Init();
            MoveLevelPosition(_targetLevel);
        }
        private void FindLevelButtons()
        {
            _levelButtons = new List<UILevelButton>(FindObjectsOfType<UILevelButton>());
        }

        protected override void Subscribe()
        {
            _sceneDataProvider.Receive<LevelConfigSO>(EventNames.SetLevel).Subscribe(newValue =>
            {
                MoveLevelPosition(newValue);

            }).AddTo(_disposables);

            _sceneDataProvider.Receive<LevelConfigSO>(SaveSlotNames.LevelConfig).Subscribe(newValue =>
            {
                _targetLevel = newValue;

            }).AddTo(_disposables);
        }

        private void GetScrollPosition(LevelConfigSO newValue)
        {
            var target = _levelButtons.FirstOrDefault(a => a.LevelConfig.levelId == newValue.levelId);
            var defauleValues = (DefaultValuesSO)_sceneDataProvider.GetValue(EventNames.DefaultValues);
            var prevLevelConfig = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.PreviosLevelConfig) ?? defauleValues.levelConfig;
            var previousTarget = _levelButtons.FirstOrDefault(a => a.LevelConfig.levelId == prevLevelConfig.levelId);

            SetScrollPosition(target.GetComponent<RectTransform>().anchoredPosition, previousTarget.GetComponent<RectTransform>().anchoredPosition);
        }

        // Установить нужные координаты прокрутки с использованием DotTween
        public void SetScrollPosition(Vector2 targetPosition, Vector2 previousTarget)
        {
            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, -previousTarget.y);
            DOTween.To(() => _content.anchoredPosition, x => _content.anchoredPosition = x, new Vector2(_content.anchoredPosition.x, -targetPosition.y), 0.5f);
        }

        public void MoveLevelPosition(LevelConfigSO newValue)
        {
            var targetRectTransform = _levelButtons.FirstOrDefault(a => a.LevelConfig.levelId == newValue.levelId).GetComponent<RectTransform>();
            var targetPosition = targetRectTransform.anchoredPosition;

            DOTween.To(() => _ship.anchoredPosition,
                pos => _ship.anchoredPosition = pos,
                targetPosition+_levelButtonPositionOffset,
                0.5f)
                .OnComplete(() =>
                {
                    _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.PlayLevelPanel);
                    Debug.Log("Animation completed!");
                });
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}
