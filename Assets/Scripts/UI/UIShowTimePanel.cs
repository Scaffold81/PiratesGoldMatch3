using Core.Data;
using Game.Enums;
using RxExtensions;
using System;
using System.Collections;
using System.Reactive.Disposables;
using UnityEngine;

namespace Game.UI
{
    public class UIShowTimePanel : MonoBehaviour
    {
        [SerializeField]
        private EventNames _eventNames;

        private SceneDataProvider _sceneDataProvider;
        private CompositeDisposable _disposables = new();

        [SerializeField]
        private float _time = 1;

        private bool _timerActive=false;
        
        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            Subscribes();
        }

        private void Subscribes()
        {
            _sceneDataProvider.Receive<EventNames>(EventNames.UIPanelStateChange).Subscribe(newValue =>
            {
                if (newValue == _eventNames)
                {
                    if(!_timerActive)
                        StartCoroutine(Timer());
                }
            }).AddTo(_disposables);


        }

        private IEnumerator Timer()
        {
            _timerActive=true;
            yield return new WaitForSeconds(_time);
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
            _timerActive = false;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

