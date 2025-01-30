using Core.Data;
using Game.Gameplay.Nodes;
using UnityEngine;
using System;
using RxExtensions;
using System.Reactive.Disposables;
using Game.Enums;
using Game.ScriptableObjects;
using System.Linq;

public class GameManagerBase : MonoBehaviour
{
    private SceneDataProvider _sceneDataProvider;
    private CompositeDisposable _disposables = new();
    private MachTreeBase _machTree;
    [SerializeField]
    private NodesSO _nodeRepository; 
    [SerializeField]
    private NodeType[] _nodeTypes;
    [SerializeField]
    private NodeType[] _excludedNodeTypes = { NodeType.Hidden };
    [SerializeField] private EventNames _gameState = EventNames.StartGame;
    [SerializeField]
    private float _targetForWinPiastres = 1000;

    private float _currentPiastres;


    public float CurrentPiastres
    {
        get { return _currentPiastres; }
        set
        {
            _currentPiastres = value;
            if (_currentPiastres >= _targetForWinPiastres && _gameState == EventNames.StartGame)
                Win();
        }
    }

    private void Awake()
    {
        _machTree = GetComponent<MachTreeBase>();
    }

    private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;

        if (SceneDataProvider.Instance != null)
            Subscribes();
        else
            Debug.LogError("SceneDataProvider provider not found. Please check SceneDataProvider in your scene");
        GetLevel();
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

        _sceneDataProvider.Receive<bool>(EventNames.Restart).Subscribe(newValue =>
        {
            Restart();
        }).AddTo(_disposables);

        _sceneDataProvider.Receive<bool>(EventNames.NextLevel).Subscribe(newValue =>
        {
            NextLevel();
        }).AddTo(_disposables);
    }

    private void RefreshBoard()
    {
        _machTree.Refresh();
    }

    private void NoVariants()
    {
        if (_gameState == EventNames.StartGame)
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
        if (_gameState == EventNames.StartGame)
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.LosePanel);
    }

    private void Win()
    {
        if (_gameState != EventNames.StartGame) return;
        _gameState = EventNames.EndGame;
        _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.WinPanel);
        OpenLevel();
    }

    private void GetLevel()
    {
        var level = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
        _targetForWinPiastres = level.targetForWinPiastres;
    }

    private void OpenLevel()
    {
        var levels = (LevelConfigRepositorySO)_sceneDataProvider.GetValue(SaveSlotNames.LevelsConfig);
        var level = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);

        if (levels == null || level == null)
        {
            Debug.LogError("Failed to open level: levels or current level is null.");
            return;
        }

        var levelToOpen = levels.levelConfigs.FirstOrDefault(a => a.levelId == level.levelId + 1);

        if (levelToOpen != null)
        {
            levelToOpen.isLevelOpen = true;
            _sceneDataProvider.Publish(SaveSlotNames.LevelsConfig, levels);
        }
        else
        {
            Debug.LogWarning("Next level not found or already open.");
        }
    }

    private void NextLevel()
    {
        if (_gameState == EventNames.StartGame)return;

        var levels = (LevelConfigRepositorySO)_sceneDataProvider.GetValue(SaveSlotNames.LevelsConfig);
        var level = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);

        if (levels == null || level == null)
        {
            Debug.LogError("Failed to transition to the next level: levels or current level is null.");
            return;
        }

        var nextLevel = levels.levelConfigs.FirstOrDefault(a => a.levelId == level.levelId + 1);

        if (nextLevel != null)
        {
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, nextLevel);
            print(nextLevel.levelId);
            _sceneDataProvider.Publish(EventNames.LoadScene, 2);
            _gameState = EventNames.EndGame;
        }
        else
        {
            Debug.LogWarning("Next level not found or already open.");
        }
    }

    private void Restart()
    {
        _sceneDataProvider.Publish(EventNames.LoadScene, 2);
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

