using Game.Core.Generators;
using System.Linq;
using UnityEngine;

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
    private NodeBase _selectodNode01; 
    [SerializeField]
    private NodeBase _selectodNode02;

    private void Awake()
    {
        AddNodes();
    }

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
        if(_selectodNode01 == null)
        {
            _selectodNode01 = nodeBase;
        }
        else
        {
            var areNeighbor = AreNodesNeighbors(_selectodNode01, nodeBase);
            
            if (areNeighbor) 
            {

            }
            else
            {
                _selectodNode01 = nodeBase;
            }

        }
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

}
