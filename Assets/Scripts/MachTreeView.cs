using DG.Tweening;
using Game.Core.Generators;
using System;
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
        }
       
        _nodesGenerator = new NodesGenerator(_nodeTypes, _excludedNodeTypes, Nodes, this);
    }

    public void SetSelectedNode(NodeBase nodeBase)
    {
        if(_selectedNode01 == null)
        {
            _selectedNode01 = nodeBase;
        }
        else
        {
            var areNeighbor = AreNodesNeighbors(_selectedNode01, nodeBase);
            
            if (areNeighbor) 
            {
                _selectedNode02 = nodeBase;
                SwitchNodes(_selectedNode01, _selectedNode02);
            }
            else
            {
                _selectedNode01 = null;
                _selectedNode02 = null;
            }

        }
    }
    
    private void SwitchNodes(NodeBase selectedNode01, NodeBase selectedNode02)
    {
        var pos01 = selectedNode01.Position;
        var pos02 = selectedNode02.Position;
       
        // —охран€ем позиции первой и второй нод во временных переменных
        Vector2 tempPosition = selectedNode01.Position;
        selectedNode01.Position = selectedNode02.Position;
        selectedNode02.Position = tempPosition;

        // ¬изуализаци€ перемещени€ нод на сцене с использованием DOTween
        selectedNode01.transform.DOMove(new Vector3(selectedNode02.transform.position.x, selectedNode02.transform.position.y, 0), 0.5f)
            .OnComplete(() =>
            {
                // ѕосле завершени€ анимации первой ноды обновл€ем позицию в массиве
                Nodes[(int)selectedNode01.Position.x, (int)selectedNode01.Position.y] = selectedNode01;
                selectedNode01.Position = pos02;
                _selectedNode01 = null;
            });

        selectedNode02.transform.DOMove(new Vector3(selectedNode01.transform.position.x, selectedNode01.transform.position.y, 0), 0.5f)
            .OnComplete(() =>
            {
                // ѕосле завершени€ анимации второй ноды обновл€ем позицию в массиве
                Nodes[(int)selectedNode01.Position.x, (int)selectedNode01.Position.y] = selectedNode02;
                selectedNode02.Position = pos01;
                _selectedNode02 =null;
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

}
