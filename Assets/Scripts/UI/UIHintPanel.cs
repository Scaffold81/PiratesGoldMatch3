using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using RxExtensions;
using System;

namespace Game.UI
{
    public class UIHintPanel : UIPanelBase
    {
        private DoubloonsProcessor _doubloonsProcessor;
        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            _doubloonsProcessor = new DoubloonsProcessor(_sceneDataProvider);
            Subscribe();
        }

        private void Subscribe()
        {
            _sceneDataProvider.Receive<bool>(EventNames.GetHintForDoubloons).Subscribe(newValue =>
            {
                GetHintForDoubloons();

            }).AddTo(_disposables);

            _sceneDataProvider.Receive<bool>(EventNames.GetHintForAdv).Subscribe(newValue =>
            {
                GetHintForAward();

            }).AddTo(_disposables);
        }

        private void GetHintForAward()
        {
            var hitMark = (float)_sceneDataProvider.GetValue(Player—urrency.HintMark);
            hitMark += 1;
            _sceneDataProvider.Publish(Player—urrency.HintMark, hitMark);
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.HintPanel);
        }

        private void GetHintForDoubloons()
        {
            var hitMark = (float)_sceneDataProvider.GetValue(Player—urrency.HintMark);
            var doubloons = (float)_sceneDataProvider.GetValue(Player—urrency.Doubloons);
            var defaultValues = (DefaultValuesSO)_sceneDataProvider.GetValue(EventNames.DefaultValues);

            if (_doubloonsProcessor.ProcessForDoubloons(defaultValues.hintMarkCostDoubloons))
            {
                hitMark += 1;
                _sceneDataProvider.Publish(Player—urrency.HintMark, hitMark);
            }
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.HintPanel);
        }
    }
}
