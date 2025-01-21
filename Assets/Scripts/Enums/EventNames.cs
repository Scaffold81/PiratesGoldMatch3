namespace Game.Enums
{
    public enum EventNames
    {
        None = 0,
        #region  UIPanelNames
        DialoguePanel = 1,
        CaperPane = 2,
        MapPanel = 3,
        ShopPanel = 4,
        MainPanel = 5,
        GamePanel = 6,
        Empty1 = 7,
        Empty2 = 8,
        Empty3 = 9,
        Empty4 = 10,
        Empty5 = 11,
        Empty6 = 12,
        Empty7 = 13,
        Empty8 = 14,
        Empty9 = 15,
        #endregion  UIPanelNames

        #region  GameEventNames
        LoadScene = 16,
        StartGame = 17,
        EndGame = 18,
        Win = 19,
        Lose = 20,
        UIPanelStateChange = 21,
        TargetScene = 22,
        Pause = 23,
        Empty14 = 25,
        Empty15 = 26,
        Empty16 = 27,
        Empty17 = 28,
        Empty18 = 29,
        Empty19 = 30
        #endregion  GameEventNames
    }
    public enum SaveSlotNames
    {
        None = 0,
        Piastres, 
        Doubloons,
        LevelsData,
        LevelConfig
    }
}
