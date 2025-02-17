﻿namespace Game.Enums
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
        NoVariantsPanel = 10,
        PlayLevelPanel = 11,
        StartDialoguePanel = 12,
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
        UIPanelStateChange = 22,
        TargetScene = 23,
        Pause = 24,
        GetHintForAdv = 25,
        GetHintForDoubloons = 26,
        Hint = 27,
        NoVariants = 28,
        RefreshForAdv = 29,
        RefreshForDoubloons = 30,
        DefaultValues = 31,
        AdmitDefeat = 32,
        Refresh = 33,
        UILoaded = 34,
        Restart = 35,
        NextLevel = 36,
        SetLevel = 37,
        OutOfMoves = 38,
        LevelTasks = 39,
        NumberOfMoves = 40
        #endregion  GameEventNames
    }
    public enum SaveSlotNames
    {
        None = 0,
        Piastres,
        Doubloons,
        Hints,
        LevelsData,
        LevelConfig,
        LevelsConfig,
        PreviosLevelConfig
    }
}
