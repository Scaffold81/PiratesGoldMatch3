using System.Reactive.Disposables;
using UnityEngine;

namespace Core.Data
{
    public class GetDataProvider : MonoBehaviour
    {
        protected SceneDataProvider _sceneDataProvider;
        protected CompositeDisposable _disposables = new();

        protected virtual void Init()
        {
            _sceneDataProvider = SceneDataProvider.Instance;
            Subscribe();
        }

        protected virtual void Subscribe() { }

        protected virtual void Unsubscribe()
        {
            _disposables.Dispose();
        }
    }

}
