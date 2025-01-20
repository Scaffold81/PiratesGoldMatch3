using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.ScriptableObjects
{

    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfigs")]
    public class LevelConfigSO : ScriptableObject
    {
        public string levelId;

        public bool isLevelPassed = false;
        public bool dialogueInLevelPassed = false;
        public bool dialogueInMapPassed = false;
        public float maxScoreValue = 100;

    }

    [CreateAssetMenu(fileName = "LevelObjects", menuName = "Configs/LevelObjects")]
    public class LevelObjectsSO : ScriptableObject
    {
        public string levelId;

        public GameObject dialogueInLevelPrefab;
        public GameObject dialogueInMapPrefab;
        public GameObject levelPrefab;

    }

    [CreateAssetMenu(fileName = "LevelConfigRepository", menuName = "Configs/LevelConfigRepository")]
    public class LevelConfigRepositorySO : ScriptableObject
    {
        public List<LevelConfigSO> levelConfigs=new List<LevelConfigSO>();
    }

    [CreateAssetMenu(fileName = "LevelObject", menuName = "Configs/LevelObject")]
    public class LevelObjectRepositorySO : ScriptableObject
    {
        public List<LevelObjectsSO> levelObjects = new List<LevelObjectsSO>();
    }
}
