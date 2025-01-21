using Core.Data;
using Game.Gameplay.Nodes;
using UnityEngine;
using System;
using RxExtensions;
using System.Reactive.Disposables;
using Game.ScriptableObjects;
using Game.Enums;

public class GameManagerBase : MonoBehaviour
{
    private SceneDataProvider _sceneDataProvider; 
    private CompositeDisposable _disposables = new();
    [SerializeField]
    private float _targetPiastres=1000;

    private float _currentPiastres;

    public float CurrentPiastres
    {
        get { return _currentPiastres; }
        set
        {
            _currentPiastres = value;
            if (_currentPiastres >= _targetPiastres)
                Win();
        }
    }
    private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;

        if (SceneDataProvider.Instance != null)
            Subscribes();
        else
            Debug.LogError("SceneDataProvider provider not found. Please check SceneDataProvider in your scene");
    }

    private void Subscribes()
    {
        _sceneDataProvider.Receive<EventNames>(EventNames.Lose).Subscribe(newValue =>
        {
            Lose();
        }).AddTo(_disposables);
    }

    private void Lose()
    {
        _sceneDataProvider.Publish(EventNames.Pause, true);
        print("Level Lose");
    }

    private void Win()
    {
        _sceneDataProvider.Publish(EventNames.Win, true);
        _sceneDataProvider.Publish(EventNames.Pause, true);
        OpenLevel();
    }
    
    private void OpenLevel()
    {
        OpenLevel();
        print("Level win");
    }
    
    public void AddPiastres(NodeReward reward)
    {
        var piastres = (float?)_sceneDataProvider.GetValue(Player—urrency.Piastres) ?? 0;
        piastres += reward.rewardValue;
        CurrentPiastres += reward.rewardValue;
        _sceneDataProvider.Publish(Player—urrency.Piastres, piastres);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}

