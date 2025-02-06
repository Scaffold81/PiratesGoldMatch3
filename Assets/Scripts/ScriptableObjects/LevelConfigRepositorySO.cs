using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfigRepository", menuName = "Configs/LevelConfigRepository")]
    public class LevelConfigRepositorySO : ScriptableObject
    {
        public List<LevelConfigSO> levelConfigs;
    }
}
