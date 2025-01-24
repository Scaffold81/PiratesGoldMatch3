using Core.Data;
using System.Reactive.Disposables;
using UnityEngine;

namespace Game.UI
{
    public class UIPanelBase : MonoBehaviour
    {
        protected UIPanelStateController _stateController;
        protected  private SceneDataProvider _sceneDataProvider;
        protected private CompositeDisposable _disposables = new();

        private void Awake()
        {
            _stateController = GetComponent<UIPanelStateController>();
            _stateController.Hide();
        }
        
        public virtual void Init()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
