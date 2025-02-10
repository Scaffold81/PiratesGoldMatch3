using Game.Enums;

namespace Game.Gameplay.Nodes
{
    [System.Serializable]
    public class LevelTasks
    {
        // тип нод которые надо собрать
        public NodeType nodeType;
        // количество нод которые надо собрать
        public int count;
    }
}

