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
    private bool _isBlock=false;

    private void Awake()
    {
        AddNodes();
        Invoke(nameof(GridLayoutGroupOff), 1);
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
        if(_isBlock)return;
        
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
                selectedNode01.Show(Nodes[(int)pos02.x, (int)pos02.y].Position);
                matchNode01 = isReverse ? false : CheckCloseNodesForMatches(selectedNode01);
                
                _isBlock=false;
            });

        selectedNode02.transform.DOMove(selectedNode01.transform.position, 0.5f)
            .OnComplete(() =>
            {
                // ѕосле завершени€ анимации второй ноды обновл€ем позиции в массиве
                Nodes[(int)pos01.x, (int)pos01.y] = selectedNode02;
                selectedNode02.Position = pos01;
                selectedNode02.Show(Nodes[(int)pos01.x, (int)pos01.y].Position);
                matchNode02 = isReverse ? false : CheckCloseNodesForMatches(selectedNode02);

                if (!matchNode01 && !matchNode02 && !isReverse)
                    Reverse(selectedNode01, selectedNode02);
                _isBlock=false;
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
       var matches = new List<NodeBase>
       {
           movedNode // ƒобавление перемещенной ноды в список
       };

        int newX = (int)movedNode.Position.x;
        int newY = (int)movedNode.Position.y;

        NodeType newNode = movedNode.NodeType;

        // ѕроверка на три одинаковые ноды подр€д по горизонтали и вертикали
        if (newX > 1 && Nodes[newX - 1, newY].NodeType == newNode && Nodes[newX - 2, newY].NodeType == newNode)
        {
            matches.Add(Nodes[newX - 1, newY]); // ƒобавление первой совпавшей ноды
            matches.Add(Nodes[newX - 2, newY]); // ƒобавление второй совпавшей ноды

            int nextX = newX - 3;
            while (nextX >= 1 && Nodes[nextX, newY].NodeType == newNode)
            {
                matches.Add(Nodes[nextX, newY]);
                nextX--;
            }
            DestroyNodes(matches);
            _selectedNode01 = null;
            _selectedNode02 = null;
            print("Matches count " + matches.Count);
            return true; // “ри ноды по горизонтали слева
        }

        if (newX < Nodes.GetLength(0) - 2 && Nodes[newX + 1, newY].NodeType == newNode && Nodes[newX + 2, newY].NodeType == newNode)
        {
            matches.Add(Nodes[newX + 1, newY]); // ƒобавление первой совпавшей ноды
            matches.Add(Nodes[newX + 2, newY]); // ƒобавление второй совпавшей ноды

            int nextX = newX + 3;
            while (nextX < Nodes.GetLength(0) - 2 && Nodes[nextX, newY].NodeType == newNode)
            {
                matches.Add(Nodes[nextX, newY]);
                nextX++;
            }
            DestroyNodes(matches);
            _selectedNode01 = null;
            _selectedNode02= null;
            print("Matches count " + matches.Count);
            return true; // “ри ноды по горизонтали справа
        }

        if (newY > 1 && Nodes[newX, newY - 1].NodeType == newNode && Nodes[newX, newY - 2].NodeType == newNode)
        {
            matches.Add(Nodes[newX, newY - 1]); // ƒобавление первой совпавшей ноды
            matches.Add(Nodes[newX, newY - 2]); // ƒобавление второй совпавшей ноды

            int nextY = newY - 3;
            while (nextY >= 1 && Nodes[newX, nextY].NodeType == newNode)
            {
                matches.Add(Nodes[newX, nextY]);
                nextY--;
            }
            DestroyNodes(matches);
            _selectedNode01 = null;
            _selectedNode02 = null;
            print("Matches count " + matches.Count);
            return true; // “ри ноды по вертикали вниз
        }

        if (newY < Nodes.GetLength(1) - 2 && Nodes[newX, newY + 1].NodeType == newNode && Nodes[newX, newY + 2].NodeType == newNode)
        {
            matches.Add(Nodes[newX, newY + 1]); // ƒобавление первой совпавшей ноды
            matches.Add(Nodes[newX, newY + 2]); // ƒобавление второй совпавшей ноды

            int nextY = newY + 3;
            while (nextY < Nodes.GetLength(1) - 2 && Nodes[newX, nextY].NodeType == newNode)
            {
                matches.Add(Nodes[newX, nextY]);
                nextY++;
            }
            print("Matches count " + matches.Count);
            DestroyNodes(matches);
            _selectedNode01 = null;
            _selectedNode02 = null;
            return true; // “ри ноды по вертикали вверх
        }


        return false; // Ќе найдено комбинаций из трех одинаковых нод
    }

    private void DestroyNodes(List<NodeBase> matches)
    {
        if (matches.Count >= 3)
        {
            foreach (NodeBase node in matches)
            {
                node.DestroyNode();
            }
        }
    }
}

