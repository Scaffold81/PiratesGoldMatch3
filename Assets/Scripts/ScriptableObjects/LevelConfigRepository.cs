using System.Collections.Generic;
using System;
using Game.ScriptableObjects;

namespace Game.Structures
{
    [Serializable]
    public class LevelConfigRepository
    {
        public List<LevelConfigSO> levelConfigs = new List<LevelConfigSO>();
    }
}
