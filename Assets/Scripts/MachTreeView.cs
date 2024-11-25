using DG.Tweening;
using Game.Core.Generators;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MachTreeView : MonoBehaviour
{
    [SerializeField]
    private int _rows = 10;
    [SerializeField]
    private int _columns = 7;

    [SerializeField]
    private NodeType[] _nodeTypes;
    [SerializeField]
    private NodeType[] _excludedNodeTypes = { NodeType.Hidden };

    [SerializeField]
    private NodeBase[,] _nodes;
    public NodeBase[,] Nodes { get; set; }

    private NodesGenerator _nodesGenerator;
    [SerializeField]
    private NodeBase _selectedNode01;
    [SerializeField]
    private NodeBase _selectedNode02;
    private bool _isBlock = false;
    private List<NodeBase> _emptyNodes;
    private List<NodeBase> _newNodes;

    private void Awake()
    {
        AddNodes();
        Invoke(nameof(GridLayoutGroupOff), 1);

        _emptyNodes = new List<NodeBase>();
        _newNodes = new List<NodeBase>();
    }

    private void GridLayoutGroupOff() => GetComponent<GridLayoutGroup>().enabled = false;

    private void AddNodes()
    {
        Nodes = new NodeBase[_columns, _rows];

        var nodes = GetComponentsInChildren<NodeBase>().ToList();

        foreach (var node in nodes)
        {
            Nodes[(int)node.Position.x, (int)node.Position.y] = node;
            node.Show(Nodes[(int)node.Position.x, (int)node.Position.y].Position);
        }

        _nodesGenerator = new NodesGenerator(_nodeTypes, _excludedNodeTypes, Nodes, this);
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

        // ¬изуализаци€ перемещени€ нод на сцене с использованием DOTween
        selectedNode01.transform.DOMove(selectedNode02.transform.position, 0.5f)
            .OnComplete(() =>
            {
                // ѕосле завершени€ анимации первой ноды обновл€ем позиции в массиве
                Nodes[(int)pos02.x, (int)pos02.y] = selectedNode01;
                selectedNode01.Position = pos02;
                selectedNode01.Show(selectedNode01.Position);
                selectedNode01.Rename();
                _isBlock = false;
            });

        selectedNode02.transform.DOMove(selectedNode01.transform.position, 0.5f)
        .OnComplete(() =>
        {
            // ѕосле завершени€ анимации второй ноды обновл€ем позиции в массиве
            Nodes[(int)pos01.x, (int)pos01.y] = selectedNode02;
            selectedNode02.Position = pos01;
            selectedNode02.Show(Nodes[(int)pos01.x, (int)pos01.y].Position);
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
            _isBlock = false;
        });
    }

    private void Reverse(NodeBase selectedNode01, NodeBase selectedNode02)
    {
        var pos01 = selectedNode01.Position;
        var pos02 = selectedNode02.Position;

        // ¬изуализаци€ перемещени€ нод на сцене с использованием DOTween
        selectedNode01.transform.DOMove(selectedNode02.transform.position, 0.5f)
            .OnComplete(() =>
            {
                // ѕосле завершени€ анимации первой ноды обновл€ем позиции в массиве
                Nodes[(int)pos02.x, (int)pos02.y] = selectedNode01;
                selectedNode01.Position = pos02;
                selectedNode01.Show(Nodes[(int)pos02.x, (int)pos02.y].Position);
                _selectedNode01 = null;
                _isBlock = false;
            });

        selectedNode02.transform.DOMove(selectedNode01.transform.position, 0.5f)
            .OnComplete(() =>
            {
                // ѕосле завершени€ анимации второй ноды обновл€ем позиции в массиве
                Nodes[(int)pos01.x, (int)pos01.y] = selectedNode02;
                selectedNode02.Position = pos01;
                selectedNode02.Show(Nodes[(int)pos01.x, (int)pos01.y].Position);
                _selectedNode02 = null;
                _isBlock = false;
            });
    }

    public bool AreNodesNeighbors(NodeBase node1, NodeBase node2)
    {
        // ѕровер€ем, €вл€ютс€ ли заданные ноды соседними в игровом поле
        int x1 = (int)node1.Position.x;
        int y1 = (int)node1.Position.y;

        int x2 = (int)node2.Position.x;
        int y2 = (int)node2.Position.y;

        // Ќоды соседние, если их позиции отличаютс€ всего на 1 по одной из осей (вертикали или горизонтали)
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) == 1;
    }

    public bool CheckCloseNodesForMatches(NodeBase movedNode)
    {
        var matchesX = new List<NodeBase>();
        var matchesY = new List<NodeBase>();

        var newX = (int)movedNode.Position.x;
        var newY = (int)movedNode.Position.y;

        NodeType newNode = movedNode.NodeType;

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

    private void FindEmptyNodes()
    {
        _isBlock = true; 
        for (int y = 0; y < Nodes.GetLength(1) - 1; y++)
        {
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, 0].NodeType == NodeType.Ready)
                {
                    if (!_newNodes.Contains(Nodes[x, 0]))
                    {
                        _newNodes.Add(Nodes[x, 0]);
                    }

                }

                if (Nodes[x, y].NodeType != NodeType.Ready && Nodes[x, y + 1].NodeType == NodeType.Ready)
                {
                    _emptyNodes.Add(Nodes[x, y]);
                }
            }
        }
        NewNode();
        MoveNode();
    }

    private void MoveNode()
    {
        if (_emptyNodes.Count > 0)
        {
            _isBlock = true;

            // ¬ыбираем случайную ноду из списка
            int randomIndex = UnityEngine.Random.Range(0, _emptyNodes.Count);
            var selectedNode = _emptyNodes[randomIndex];
            var belowNode = Nodes[(int)selectedNode.Position.x, (int)selectedNode.Position.y + 1];
            var pos = selectedNode.Position;

            selectedNode.transform.DOMove(belowNode.transform.position, 0.1f)
                .OnComplete(() =>
                {
                    Nodes[(int)belowNode.Position.x, (int)belowNode.Position.y] = selectedNode;
                    selectedNode.Position = belowNode.Position;
                    selectedNode.Show(selectedNode.Position);
                    selectedNode.Rename();
                    _isBlock = false;
                });

            belowNode.transform.DOMove(selectedNode.transform.position, 0.1f)
                .OnComplete(() =>
                {
                    Nodes[(int)pos.x, (int)pos.y] = belowNode;
                    belowNode.Position = pos;
                    belowNode.Show(belowNode.Position);
                    belowNode.Rename();
                       
                    if((int)selectedNode.Position.y>Nodes.GetLength(1))
                        if (Nodes[(int)selectedNode.Position.x, (int)selectedNode.Position.y+1].NodeType!=NodeType.Ready)
                            _emptyNodes.Remove(selectedNode);

                    if (_emptyNodes.Count > 0)
                        Invoke(nameof(MoveNode), 0.1f);
                    else 
                        Invoke(nameof(FindEmptyNodes), 0.2f);
                });
        }
        else
        {
            
            _isBlock = false;
        }
    }

    private void NewNode()
    {
        if (_newNodes.Count > 0)
        {
            _isBlock = true;

            int randomIndex = UnityEngine.Random.Range(0, _newNodes.Count);

            NodeBase selectedNode = _newNodes[randomIndex];

            // —охран€ем выбранный индекс дл€ последующего удалени€
            int indexToRemove = randomIndex;

            selectedNode.NodeType = _nodesGenerator.GetNewNode(_nodeTypes, _excludedNodeTypes);
            selectedNode.LoadNewSprite();
            selectedNode.transform.position = selectedNode.transform.position + Vector3.up * 100;
            
            selectedNode.transform.DOMove(selectedNode.transform.position - Vector3.up * 100, 0.1f)
                .OnComplete(() =>
                {
                    // ѕровер€ем, что индекс на момент удалени€ все еще действителен
                    if (indexToRemove >= 0 && indexToRemove < _newNodes.Count)
                    {
                        _newNodes.RemoveAt(indexToRemove);

                        // ѕровер€ем, что еще остались ноды в списке дл€ продолжени€
                        if (_newNodes.Count > 0)
                            Invoke(nameof(NewNode), 0.1f);
                        else
                            Invoke(nameof(FindEmptyNodes), 0.1f);
                    }
                });
        }

    }
}

