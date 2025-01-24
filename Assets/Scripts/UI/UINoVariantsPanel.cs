using System;
using Core.Data;
using Game.Enums;
using RxExtensions;
using Game.ScriptableObjects;

namespace Game.UI
{
    public class UINoVariantsPanel : UIPanelBase
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
            Subscribes();
        }

        private void Subscribes()
        {
            _sceneDataProvider.Receive<bool>(EventNames.RefreshForAdv).Subscribe(newValue =>
            {
                RefreshForAdv();
            }).AddTo(_disposables);

            _sceneDataProvider.Receive<bool>(EventNames.RefreshForDoubloons).Subscribe(newValue =>
            {
                RefreshForDoubloons();
            }).AddTo(_disposables);

            _sceneDataProvider.Receive<bool>(EventNames.AdmitDefeat).Subscribe(newValue =>
            {
                AdmitDefeat();
            }).AddTo(_disposables);
        }

        private void AdmitDefeat()
        {
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.LosePanel);
        }

        private void RefreshForDoubloons()
        {
            var defaultValues = (DefaultValuesSO)_sceneDataProvider.GetValue(EventNames.DefaultValues);

            if (_doubloonsProcessor.ProcessForDoubloons(defaultValues.refreshCostForDoubloons))
            {
                _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
                _sceneDataProvider.Publish(EventNames.Refresh, true);
            }
        }

        private void RefreshForAdv()
        {
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
            _sceneDataProvider.Publish(EventNames.Refresh, true);
        }
    }
}
