using Game.Enums;
using Game.Gameplay.Nodes;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay.Generators
{
    public class NodesGenerator
    {
        private NodeBase[,] nodes;
        private NodeType _previosNode;
        private NodesSO _repository;

        public NodesGenerator(NodesSO nodeRepository)
        {
            _repository = nodeRepository;
        }

        public void GenerateNodes(NodeType[] nodeTypes, NodeType[] excludedNodeTypes, NodeBase[,] nodes, MachTreeBase machTreeView)
        {
            this.nodes = nodes;
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    var node = nodes[x, y];
                    if (nodes[x, y].NodeType == NodeType.Empty)
                    {
                        // Set the NodeType of the node
                        var nodeType = GetUniqueNode(nodeTypes, excludedNodeTypes);
                        var nodeConfig = GetNodeConfig(nodeType);
                        node.Init(nodeType, machTreeView, nodeConfig.nodeReward);
                    }
                    else 
                    {
                        var nodeType = node.NodeType;
                        var nodeConfig = GetNodeConfig(nodeType);
                        node.Init(nodeType, machTreeView, nodeConfig.nodeReward);
                    }
                }
            }

            CheckAndReplaceNodesForMatches(machTreeView);
        }

        public NodeType GetNewNode(NodeType[] nodeTypes, NodeType[] excludedNodeTypes)
        {
            int attempts = 0;
            var randomNode = nodeTypes[Random.Range(0, nodeTypes.Length)];
            
            while (attempts < 100)
            {
                if (!CheckCloseNodesForMatches(randomNode) && !excludedNodeTypes.Contains(randomNode)&&_previosNode== randomNode) {
                    
                    _previosNode = randomNode;
                    break; 
                }
                randomNode = nodeTypes[Random.Range(0, nodeTypes.Length)];
                attempts++;
            }
            return randomNode;
        }

        private NodeType GetUniqueNode(NodeType[] nodeTypes, NodeType[] excludedNodeTypes)
        {
            int attempts = 0;
            var randomNode = nodeTypes[Random.Range(0, nodeTypes.Length)];
           
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

        public void CheckAndReplaceNodesForMatches(MachTreeBase machTreeView)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    // Horizontal check
                    if (x < nodes.GetLength(0) - 2 && nodes[x, y].NodeType == nodes[x + 1, y].NodeType && nodes[x + 1, y].NodeType == nodes[x + 2, y].NodeType)
                    {
                        int centerX = x + 1;
                        ReplaceNode(centerX, y, nodes[x + 1, y].NodeType, machTreeView);
                    }

                    // Vertical check
                    if (y < nodes.GetLength(1) - 2 && nodes[x, y].NodeType == nodes[x, y + 1].NodeType && nodes[x, y + 1].NodeType == nodes[x, y + 2].NodeType)
                    {
                        int centerY = y + 1;
                        ReplaceNode(x, centerY, nodes[x, y + 1].NodeType, machTreeView);
                    }
                }
            }
        }

        private void ReplaceNode(int x, int y, NodeType newNode, MachTreeBase machTreeView)
        {
            var nodeConfig = GetNodeConfig(newNode);
            nodes[x, y].Init(newNode, machTreeView, nodeConfig.nodeReward); // Replace the node at position (x, y) with the new node
        }

        private NodeSO GetNodeConfig(NodeType newNode)
        {
            return _repository.nodesRepository.FirstOrDefault(q => q.nodeType == newNode);
        }
    }
}
