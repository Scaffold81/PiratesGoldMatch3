using Core.Data;
using Game.Gameplay.Nodes;
using UnityEngine;
using System;
using RxExtensions;
using System.Reactive.Disposables;
using Game.Enums;

public class GameManagerBase : MonoBehaviour
{
    private SceneDataProvider _sceneDataProvider;
    private CompositeDisposable _disposables = new();
    private MachTree _machTree;
    [SerializeField]
    private float _targetPiastres = 1000;

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

    private void Awake()
    {
        _machTree = GetComponent<MachTree>();
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

        _sceneDataProvider.Receive<bool>(EventNames.Hint).Subscribe(newValue =>
        {
            Hint();
        }).AddTo(_disposables);

        _sceneDataProvider.Receive<bool>(EventNames.NoVariants).Subscribe(newValue =>
        {
            NoVariants();
        }).AddTo(_disposables);

        _sceneDataProvider.Receive<bool>(EventNames.Refresh).Subscribe(newValue =>
        {
            RefreshBoard();
        }).AddTo(_disposables);
    }

    private void RefreshBoard()
    {
        _machTree.Refresh();
    }

    private void NoVariants()
    {
        _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
    }

    private void Hint()
    {
        var hintValue = (float)_sceneDataProvider.GetValue(Player—urrency.HintMark);

        if (hintValue <= 0)
        {
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.HintPanel);
        }
        else
        {
            hintValue -= 1;
            _machTree.Hint();
            _sceneDataProvider.Publish(Player—urrency.HintMark, hintValue);
        }
    }

    private void Lose()
    {
        _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.LosePanel);
    }

    private void Win()
    {
        _sceneDataProvider.Publish(EventNames.Win, true);
        _sceneDataProvider.Publish(EventNames.WinPanel, true);
        OpenLevel();
    }

    private void OpenLevel()
    {

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

