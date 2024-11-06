using Game.Core.Generators;
using System.Linq;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

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
}
