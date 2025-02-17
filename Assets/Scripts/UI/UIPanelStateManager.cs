using Core.Data;
using Game.Enums;
using RxExtensions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using UnityEngine;

namespace Game.UI
{
    public class UIPanelStateManager : MonoBehaviour
    {
        private List<UIPanelStateController> _panelStateControllers = new List<UIPanelStateController>();
        private SceneDataProvider _sceneDataProvider;

        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            GetControllers();
            HideAllPanel();
        }

        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            Subscribe();
        }

        private void Subscribe()
        {
            _sceneDataProvider.Receive<EventNames>(EventNames.UIPanelsStateChange).Subscribe(newValue =>
            {
                PanelStateChange(newValue);

            }).AddTo(_disposables);

            _sceneDataProvider.Receive<EventNames>(EventNames.UIPanelStateChange).Subscribe(newValue =>
            {
                SinglePanelStateChange(newValue);

            }).AddTo(_disposables); 
            
            _sceneDataProvider.Receive<bool>(EventNames.UILoaded).Subscribe(newValue =>
            {
                GetControllers();

            }).AddTo(_disposables);
        }

        private void SinglePanelStateChange(EventNames panelName)
        {
            foreach (var panel in _panelStateControllers)
            {
                if (panel.UIPanelName == panelName)
                {
                    if (panel.IsActive)
                        panel.Hide();
                    else
                        panel.Show();

                }
            }
        }

        private void GetControllers()
        {
            _panelStateControllers.Clear();

            UIPanelStateController[] foundControllers = FindObjectsOfType<UIPanelStateController>();

            if (foundControllers != null && foundControllers.Length > 0)
            {
                foreach (UIPanelStateController controller in foundControllers)
                {
                    if (!_panelStateControllers.Contains(controller))
                    {
                        _panelStateControllers.Add(controller);
                    }
                }
               // Debug.Log("UIPanelStateController components found in the scene count ."+ foundControllers.Length);
            }
            else
            {
               // Debug.Log("No UIPanelStateController components found in the scene.");
            }
        }

        private void PanelStateChange(EventNames name)
        {
            foreach (var panel in _panelStateControllers)
            {
                if (panel.UIPanelName == name)
                    panel.Show();
                else
                    panel.Hide();
            }
        }

        private void HideAllPanel()
        {
            foreach (var panel in _panelStateControllers)
            {
                panel.Hide();
            }
        }
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
