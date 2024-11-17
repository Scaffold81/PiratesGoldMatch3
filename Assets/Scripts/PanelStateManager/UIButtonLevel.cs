using Core.Data;
using Game.Enums;
using Game.Structures;
using UnityEngine;

namespace Game.UI
{
    public class UIButtonLevel : UIUICustomButtonBase
    {
        
        private EventNames _targetName = EventNames.LoadLevel;
        

        private SceneDataProvider _sceneDataProvider;

        [SerializeField]
        private LevelConfig _levelConfig;
        
        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
        }

        protected override void OnClick()
        {
            _sceneDataProvider.Publish(_targetName,_levelConfig);
        }
    }
}