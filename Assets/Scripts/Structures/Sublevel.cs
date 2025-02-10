﻿using Game.Gameplay.Nodes;
using System;
using System.Collections.Generic;

namespace Game.Structures
{
    [Serializable]
    public class Sublevel
    {
        public string levelName;
        public int numberOfMoves = 30;
        public List<LevelTasks> levelTasks;
    }
}
