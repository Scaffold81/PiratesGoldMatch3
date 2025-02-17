﻿using Core.Data;
using DG.Tweening;
using Game.Enums;
using Game.Gameplay.Generators;
using Game.ScriptableObjects;
using Game.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay.Nodes
{
    public class MachTreeBase : MonoBehaviour
    {
        private NodesGenerator _nodesGenerator;
        private SceneDataProvider _sceneDataProvider;
        private GameManagerBase _gameManager;
        private DefaultValuesSO _defaultValues;
        private Hint _hint;

        [SerializeField]
        private NodesSO _nodeRepository;

        [SerializeField]
        private NodeType[] _nodeTypes;
        [SerializeField]
        private NodeType[] _excludedNodeTypes = { NodeType.Hidden };

        private NodeBase[,] _nodes;
        private List<MatchetNodes> _matchesNodes = new();
        private List<AvalableNodeForMatch> _avalableNodeForMatches = new List<AvalableNodeForMatch>();

        private NodeBase _selectedNode01;
        private NodeBase _selectedNode02;
        private NodeBase _emptyNode = null;

        private bool _isBlock = false;
        private bool _outOfMoves;

        [SerializeField]
        private float _destroyNodeTime = 0.2f;
        [SerializeField]
        private float _moveNodeTime = 0.01f;
        [SerializeField]
        private float _nodeDescentTime = 0.01f;
        [SerializeField]
        private float _executionDelay = 0.01f;
        [SerializeField]
        private float _scaleMultiplier = 1.05f;

        public NodeBase[,] Nodes { get => _nodes; set => _nodes = value; }

        private void Start()
        {
            Init();
        }

        private void Subscribes()
        {
            _sceneDataProvider.Receive<bool>(EventNames.Pause).Subscribe(newValue =>
            {
                _isBlock = newValue;
            });

            _sceneDataProvider.Receive<bool>(EventNames.OutOfMoves).Subscribe(newValue =>
            {
                _outOfMoves = newValue;
            });
        }

        private void Init()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            _nodesGenerator = new NodesGenerator(_nodeRepository);
            _gameManager = GetComponent<GameManagerBase>();
           
            _defaultValues = (DefaultValuesSO)_sceneDataProvider.GetValue(EventNames.DefaultValues);
            _hint = new Hint(_defaultValues._timeToHint);

            var nodes = SetFieldSyze();
            SetField(nodes);

            _nodesGenerator.GenerateNodes(_nodeTypes, _excludedNodeTypes, Nodes, this);

            Invoke(nameof(FindAvailableMatchesHorizontal), 0.1f);
            Subscribes();
        }

        private void SetField(List<NodeBase> nodes)
        {
            foreach (var node in nodes)
            {
                Nodes[(int)node.Position.x, (int)node.Position.y] = node;
                node.Show();
            }
        }

        private List<NodeBase> SetFieldSyze()
        {
            var nodes = GetComponentsInChildren<NodeBase>().ToList();
            var columns = float.MinValue;
            var rows = float.MinValue;

            // Пройти по всем элементам списка и найти максимальное значение координаты x
            foreach (var node in nodes)
            {
                if (node.Position.x > columns)
                {
                    columns = node.Position.x;
                }
                if (node.Position.y > rows)
                {
                    rows = node.Position.y;
                }
            }
            Nodes = new NodeBase[(int)columns + 1, (int)rows + 1];
            return nodes;
        }

        public void SetSelectedNode(NodeBase nodeBase)
        {
            if (_isBlock|| _outOfMoves) return;

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

                StartCoroutine(CheckCloseNodesForMatches());
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

        #region CheckCloseNodesForMatches

        private IEnumerator CheckCloseNodesForMatches()
        {
            var nodesIsMatchNode1 = CheckHorizontalAndVerticalMatches(_selectedNode01);
            yield return new WaitForSeconds(_executionDelay);
            var nodesIsMatchNode2 = CheckHorizontalAndVerticalMatches(_selectedNode02);
            yield return new WaitForSeconds(_executionDelay);

            if (!nodesIsMatchNode1&& !nodesIsMatchNode2)
                Reverse(_selectedNode01, _selectedNode02);
            else 
                _hint.OffHint();
           
            _gameManager.NumberOfMoves -= 1;
            _selectedNode01 = null;
            _selectedNode02 = null;
            CheckForAllMatches();

        }

        private bool CheckHorizontalAndVerticalMatches(NodeBase selectedNode)
        {
            return CheckHorizontalMatches(selectedNode) || CheckVerticalMatches(selectedNode);
        }

        private bool CheckVerticalMatches(NodeBase selectedNode)
        {
            var matchesY = new List<NodeBase>();
            var returnValue = false;

            for (int y = 0; y < Nodes.GetLength(1); y++)
            {
                if (Nodes[(int)selectedNode.Position.x, y].NodeType == selectedNode.NodeType)
                {
                    matchesY.Add(Nodes[(int)selectedNode.Position.x, y]);
                }
                else
                {
                    if (matchesY.Count >= 3)
                    {
                        var nodes = new MatchetNodes();
                        nodes.nodes.AddRange(matchesY);
                        _matchesNodes.Add(nodes); 
                        returnValue = true;
                    }
                    matchesY.Clear(); // Очищаем список после каждой итерации цикла
                }
            }

            if (matchesY.Count >= 3)
            {
                var nodes = new MatchetNodes();
                nodes.nodes.AddRange(matchesY);
                _matchesNodes.Add(nodes);
                returnValue = true;
            }

            return returnValue;
        }

        private bool CheckHorizontalMatches(NodeBase selectedNode)
        {
            var returnValue = false;
            var matchesX = new List<NodeBase>();
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, (int)selectedNode.Position.y].NodeType == selectedNode.NodeType)
                {
                    matchesX.Add(Nodes[x, (int)selectedNode.Position.y]);
                }
                else
                {
                    if (matchesX.Count >= 3)
                    {
                        var nodes = new MatchetNodes();
                        nodes.nodes.AddRange(matchesX);
                        _matchesNodes.Add(nodes);
                        returnValue = true;
                    }
                    matchesX.Clear();
                }
            }
            if (matchesX.Count >= 3)
            {
                var nodes = new MatchetNodes();
                nodes.nodes.AddRange(matchesX);
                _matchesNodes.Add(nodes);
                returnValue = true;
            }
            return returnValue;
        }

        #endregion CheckCloseNodesForMatches

        private bool CheckForAllMatches()
        {
            var foundMatch = false;

            for (int y = 0; y < Nodes.GetLength(1); y++)
            {
                var matchedNodesX = new List<NodeBase>();
                var tempX = 0;

                for (int x = 0; x < Nodes.GetLength(0); x++)
                {
                    if (x == Nodes.GetLength(0) - 1 || Nodes[tempX, y].NodeType != Nodes[x + 1, y].NodeType)
                    {
                        matchedNodesX.Add(Nodes[x, y]);

                        if (matchedNodesX.Count >= 3)
                        {
                            var nodes = new MatchetNodes();
                            nodes.nodes.AddRange(matchedNodesX);
                            if (!ContainsMatchedNodes(nodes))
                            {
                                _matchesNodes.Add(nodes);
                                foundMatch = true;
                            }
                        }
                        matchedNodesX.Clear();
                        tempX = x + 1;
                    }
                    else
                    {
                        matchedNodesX.Add(Nodes[x, y]);
                    }
                }
            }

            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                var matchedNodesY = new List<NodeBase>();
                var tempY = 0;

                for (int y = 0; y < Nodes.GetLength(1); y++)
                {
                    if (y == Nodes.GetLength(1) - 1 || Nodes[x, tempY].NodeType != Nodes[x, y + 1].NodeType)
                    {
                        matchedNodesY.Add(Nodes[x, y]);

                        if (matchedNodesY.Count >= 3)
                        {
                            var nodes = new MatchetNodes();
                            nodes.nodes.AddRange(matchedNodesY);
                            if (!ContainsMatchedNodes(nodes))
                            {
                                _matchesNodes.Add(nodes);
                                foundMatch = true;
                            }
                        }
                        matchedNodesY.Clear();
                        tempY = y + 1;
                    }
                    else
                    {
                        matchedNodesY.Add(Nodes[x, y]);
                    }
                }
            }
            StartCoroutine(nameof(DestroyMatchesNodes));

            return foundMatch;
        }

        private bool ContainsMatchedNodes(MatchetNodes nodes)
        {
            foreach (MatchetNodes existingNodes in _matchesNodes)
            {
                if (existingNodes.nodes.All(node => nodes.nodes.Contains(node)))
                {
                    return true;
                }
            }
            return false;
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
                        if (Nodes[x, y].NodeType == NodeType.Empty)
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

        private bool CheckIfBoardIsFull()
        {
            var returnValue = true;

            foreach (NodeBase node in Nodes)
            {
                if (node.NodeType == NodeType.Empty)
                {
                    return false;
                }
            }

            return returnValue;
        }

        private IEnumerator DestroyMatchesNodes()
        {
            _isBlock = true; 
            
            var abiltyNodes = new List<NodeBase>();

            for (int n = 0; n < _matchesNodes.Count; n++)
            {
                if (_matchesNodes[n].nodes.Count > 4)
                {
                    var middleNode = _matchesNodes[n].nodes[_matchesNodes[n].nodes.Count / 2];
                    middleNode.SetNodeAbility(new NodeAbilityLightingNodeType(Nodes));
                    abiltyNodes.Add(middleNode);
                }
                else if (_matchesNodes[n].nodes.Count == 4)
                {
                    var middleNode = _matchesNodes[n].nodes[_matchesNodes[n].nodes.Count / 2];
                    var isVerticalMatch = _matchesNodes[n].nodes[0].Position.y == _matchesNodes[n].nodes[1].Position.y;
                    
                    if (isVerticalMatch)
                    {
                        // Если узлы вертикальные, устанавливаем middleNode в вертикальное положение и добавляем в список вертикальных узлов
                        abiltyNodes.Add(middleNode);
                        middleNode.SetNodeAbility(new NodeAbilityLightingVertical(Nodes));
                    }
                    else
                    {
                        // Если узлы горизонтальные, устанавливаем middleNode в горизонтальное положение и добавляем в список горизонтальных узлов
                        abiltyNodes.Add(middleNode);
                        middleNode.SetNodeAbility(new NodeAbilityLightingHorisontall(Nodes));
                    }
                }
                else if (_matchesNodes[n].nodes.Count == 3)
                {
                    var centerNode = _matchesNodes[n].nodes[2]; // Получаем центральную ноду

                    // Проверяем центральную ноду на перекрестие по осям Y и X
                    var isCrossMatchY = _matchesNodes[n].nodes[0].Position.y == centerNode.Position.y  // Проверяем горизонтальные узлы
                        && _matchesNodes[n].nodes[1].Position.y == centerNode.Position.y;  // Проверяем горизонтальные узлы

                    var isCrossMatchX = _matchesNodes[n].nodes[1].Position.x == centerNode.Position.x  // Проверяем вертикальные узлы
                        && _matchesNodes[n].nodes[2].Position.x == centerNode.Position.x;  // Проверяем вертикальные узлы

                    if (isCrossMatchY && isCrossMatchX)
                    {
                        abiltyNodes.Add(centerNode);
                        centerNode.SetNodeAbility(new NodeAbilityCrossMatch(Nodes));
                    }
                }
            }

            for (int n = 0; n < _matchesNodes.Count; n++)
            {
                foreach (var node in _matchesNodes[n].nodes)
                {
                    if (!abiltyNodes.Contains(node))
                    {
                        node.SetNodeReaward();
                        node.SetNodeEmpty();

                    }
                    yield return new WaitForSeconds(_destroyNodeTime);
                }
            }
            
            _matchesNodes.Clear(); 
            FindEmptyNodes();
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
                    if (Nodes[(int)topNode.Position.x, (int)topNode.Position.y + 1].NodeType == NodeType.Empty && Nodes[(int)topNode.Position.x, (int)topNode.Position.y + 1].NodeType != NodeType.Hidden)
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
                var matches = CheckForAllMatches();
                if (!matches)
                    FindAvailableMatchesHorizontal();
                _isBlock = false;
            }
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
                _sceneDataProvider.Publish(EventNames.NoVariants, true);
            else 
                _hint.StartHintTimer(_avalableNodeForMatches); 
        }

        private void CheckHorizontalMatch(NodeBase[,] nodes, int x, int y, int offsetX1, int offsetX2, int targetOffsetX, int offsetY1)
        {
            if (nodes[x + offsetX1, y].NodeType == nodes[x + offsetX2, y].NodeType && nodes[x + offsetX1, y].NodeType == nodes[x + targetOffsetX, y + offsetY1].NodeType)
            {

                AvalableNodeForMatch avlableNodes = new AvalableNodeForMatch
                {
                    FirstNode =nodes[x + targetOffsetX, y],
                    SecondNode = nodes[x + targetOffsetX, y + offsetY1]
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
                    FirstNode = nodes[x, y + targetOffset],
                    SecondNode = nodes[x + offsetX1, y + targetOffset]
                };
                _avalableNodeForMatches.Add(avlableNodes);
            }

        }

        public void Reward(NodeBase node)
        {
            _gameManager.AddPiastres(node.NodeReward);
            _gameManager.AddTargetNode(node.NodeType);
        }

        public void Refresh()
        {
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                for (int y = 0; y < Nodes.GetLength(1); y++)
                {
                    if (!_excludedNodeTypes.Contains(Nodes[x, y].NodeType))
                    {
                        Nodes[x, y].NodeType = NodeType.Empty;
                    }
                }
            }
            Init();
        }
    }
}

