using Core.Data;
using Game.Enums;
using UnityEngine;

namespace Game.Common
{
    public class StartUpScene : MonoBehaviour
    {
        private SceneDataProvider _sceneDataProvider;
        [SerializeField]
        private EventNames _startPanel;
        
        private void Start()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            _sceneDataProvider.Publish(EventNames.UIPanelsStateChange, _startPanel);
        }
    }
}
