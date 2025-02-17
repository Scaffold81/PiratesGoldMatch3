using Core.Data;
using Game.Common;
using Game.Enums;
using Game.ScriptableObjects;
using Game.Structures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIStartDialogue : MonoBehaviour
    {
        private SceneDataProvider _sceneDataProvider;
        private LevelConfigSO _currentLevel;
        private Sublevel _currentSubLevel;

        [SerializeField]
        private TMP_Text _dialogueText;


        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            Invoke(nameof( Init),0.1f);
        }

        private void Init()
        {
            _currentLevel = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
            _currentSubLevel = _currentLevel.subLevels[_currentLevel.currentSublevelIndex];

            _dialogueText.text = _currentSubLevel.levelStartDialogue;
        }
    }
}
