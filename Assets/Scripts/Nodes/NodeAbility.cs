namespace Game.Gameplay.Nodes
{
    public class NodeAbility
    {
        public NodeBase[,] nodes;

        public NodeAbility(NodeBase[,] nodes)
        {
            this.nodes = nodes;
        }

        public virtual void ActivateAbility(NodeBase node) { }
    }

    public class NodeAbilityLightingHorisontall: NodeAbility
    {
        public NodeAbilityLightingHorisontall(NodeBase[,] nodes) : base(nodes)
        {
        }

        public override void ActivateAbility(NodeBase node) 
        { 
            for(int x=0;x< nodes.GetLength(0); x++)
            {
                if (nodes[x, (int)node.Position.y] != node)
                {
                    if (nodes[x,(int)node.Position.y]._nodeAbility == null)
                        nodes[x,(int)node.Position.y].SetNodeEmpty();
                }
            }
        }
    }

    public class NodeAbilityLightingVertical : NodeAbility
    {
        public NodeAbilityLightingVertical(NodeBase[,] nodes) : base(nodes)
        {
        }

        public override void ActivateAbility( NodeBase node)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes[(int)node.Position.x, y] != node)
                {
                    if (nodes[(int)node.Position.x, y]._nodeAbility == null)
                        nodes[(int)node.Position.x, y].SetNodeEmpty();
                }
            }
        }
    }
    public class NodeAbilityLightingNodeType : NodeAbility
    {
        public NodeAbilityLightingNodeType(NodeBase[,] nodes) : base(nodes)
        {
        }

        public override void ActivateAbility(NodeBase node)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                { 
                    if(nodes[x, y].NodeType == node.NodeType&& nodes[x, y]!=node)
                    {
                        if(nodes[x, y]._nodeAbility==null)
                            nodes[x, y].SetNodeEmpty();
                       // else nodes[x, y]._nodeAbility.ActivateAbility(nodes[x, y]);
                    }
                }
            }
        }
    }

    public class NodeAbilityCrossMatch : NodeAbility
    {
        public NodeAbilityCrossMatch(NodeBase[,] nodes) : base(nodes)
        {
        }

        public override void ActivateAbility(NodeBase node)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                if (nodes[x, (int)node.Position.y] != node)
                {
                    nodes[x, (int)node.Position.y].SetNodeEmpty();
                }
            }

            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes[(int)node.Position.x, y] != node)
                {
                    nodes[(int)node.Position.x, y].SetNodeEmpty();
                }
            }
        }
    }
}