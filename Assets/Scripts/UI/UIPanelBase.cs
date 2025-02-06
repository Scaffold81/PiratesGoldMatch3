using Core.Data;
using System.Reactive.Disposables;
using UnityEngine;

namespace Game.UI
{
    public class UIPanelBase : GetDataProvider
    {
        protected UIPanelStateController _stateController;

        private void Awake()
        {
            _stateController = GetComponent<UIPanelStateController>();
            _stateController.Hide();
        }
    }
}
