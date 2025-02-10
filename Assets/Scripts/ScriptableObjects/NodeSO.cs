using Game.Enums;
using Game.Structures;
using UnityEngine;

namespace Game.Gameplay.Nodes
{
    [CreateAssetMenu(fileName = "Node", menuName = "Nodes/Node" + "ScriptableObject", order = 1)]
    public class NodeSO : ScriptableObject
    {
        public NodeType nodeType;
        public NodeReward nodeReward;
    }
}
