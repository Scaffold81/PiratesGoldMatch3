using Core.Data;
using Game.Enums;
using Game.Structures;
using Game.UI;
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
            _sceneDataProvider.Receive<LevelConfig>(EventNames.LoadLevel).Subscribe(newValue =>
            {
                LoadLevel();

            }).AddTo(_disposables);
        }

        private void LoadLevel()
        {
            PlayerPrefs.SetInt(EventNames.TargetScene.ToString(), 2);
            SceneManager.LoadSceneAsync(0);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
