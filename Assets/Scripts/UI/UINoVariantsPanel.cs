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
            _doubloonsProcessor = new DoubloonsProcessor(_sceneDataProvider);
        }

        protected override void Subscribe()
        {
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

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}
