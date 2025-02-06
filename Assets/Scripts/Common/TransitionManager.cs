using Core.Data;
using Game.Enums;
using RxExtensions;
using System;
using System.Reactive.Disposables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Common
{
    public class TransitionManager : MonoBehaviour
    {
        private SceneDataProvider _sceneDataProvider;
        private CompositeDisposable _disposables = new();
        
        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            Subscribe();
        }

        private void Subscribe()
        {
            _sceneDataProvider.Receive<int>(EventNames.LoadScene).Subscribe(value =>
            {
                LoadScene(value);

            }).AddTo(_disposables);
        }

        private void LoadScene(int sceneIndex)
        {
            PlayerPrefs.SetInt(EventNames.TargetScene.ToString(), sceneIndex);
            SceneManager.LoadSceneAsync(0);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
