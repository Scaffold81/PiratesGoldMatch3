using Core.Data;
using Game.Enums;
using Game.Structures;
using RxExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class UICurrentTaskPanel : UITaskPanel
    {
        private SceneDataProvider _sceneDataProvider;
        private CompositeDisposable _disposables = new();

        [SerializeField]
        private TMP_Text _textCurrentCount;

        private void Start()
        {
            _sceneDataProvider=SceneDataProvider.Instance;
            Subscribe();
        }

        private void Subscribe()
        {
            _sceneDataProvider.Receive<List<LevelTasks>>(EventNames.LevelTasks).Subscribe(newValue =>
            {
                var currentTask= newValue.FirstOrDefault(a=>a.nodeType==_type);
                _textCurrentCount.text = currentTask.count.ToString()+"/";
            }).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
