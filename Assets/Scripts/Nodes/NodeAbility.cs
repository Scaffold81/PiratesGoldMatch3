using UnityEngine;

namespace Game.Gameplay.Nodes
{
    public class NodeAbility
    {
        public virtual void ActivateAbility(NodeBase[,] nodes, Vector2 position, NodeBase node) { }
    }

    public class NodeAbilityLightingHorisontall: NodeAbility
    {
        public override void ActivateAbility(NodeBase[,] nodes, Vector2 position,NodeBase node) 
        { 
            for(int x=0;x< nodes.GetLength(0); x++)
            {
                if (nodes[x, (int)position.y] != node)
                {
                    nodes[x, (int)position.y].transform.localScale = Vector3.one * 2;
                    nodes[x, (int)position.y].SetNodeEmpty(nodes);
                }
            }
        }
    }

    public class NodeAbilityLightingVertical : NodeAbility
    {
        public override void ActivateAbility(NodeBase[,] nodes, Vector2 position, NodeBase node)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes[(int)position.x, y] != node)
                {
                    nodes[(int)position.x, y].transform.localScale = Vector3.one*2;
                    nodes[(int)position.x, y].SetNodeEmpty(nodes);
                }
            }
        }
    }
}