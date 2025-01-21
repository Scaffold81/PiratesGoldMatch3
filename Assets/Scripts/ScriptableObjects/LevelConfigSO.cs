using UnityEngine;
using System.Collections.Generic;
using System;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]
    public class LevelConfigSO : ScriptableObject
    {
       public LevelConfig config;
    }

    [Serializable]
    public class LevelConfig
    {
        public int levelId;

        public bool isLevelPassed = false;
        public bool dialogueInLevelPassed = false;
        public bool dialogueInMapPassed = false;
        public float maxScoreValue = 100;

        public string dialogueInLevelName;
        public string dialogueInMapName;
        public string levelName;
    }

    [CreateAssetMenu(fileName = "LevelConfigRepository", menuName = "Configs/LevelConfigRepository")]
    public class LevelConfigRepositorySO : ScriptableObject
    {
        public LevelConfigRepository levelConfigs;
    }

    [Serializable]
    public class LevelConfigRepository 
    {
        public List<LevelConfigSO> levelConfigs = new List<LevelConfigSO>();
    }
}
