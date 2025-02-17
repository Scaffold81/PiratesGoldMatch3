using Game.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game.Structures
{
    [CreateAssetMenu(fileName = "SubLevelConfig", menuName = "Configs/SubLevelConfig")]
    [System.Serializable]
    public class Sublevel: ScriptableObject
    {
        public string levelName;
        public int numberOfMoves = 30;
        public List<LevelTasks> levelTasks;
        [Multiline]
        public string levelStartDialogue;

        
        public NodeType[] nodeTypes;
        public NodeType[] excludedNodeTypes = { NodeType.Hidden };

        public int rows = 3;
        public int cols = 3;

        public NodeType[,] nodeField;

        public NodeType[,] targetField;
    }
}




