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
        HintPanel = 7,
        WinPanel = 8,
        LosePanel = 9,
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
        UIPanelsStateChange = 21,
        UIPanelStateChange= 22,
        TargetScene = 23,
        Pause = 24,
        GetHintForAdv = 25,
        GetHintForDoubloons = 26,
        Hint = 27,
        Empty17 = 28,
        Empty18 = 29,
        Empty19 = 30,
        DefaultValues = 31
        #endregion  GameEventNames
    }
    public enum SaveSlotNames
    {
        None = 0,
        Piastres, 
        Doubloons,
        Hints,
        LevelsData,
        LevelConfig
    }
}
