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
namespace Game.Gameplay
{
    public class GameManagerBase : MonoBehaviour
    {
        private SceneDataProvider _sceneDataProvider;
        private CompositeDisposable _disposables = new();
        private MachTreeBase _machTree;
        private EventNames _gameState = EventNames.StartGame;
        private int _numberOfMoves;
        private LevelConfigSO level;

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

            if (SceneDataProvider.Instance != null)
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
                BackToMap();
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

        private void Lose()
        {
            if (_gameState == EventNames.StartGame)
                _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.LosePanel);
            level.currentSublevelIndex = 0;
        }

        private void Win()
        {
            if (_gameState != EventNames.StartGame) return;
            _gameState = EventNames.EndGame;
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.WinPanel);
            OpenSubLevel();
        }

        private void GetLevel()
        {
            level = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
            var currentLevel = level.sublevels[level.currentSublevelIndex];
            NumberOfMoves = currentLevel.numberOfMoves;
            _sceneDataProvider.Publish(EventNames.LevelTasks, currentLevel.levelTasks);
        }
        
        private void OpenSubLevel()
        {
            if(level.sublevels.Count < level.currentSublevelIndex)
                level.currentSublevelIndex += 1;
            else
            {
                level.currentSublevelIndex = 0;
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
                _sceneDataProvider.Publish(SaveSlotNames.LevelsConfig, levels);
            }
            else
            {
                Debug.LogWarning("Next level not found or already open.");
            }
        }

        private void BackToMap()
        {
            if (_gameState == EventNames.StartGame) return;

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
                _sceneDataProvider.Publish(EventNames.LoadScene, 1);
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
            _sceneDataProvider.Publish(Player—urrency.Piastres, piastres);
        }

        public void AddTargetNode(NodeType type)
        {
            var levelTasks = (List<LevelTasks>)_sceneDataProvider.GetValue(EventNames.LevelTasks);
           
            var nodeTarget = levelTasks.Find(a => a.nodeType == type);
            
            if (nodeTarget == null)return;
                nodeTarget.count -= 1;

            CheckWin(levelTasks);
        }

        private void CheckWin(List<LevelTasks> levelTasks)
        {
            var win =false;
            foreach (var levelTask in levelTasks) 
            { 
                if(levelTask.count!= 0)
                    win = false;
                else 
                    win = true;
                
            }
            if (win) Win();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}

