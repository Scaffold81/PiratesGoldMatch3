using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using Game.Structures;
using Game.UI;
using UnityEngine;

public class UITasksPanel : MonoBehaviour
{
    private SceneDataProvider _sceneDataProvider;
    private LevelConfigSO _currentLevel;
    private Sublevel _currentSubLevel;

    [SerializeField]
    private RectTransform _leveltaskPanel;
    [SerializeField]
    private UITaskPanel _taskPanel; private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;
        Invoke(nameof(Init), 0.1f);
    }

    private void Init()
    {
        _currentLevel = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
        _currentSubLevel = _currentLevel.subLevels[_currentLevel.currentSublevelIndex];
        CreateTasks();
    }

    private void CreateTasks()
    {
        for (int i = 0; i < _currentSubLevel.levelTasks.Count; i++)
        {
            var targetLevel = Instantiate(_taskPanel);
            targetLevel.transform.SetParent(_leveltaskPanel);
            targetLevel.Init(_currentSubLevel.levelTasks[i].nodeType, _currentSubLevel.levelTasks[i].count.ToString());
        }
    }
}
