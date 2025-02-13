using Core.Data;
using Game.Gameplay.Nodes;
using UnityEngine;
using System;
using RxExtensions;
using System.Reactive.Disposables;
using Game.Enums;
using Game.ScriptableObjects;
using System.Linq;
using System.Collections.Generic;
using Game.Structures;
using TMPro;
namespace Game.Gameplay
{
    public class GameManagerBase : MonoBehaviour
    {
        private SceneDataProvider _sceneDataProvider;
        private CompositeDisposable _disposables = new();
        private MachTreeBase _machTree;

        private EventNames _gameState = EventNames.StartGame;

        private List<LevelTasks> _levelTasks;

        private int _numberOfMoves;
        private LevelConfigSO _currentLevel;

        public int NumberOfMoves
        {
            get
            {
                return _numberOfMoves;
            }
            set
            {
                _numberOfMoves = value;
                _sceneDataProvider.Publish(EventNames.NumberOfMoves, (float)_numberOfMoves);
                var outOfMoves = false;
                if (_numberOfMoves <= 0 && _gameState == EventNames.StartGame)
                {
                    Lose();
                    outOfMoves = true;
                }
                else
                    outOfMoves = false;

                _sceneDataProvider.Publish(EventNames.OutOfMoves, outOfMoves);
            }
        }

        private void Awake()
        {
            _machTree = GetComponent<MachTreeBase>();
        }

        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;

            if (SceneDataProvider.Instance == null)return;

            Subscribes();
            GetLevel();
        }

        private void Subscribes()
        {
            _sceneDataProvider.Receive<EventNames>(EventNames.Lose).Subscribe(newValue =>
            {
                Lose();
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
        
        #region Level Management

        private void GetLevel()
        {
            _currentLevel = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
            var currentSubLevel = GetCurrentSublevel(_currentLevel);
            
            _sceneDataProvider.Publish(EventNames.UIPanelsStateChange,EventNames.StartDialoguePanel);

            NumberOfMoves = currentSubLevel.numberOfMoves;
            _levelTasks = currentSubLevel.levelTasks;
           
            _sceneDataProvider.Publish(EventNames.LevelTasks, currentSubLevel.levelTasks);
        }

        private Sublevel GetCurrentSublevel(LevelConfigSO currentLevel)
        {
            var currentSubLevel = currentLevel.subLevels[currentLevel.currentSublevelIndex];
            return currentSubLevel;
        }

        private void OpenSubLevel()
        {
            _currentLevel.currentSublevelIndex += 1;
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, _currentLevel);
            
            if (_currentLevel.currentSublevelIndex >= _currentLevel.subLevels.Count)
            {
                OpenLevel();
            }
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
                _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, levelToOpen);
                _sceneDataProvider.Publish(SaveSlotNames.LevelsConfig, levels);
            }
            else
            {
                Debug.LogWarning("Next level not found or already open.");
            }
        }

        private void NextLevel()
        {
            var level = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
            if (level!= _currentLevel)
            {
                _sceneDataProvider.Publish(EventNames.LoadScene, 1);
            }
            else
            {
                _sceneDataProvider.Publish(EventNames.LoadScene, 2);
            }
        }

        private void Restart()
        {
            var currentLevel = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
            currentLevel.currentSublevelIndex = 0;
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, currentLevel);

            _sceneDataProvider.Publish(EventNames.LoadScene, 2);
        }

        private void Exit()
        {
            var currentLevel = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
            currentLevel.currentSublevelIndex = 0;
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, currentLevel);

            _sceneDataProvider.Publish(EventNames.LoadScene, 1);
        }

        #endregion Level Management

        private void RefreshBoard()
        {
            _machTree.Refresh();
        }

        private void NoVariants()
        {
            if (_gameState == EventNames.StartGame)
                _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
        }

        private void Lose()
        {
           /* if (_gameState != EventNames.StartGame) return;
            var currentLevel = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.LosePanel);
            currentLevel.currentSublevelIndex = 0;
            _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, currentLevel);*/
        }

        private void Win()
        {
            if (_gameState != EventNames.StartGame) return;
           
            _gameState = EventNames.EndGame;
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.WinPanel);

            OpenSubLevel();
        }
        public void AddPiastres(NodeReward reward)
        {
            var piastres = (float?)_sceneDataProvider.GetValue(PlayerÑurrency.Piastres) ?? 0;
            piastres += reward.rewardValue;
            _sceneDataProvider.Publish(PlayerÑurrency.Piastres, piastres);
        }

        public void AddTargetNode(NodeType type)
        {
            var nodeTarget = _levelTasks.Find(a => a.nodeType == type);

            if (nodeTarget == null) return;
            nodeTarget.count -= 1;
            CheckWin(_levelTasks);
        }

        private void CheckWin(List<LevelTasks> levelTasks)
        {
            var win = false;
            foreach (var levelTask in levelTasks)
            {
                win = levelTask.count < 1;
            }
            if (win) Win();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

