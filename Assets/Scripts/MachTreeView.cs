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
        FindAvailableMatchesHorizontal(Nodes);
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

        // Визуализация перемещения нод на сцене с использованием DOTween
        selectedNode01.transform.DOMove(selectedNode02.transform.position, 0.5f)
            .OnComplete(() =>
            {
                // После завершения анимации первой ноды обновляем позиции в массиве
                Nodes[(int)pos02.x, (int)pos02.y] = selectedNode01;
                selectedNode01.Position = pos02;
                selectedNode01.Show(selectedNode01.Position);
                selectedNode01.Rename();
                _isBlock = false;
            });

        selectedNode02.transform.DOMove(selectedNode01.transform.position, 0.5f)
        .OnComplete(() =>
        {
            // После завершения анимации второй ноды обновляем позиции в массиве
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

        // Визуализация перемещения нод на сцене с использованием DOTween
        selectedNode01.transform.DOMove(selectedNode02.transform.position, 0.5f)
            .OnComplete(() =>
            {
                // После завершения анимации первой ноды обновляем позиции в массиве
                Nodes[(int)pos02.x, (int)pos02.y] = selectedNode01;
                selectedNode01.Position = pos02;
                selectedNode01.Show(Nodes[(int)pos02.x, (int)pos02.y].Position);
                _selectedNode01 = null;
                _isBlock = false;
            });

        selectedNode02.transform.DOMove(selectedNode01.transform.position, 0.5f)
            .OnComplete(() =>
            {
                // После завершения анимации второй ноды обновляем позиции в массиве
                Nodes[(int)pos01.x, (int)pos01.y] = selectedNode02;
                selectedNode02.Position = pos01;
                selectedNode02.Show(Nodes[(int)pos01.x, (int)pos01.y].Position);
                _selectedNode02 = null;
                _isBlock = false;
            });
    }

    public bool AreNodesNeighbors(NodeBase node1, NodeBase node2)
    {
        // Проверяем, являются ли заданные ноды соседними в игровом поле
        int x1 = (int)node1.Position.x;
        int y1 = (int)node1.Position.y;

        int x2 = (int)node2.Position.x;
        int y2 = (int)node2.Position.y;

        // Ноды соседние, если их позиции отличаются всего на 1 по одной из осей (вертикали или горизонтали)
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

    private bool CheckAllNodesForMatches()
    {
        List<NodeBase> matchedNodes = new List<NodeBase>();
        bool foundMatch = false;

        // Проверка строк на матчи
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

        // Проверка столбцов на матчи
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
        }
        else
        {

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

    public bool FindAvailableMatchesHorizontal(NodeBase[,] nodes)
    {
        var match=false;

        for (int y = 0; y < nodes.GetLength(1); y++)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                if (x < nodes.GetLength(0) - 2)
                {
                    // Проверка горизонтальных сочетаний снизу вверх
                    if (y < nodes.GetLength(1) - 1)
                    {
                        match= CheckHorizontalMatch(nodes, x, y, 0, 1, 2, 1);
                        match = CheckHorizontalMatch(nodes, x, y, 0, 2, 1, 1);
                        match = CheckHorizontalMatch(nodes, x, y, 1, 2, 0, 1);
                    }

                    // Проверка горизонтальных сочетаний сверху вниз
                    if (y > 0)
                    {
                        match = CheckHorizontalMatch(nodes, x, y, 0, 1, 2, -1);
                        match = CheckHorizontalMatch(nodes, x, y, 0, 2, 1, -1);
                        match = CheckHorizontalMatch(nodes, x, y, 1, 2, 0, -1);
                    }
                }

                if (x < nodes.GetLength(0) - 3)
                {
                    // Проверка горизонтальных сочетаний в одной строке
                    match = CheckHorizontalMatch(nodes, x, y, 0, 1, 3, 0);
                    match = CheckHorizontalMatch(nodes, x, y, 1, 2, 0, 0);
                }
            }
        }
        /// Мы будем заполнять список и если он не пустой то возвращать true
        print(match);
        return match;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="offsetX1"></param> 
    /// <param name="offsetX2"></param>
    /// <param name="targetOffsetX"></param>
    /// <param name="offsetY1"></param>
    private bool CheckHorizontalMatch(NodeBase[,] nodes, int x, int y, int offsetX1, int offsetX2, int targetOffsetX, int offsetY1)
    {
        var match = false;

        if (nodes[x + offsetX1, y].NodeType == nodes[x + offsetX2, y].NodeType && nodes[x + offsetX1, y].NodeType == nodes[x + targetOffsetX, y + offsetY1].NodeType)
        {
            nodes[x + targetOffsetX, y].TestShowText();
            nodes[x + targetOffsetX, y + offsetY1].TestShowText();
            match=true;
            
        }
        print("CheckHorizontalMatch "+match);
        return match;
    }

    private void FindEmptyNodes()
    {
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

                if (y < Nodes.GetLength(1) - 1) // Check if y is not the last line
                {
                    if (Nodes[x, y].NodeType != NodeType.Ready && Nodes[x, y + 1].NodeType == NodeType.Ready)
                    {
                        _emptyNodes.Add(Nodes[x, y]);
                    }
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

            int randomIndex = UnityEngine.Random.Range(0, _emptyNodes.Count);
            int indexToRemove = randomIndex;
            var selectedNode = _emptyNodes[randomIndex];

            if ((int)selectedNode.Position.y + 1 < Nodes.GetLength(1))
            {
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
                        // Check the array bounds before accessing elements
                        if ((int)pos.x < Nodes.GetLength(0) && (int)pos.y < Nodes.GetLength(1) &&
                            (int)belowNode.Position.x < Nodes.GetLength(0) && (int)belowNode.Position.y < Nodes.GetLength(1))
                        {
                            Nodes[(int)pos.x, (int)pos.y] = belowNode;
                            belowNode.Position = pos;
                            belowNode.Show(belowNode.Position);
                            belowNode.Rename();

                            if ((int)selectedNode.Position.y + 1 < Nodes.GetLength(1) &&
                                Nodes[(int)selectedNode.Position.x, (int)selectedNode.Position.y + 1].NodeType != NodeType.Ready)
                            {
                                _emptyNodes.Remove(selectedNode);
                            }

                            if (_emptyNodes.Count > 0)
                                Invoke(nameof(MoveNode), 0.1f);
                            else
                                Invoke(nameof(FindEmptyNodes), 0.2f);
                            _isBlock = false;
                        }
                    });
            }
            else
            {
                if (_emptyNodes.Count > 0)
                {
                    _emptyNodes.Clear();
                    Invoke(nameof(FindEmptyNodes), 0.2f);
                }
                _isBlock = false;
            }
        }
    }

    private void NewNode()
    {
        if (_newNodes.Count > 0)
        {
            _isBlock = true;

            int randomIndex = UnityEngine.Random.Range(0, _newNodes.Count);

            NodeBase selectedNode = _newNodes[randomIndex];

            // Сохраняем выбранный индекс для последующего удаления
            int indexToRemove = randomIndex;

            selectedNode.NodeType = _nodesGenerator.GetNewNode(_nodeTypes, _excludedNodeTypes);
            selectedNode.LoadNewSprite();
            selectedNode.transform.position = selectedNode.transform.position + Vector3.up * 100;

            selectedNode.transform.DOMove(selectedNode.transform.position - Vector3.up * 100, 0.1f)
                .OnComplete(() =>
                {
                    // Проверяем, что индекс на момент удаления все еще действителен
                    if (indexToRemove >= 0 && indexToRemove < _newNodes.Count)
                    {
                        _newNodes.RemoveAt(indexToRemove);

                        // Проверяем, что еще остались ноды в списке для продолжения
                        if (_newNodes.Count > 0)
                            Invoke(nameof(NewNode), 0.1f);
                        else
                            Invoke(nameof(FindEmptyNodes), 0.1f);
                    }

                    var fully = CheckIfBoardIsFull();

                    if (fully)
                    {
                        var matches = CheckAllNodesForMatches();
                        if (!matches)
                            print("Available Matches Horizontal " + FindAvailableMatchesHorizontal(Nodes));
                    }

                    _isBlock = false;
                });
        }

    }
}

