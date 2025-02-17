using Game.Structures;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]
    public class LevelConfigSO : ScriptableObject
    {
        public int levelId; 
        public bool isLevelOpen = false;
        public int currentSublevelIndex;
        public List<Sublevel> subLevels = new List<Sublevel>();
    }
}
