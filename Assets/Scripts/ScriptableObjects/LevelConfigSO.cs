using Game.Gameplay.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]
    public class LevelConfigSO : ScriptableObject
    {
        public int levelId;

        public bool isLevelOpen = false;

        public List<LevelTasks> levelTasks;

        public bool dialogueInLevelPassed = false;
        public bool dialogueInMapPassed = false;
        
        public string dialogueInLevelName;
        public string dialogueInMapName;
        public string levelName;
        public string gameUIName = "UIGameCanvas";
        public int NumberOfMoves = 30;

    }
}
