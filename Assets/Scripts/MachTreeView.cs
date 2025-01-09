using Core.Data;
using DG.Tweening;
using Game.Gameplay.Generators;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay.Nodes
{
    public class MachTreeView : MonoBehaviour
    {
        private NodesGenerator _nodesGenerator;
        private SceneDataProvider _sceneDataProvider;
        [SerializeField]
        private NodesSO _nodeRepository;

        [SerializeField]
        private int _rows = 10;
        [SerializeField]
        private int _columns = 7;

        [SerializeField]
        private NodeType[] _nodeTypes;
        [SerializeField]
        private NodeType[] _excludedNodeTypes = { NodeType.Hidden };
        public NodeBase[,] Nodes { get; private set; }

        private List<AvalableNodeForMatch> _avalableNodeForMatches;
        
        private NodeBase _selectedNode01;
        private NodeBase _selectedNode02;
        private NodeBase _emptyNode = null;

        private bool _isBlock = false;

        [SerializeField]
        private float _moveNodeTime = 0.01f;
        [SerializeField]
        private float _nodeDescentTime = 0.01f;
        [SerializeField]
        private float _executionDelay = 0.01f;
        [SerializeField]
        private float _scaleMultiplier = 1.05f;

        private void Awake()
        {
            _nodesGenerator = new NodesGenerator(_nodeRepository);

            AddNodes();
            Invoke(nameof(GridLayoutGroupOff), 1);
            _avalableNodeForMatches = new List<AvalableNodeForMatch>();
        }

        private void Start()
        {
            _sceneDataProvider = new SceneDataProvider();
        }

        private void GridLayoutGroupOff() => GetComponent<GridLayoutGroup>().enabled = false;

        private void AddNodes()
        {
            Nodes = new NodeBase[_columns, _rows];

            var nodes = GetComponentsInChildren<NodeBase>().ToList();

            foreach (var node in nodes)
            {
                Nodes[(int)node.Position.x, (int)node.Position.y] = node;
                node.Show();
            }

            
            _nodesGenerator.GenerateNodes(_nodeTypes, _excludedNodeTypes, Nodes, this);
            Invoke(nameof(FindAvailableMatchesHorizontal), 0.1f);
        }

        public void SetSelectedNode(NodeBase nodeBase)
        {
            if (_isBlock) return;

            if (_selectedNode01 == null)
            {
                _selectedNode01 = nodeBase;
            }
            else
            {
                var areNeighbor = AreNodesNeighbors(_selectedNode01, nodeBase);

                if (areNeighbor)
                {
                    _selectedNode02 = nodeBase;
                    SwitchNodes(_selectedNode01, _selectedNode02, false);
                }
                else
                {
                    _selectedNode01 = nodeBase;
                    _selectedNode02 = null;
                }
            }
        }

        private void SwitchNodes(NodeBase selectedNode01, NodeBase selectedNode02, bool isReverse)
        {
            _isBlock = true;

            var matchNode01 = false;
            var matchNode02 = false;
            var pos01 = selectedNode01.Position;
            var pos02 = selectedNode02.Position;

            var initialScale = selectedNode01.transform.localScale;

            selectedNode01.transform.SetAsLastSibling();

            selectedNode01.transform.DOMove(selectedNode02.transform.position, _moveNodeTime)
                .OnComplete(() =>
                {
                    Nodes[(int)pos02.x, (int)pos02.y] = selectedNode01;
                    selectedNode01.Position = pos02;
                    selectedNode01.Show();
                    selectedNode01.Rename();
                });

            selectedNode01.transform.DOScale(initialScale * _scaleMultiplier, _moveNodeTime / 2)
                .SetEase(Ease.InOutQuart)
                .OnComplete(() =>
                {
                    selectedNode01.transform.DOScale(initialScale, _moveNodeTime / 2)
                    .SetEase(Ease.InOutQuart);
                });


            selectedNode02.transform.DOMove(selectedNode01.transform.position, _moveNodeTime)
            .OnComplete(() =>
            {
                Nodes[(int)pos01.x, (int)pos01.y] = selectedNode02;
                selectedNode02.Position = pos01;
                selectedNode02.Show();
                selectedNode02.Rename();

                matchNode01 = !isReverse && CheckCloseNodesForMatches(selectedNode01);
                matchNode02 = !isReverse && CheckCloseNodesForMatches(selectedNode02);

                if (!matchNode01 && !matchNode02 && !isReverse)
                {
                    Reverse(selectedNode01, selectedNode02);
                }
                else
                {
                    FindEmptyNodes();
                }

                _selectedNode01 = null;
                _selectedNode02 = null;
            });
        }

        private void Reverse(NodeBase selectedNode01, NodeBase selectedNode02)
        {
            var initialScale = selectedNode01.transform.localScale;

            var pos01 = selectedNode01.Position;
            var pos02 = selectedNode02.Position;

            selectedNode01.transform.DOMove(selectedNode02.transform.position, _moveNodeTime)
                .OnComplete(() =>
                {
                    Nodes[(int)pos02.x, (int)pos02.y] = selectedNode01;
                    selectedNode01.Position = pos02;
                    selectedNode01.Show();
                    _selectedNode01 = null;
                });

            selectedNode01.transform.DOScale(initialScale * _scaleMultiplier, _moveNodeTime / 2)
                .SetEase(Ease.InOutQuart)
                .OnComplete(() =>
                {
                    selectedNode01.transform.DOScale(initialScale, _moveNodeTime / 2)
                    .SetEase(Ease.InOutQuart);
                });

            selectedNode02.transform.DOMove(selectedNode01.transform.position, _moveNodeTime)
                .OnComplete(() =>
                {
                    Nodes[(int)pos01.x, (int)pos01.y] = selectedNode02;
                    selectedNode02.Position = pos01;
                    selectedNode02.Show();
                    _selectedNode02 = null;
                    _isBlock = false;
                });
        }

        public bool AreNodesNeighbors(NodeBase node1, NodeBase node2)
        {
            int x1 = (int)node1.Position.x;
            int y1 = (int)node1.Position.y;

            int x2 = (int)node2.Position.x;
            int y2 = (int)node2.Position.y;

            return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) == 1;
        }

        public bool CheckCloseNodesForMatches(NodeBase movedNode)
        {
            var matchesX = new List<NodeBase>();
            var matchesY = new List<NodeBase>();

            var newX = (int)movedNode.Position.x;
            var newY = (int)movedNode.Position.y;

            var newNode = movedNode.NodeType;

            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, newY].NodeType == newNode)
                {
                    matchesX.Add(Nodes[x, newY]);
                }
                else
                {
                    if (matchesX.Count < 3)
                    {
                        matchesX.Clear();
                    }
                    else
                    {
                        foreach (NodeBase node in matchesX)
                        {
                            node.DestroyNode();
                        }
                        return true;
                    }
                }
            }

            if (matchesX.Count < 3)
            {
                matchesX.Clear();
            }
            else
            {
                foreach (NodeBase node in matchesX)
                {
                    node.DestroyNode();
                }
                return true;
            }

            for (int y = 0; y < Nodes.GetLength(1); y++)
            {
                if (Nodes[newX, y].NodeType == newNode)
                {
                    matchesY.Add(Nodes[newX, y]);
                }
                else
                {
                    if (matchesY.Count < 3)
                    {
                        matchesY.Clear();
                    }
                    else
                    {
                        foreach (NodeBase node in matchesY)
                        {
                            node.DestroyNode();
                        }

                        return true;
                    }
                }
            }

            if (matchesY.Count >= 3)
            {
                foreach (NodeBase node in matchesY)
                {
                    node.DestroyNode();
                }
                return true;
            }

            return false; // No combinations of three identical nodes found
        }

        private bool CheckAllNodesForMatches()
        {
            var matchedNodes = new List<NodeBase>();
            var foundMatch = false;

            for (int y = 0; y < Nodes.GetLength(1); y++)
            {
                int consecutiveCountX = 1;

                for (int x = 1; x < Nodes.GetLength(0); x++)
                {
                    if (Nodes[x, y] != null && Nodes[x - 1, y] != null && Nodes[x, y].NodeType == Nodes[x - 1, y].NodeType)
                    {
                        consecutiveCountX++;
                    }
                    else
                    {
                        if (consecutiveCountX >= 3)
                        {
                            for (int i = 0; i < consecutiveCountX; i++)
                            {
                                matchedNodes.Add(Nodes[x - i - 1, y]);
                            }
                        }

                        consecutiveCountX = 1; // Reset the consecutive count
                    }
                }

                if (consecutiveCountX >= 3)
                {
                    for (int i = 0; i < consecutiveCountX; i++)
                    {
                        matchedNodes.Add(Nodes[Nodes.GetLength(0) - i - 1, y]);
                    }
                }
            }

            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                int consecutiveCountY = 1;

                for (int y = 1; y < Nodes.GetLength(1); y++)
                {
                    if (Nodes[x, y] != null && Nodes[x, y - 1] != null && Nodes[x, y].NodeType == Nodes[x, y - 1].NodeType)
                    {
                        consecutiveCountY++;
                    }
                    else
                    {
                        if (consecutiveCountY >= 3)
                        {
                            for (int i = 0; i < consecutiveCountY; i++)
                            {
                                if (!matchedNodes.Contains(Nodes[x, y - i - 1]))
                                {
                                    matchedNodes.Add(Nodes[x, y - i - 1]);
                                }
                            }
                        }

                        consecutiveCountY = 1; // Reset the consecutive count
                    }
                }

                if (consecutiveCountY >= 3)
                {
                    for (int i = 0; i < consecutiveCountY; i++)
                    {
                        if (!matchedNodes.Contains(Nodes[x, Nodes.GetLength(1) - i - 1]))
                        {
                            matchedNodes.Add(Nodes[x, Nodes.GetLength(1) - i - 1]);
                        }
                    }
                }
            }

            if (matchedNodes.Count > 0)
            {
                foundMatch = true;
                foreach (var node in matchedNodes)
                {
                    node.DestroyNode();
                }
                FindEmptyNodes();
            }

            return foundMatch;
        }

        private bool CheckIfBoardIsFull()
        {
            foreach (NodeBase node in Nodes)
            {
                if (node.NodeType == NodeType.Ready)
                {
                    return false;
                }
            }

            return true;
        }

        public void FindAvailableMatchesHorizontal()
        {
            var nodes = Nodes;
            _avalableNodeForMatches.Clear();

            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                for (int x = 0; x < nodes.GetLength(0); x++)
                {
                    if (x < nodes.GetLength(0) - 2)
                    {
                        if (y < nodes.GetLength(1) - 1)
                        {
                            CheckHorizontalMatch(nodes, x, y, 0, 1, 2, 1);
                            CheckHorizontalMatch(nodes, x, y, 0, 2, 1, 1);
                            CheckHorizontalMatch(nodes, x, y, 1, 2, 0, 1);
                        }
                        if (y > 0)
                        {
                            CheckHorizontalMatch(nodes, x, y, 0, 1, 2, -1);
                            CheckHorizontalMatch(nodes, x, y, 0, 2, 1, -1);
                            CheckHorizontalMatch(nodes, x, y, 1, 2, 0, -1);
                        }
                    }

                    if (x < nodes.GetLength(0) - 3)
                    {
                        CheckHorizontalMatch(nodes, x, y, 0, 1, 3, 0);
                        CheckHorizontalMatch(nodes, x, y, 1, 2, 0, 0);
                    }
                }
            }

            FindAvailableMatchesVertical();
        }

        public void FindAvailableMatchesVertical()
        {
            var nodes = Nodes;
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    if (y < nodes.GetLength(1) - 2)
                    {
                        if (x < nodes.GetLength(0) - 1)
                        {
                            CheckVerticalMatch(nodes, x, y, 0, 1, 2, 1);
                            CheckVerticalMatch(nodes, x, y, 0, 2, 1, 1);
                            CheckVerticalMatch(nodes, x, y, 1, 2, 0, 1);
                        }
                        if (x > 0)
                        {
                            CheckVerticalMatch(nodes, x, y, 0, 1, 2, -1);
                            CheckVerticalMatch(nodes, x, y, 0, 2, 1, -1);
                            CheckVerticalMatch(nodes, x, y, 1, 2, 0, -1);
                        }
                    }

                    if (y < nodes.GetLength(1) - 3)
                    {
                        CheckVerticalMatch(nodes, x, y, 0, 1, 3, 0);
                        CheckVerticalMatch(nodes, x, y, 1, 2, 0, 0);
                    }
                }
            }
            if (_avalableNodeForMatches.Count() <= 0)
            {
                print("Avalable matches not found");
            }
        }

        private void CheckHorizontalMatch(NodeBase[,] nodes, int x, int y, int offsetX1, int offsetX2, int targetOffsetX, int offsetY1)
        {
            if (nodes[x + offsetX1, y].NodeType == nodes[x + offsetX2, y].NodeType && nodes[x + offsetX1, y].NodeType == nodes[x + targetOffsetX, y + offsetY1].NodeType)
            {

                AvalableNodeForMatch avlableNodes = new AvalableNodeForMatch
                {
                    NodePosition01 = new Vector2(x + targetOffsetX, y),
                    Node0Position02 = new Vector2(x + targetOffsetX, y + offsetY1)
                };
                _avalableNodeForMatches.Add(avlableNodes);
            }
        }

        private void CheckVerticalMatch(NodeBase[,] nodes, int x, int y, int offset1, int offset2, int targetOffset, int offsetX1)
        {
            if (nodes[x, y + offset1].NodeType == nodes[x, y + offset2].NodeType && nodes[x, y + offset1].NodeType == nodes[x + offsetX1, y + targetOffset].NodeType)
            {
                AvalableNodeForMatch avlableNodes = new AvalableNodeForMatch
                {
                    NodePosition01 = new Vector2(x, y + targetOffset),
                    Node0Position02 = new Vector2(x + offsetX1, y + targetOffset)
                };
                _avalableNodeForMatches.Add(avlableNodes);
            }

        }
        
        private void FindEmptyNodes()
        {
            bool isEmptyNodeFound = false; // Flag to track if an empty node is found

            for (int y = 0; y < Nodes.GetLength(1) && !isEmptyNodeFound; y++)
            {
                for (int x = 0; x < Nodes.GetLength(0); x++)
                {
                    if (y < Nodes.GetLength(1)) // Check if y is not the last line
                    {

                        if (Nodes[x, y].NodeType == NodeType.Ready)
                        {
                            _emptyNode = Nodes[x, y];
                            StartCoroutine(DescentNodeCoroutine());
                            isEmptyNodeFound = true; // Set the flag to true to stop both loops
                            break; // Exit the inner loop
                        }
                    }
                }
            }
        }

        private IEnumerator DescentNodeCoroutine()
        {
            _isBlock = true;

            var emptyNode = _emptyNode;

            if (emptyNode.Position.y != 0)
            {
                var topNode = Nodes[(int)emptyNode.Position.x, (int)emptyNode.Position.y - 1];

                // Store the positions before swapping
                var pos01 = new Vector2Int((int)emptyNode.Position.x, (int)emptyNode.Position.y);
                var pos02 = new Vector2Int((int)emptyNode.Position.x, (int)emptyNode.Position.y - 1);

                var topNodeTransformPosition = Nodes[pos02.x, pos02.y].transform.position;
                // Animate the top node moving to the empty node's position
                yield return topNode.transform.DOMove(emptyNode.transform.position, _nodeDescentTime).WaitForCompletion();

                // Update the positions and display of the nodes
                emptyNode.transform.position = topNodeTransformPosition;

                emptyNode.Position = new Vector2(pos02.x, pos02.y);
                emptyNode.Show();
                emptyNode.Rename();
                Nodes[(int)emptyNode.Position.x, (int)emptyNode.Position.y] = emptyNode;

                yield return new WaitForSeconds(_executionDelay);

                topNode.Position = new Vector2(pos01.x, pos01.y);
                topNode.Show();
                topNode.Rename();
                Nodes[(int)topNode.Position.x, (int)topNode.Position.y] = topNode;

                // Add a delay before the next node movement
                yield return new WaitForSeconds(_executionDelay);

                // Perform any actions after the swap

                if ((int)topNode.Position.y + 1 < Nodes.GetLength(1))
                {
                    if (Nodes[(int)topNode.Position.x, (int)topNode.Position.y + 1].NodeType == NodeType.Ready)
                    {
                        _emptyNode = Nodes[(int)topNode.Position.x, (int)topNode.Position.y + 1];
                        StartCoroutine(DescentNodeCoroutine());
                    }
                    else
                    {
                        FindEmptyNodes();
                    }
                }
                else
                {
                    FindEmptyNodes();
                }
            }
            else
            {
                StartCoroutine(NewNodeCoroutine());
            }
        }

        private IEnumerator NewNodeCoroutine()
        {
            _isBlock = true;

            NodeBase selectedNode = _emptyNode;

            selectedNode.NodeType = _nodesGenerator.GetNewNode(_nodeTypes, _excludedNodeTypes);
            selectedNode.LoadNewSprite();
            selectedNode.transform.position = selectedNode.transform.position + Vector3.up * 100;

            yield return selectedNode.transform.DOMove(selectedNode.transform.position - Vector3.up * 100, 0.05f).WaitForCompletion();

            yield return new WaitForSeconds(_executionDelay);
            FindEmptyNodes();

            var fully = CheckIfBoardIsFull();

            if (fully)
            {
                var matches = CheckAllNodesForMatches();
                if (!matches)
                    FindAvailableMatchesHorizontal();
            }

            _isBlock = false;
        }

        public void Reward(NodeBase node)
        {
            print("Mode reward " + node.NodeReward);
        }
    }
}

