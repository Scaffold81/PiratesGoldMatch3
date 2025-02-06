using UnityEngine;
using System.Collections.Generic;
using System;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]
    public class LevelConfigSO : ScriptableObject
    {
        public int levelId;

        public bool isLevelOpen = false;
        public bool dialogueInLevelPassed = false;
        public bool dialogueInMapPassed = false;
        
        public string dialogueInLevelName;
        public string dialogueInMapName;
        public string levelName;
        public string gameUIName = "UIGameCanvas";

        public float targetForWinPiastres = 100;

    }
}
