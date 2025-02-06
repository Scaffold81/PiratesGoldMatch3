using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Nodes
{

    [CreateAssetMenu(fileName = "NodesRepository", menuName = "Nodes/Repository" + "ScriptableObject", order = 1)]
    public class NodesSO : ScriptableObject
    {
        public List<NodeSO> nodesRepository;
    }
}
