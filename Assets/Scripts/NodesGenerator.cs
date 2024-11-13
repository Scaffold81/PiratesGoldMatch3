using System.Linq;
using UnityEngine;
namespace Game.Core.Generators
{
    public class NodesGenerator
    {
        private NodeBase[,] nodes;

        public NodesGenerator(NodeType[] nodeTypes, NodeType[] excludedNodeTypes, NodeBase[,] nodes, MachTreeView machTreeView)
        {
            this.nodes = nodes;

            SetNodes(nodeTypes, excludedNodeTypes, machTreeView);
        }

        private void SetNodes(NodeType[] nodeTypes, NodeType[] excludedNodeTypes, MachTreeView machTreeView)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    var node = nodes[x, y];
                    // Set the NodeType of the node
                    var nodeType = GetUniqueNode(nodeTypes, excludedNodeTypes);
                    node.Init(nodeType, machTreeView);
                }
            }
        }

        private NodeType GetUniqueNode(NodeType[] nodeTypes, NodeType[] excludedNodeTypes)
        {
            var randomNode = nodeTypes[Random.Range(0, nodeTypes.Length)];
            int attempts = 0;
            while (attempts < 10000)
            {
                if (!CheckCloseNodesForMatches(randomNode) && !excludedNodeTypes.Contains(randomNode))
                {
                    break;
                }
                randomNode = nodeTypes[Random.Range(0, nodeTypes.Length)];
                attempts++;
            }

            if (attempts >= 10000)
            {
                Debug.Log("Exceeded attempts limit. Could not find a unique node.");
                // Additional handling for when a unique node cannot be found
            }

            return randomNode;
        }

        private bool CheckCloseNodesForMatches(NodeType newNode)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    if (x > 1 && nodes[x - 1, y].NodeType == newNode && nodes[x - 2, y].NodeType == newNode)
                    {
                        return true;
                    }
                    if (x < nodes.GetLength(0) - 2 && nodes[x + 1, y].NodeType == newNode && nodes[x + 2, y].NodeType == newNode)
                    {
                        return true;
                    }
                    if (y > 1 && nodes[x, y - 1].NodeType == newNode && nodes[x, y - 2].NodeType == newNode)
                    {
                        return true;
                    }
                    if (y < nodes.GetLength(1) - 2 && nodes[x, y + 1].NodeType == newNode && nodes[x, y + 2].NodeType == newNode)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}