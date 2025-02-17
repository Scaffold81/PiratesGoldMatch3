using Game.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game.Structures
{
    [CreateAssetMenu(fileName = "SubLevelConfig", menuName = "Configs/SubLevelConfig")]
    [System.Serializable]
    public class Sublevel: SerializedScriptableObject
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
        [TableMatrix(HorizontalTitle = "Columns", VerticalTitle = "Rows")]
        public NodeType[,] nodeField; 
        [TableMatrix(HorizontalTitle = "Columns", VerticalTitle = "Rows")]
        public NodeType[,] targetField;
    }
}




