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
    public LevelConfigSO levelConfig;
    private SceneDataProvider _sceneDataProvider; 
    private CompositeDisposable _disposables = new();

    private float _doubloons;

    private float _health;
    private float _armor;

    public float Doubloons
    {
        get { return _doubloons; }
        set { }
    }

    public float Health
    {
        get { return _health; }
        set { }
    }

    public float Armor
    {
        get { return _armor; }
        set { }
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
        _sceneDataProvider.Receive<LevelConfig>(SaveSlotNames.LevelConfig).Subscribe(newValue =>
        {
            if (newValue is LevelConfig)
            {
               levelConfig = ScriptableObject.CreateInstance<LevelConfigSO>();
               levelConfig.config = newValue;
            }
        }).AddTo(_disposables);
    }

    public void AddPiastres(NodeReward reward)
    {
        var piastres = (float?)_sceneDataProvider.GetValue(Player�urrency.Piastres) ?? 0;
        piastres += reward.rewardValue;
        _sceneDataProvider.Publish(Player�urrency.Piastres, piastres);
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}

