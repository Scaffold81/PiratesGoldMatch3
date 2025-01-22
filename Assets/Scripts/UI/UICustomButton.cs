using Core.Data;
using Game.Enums;
using Game.UI;
using UnityEngine;

public class UICustomButton : UICustomButtonBase
{
    [SerializeField]
    private bool _stateValue;
    private SceneDataProvider _sceneDataProvider;

    private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;
    }

    protected override void OnClick()
    {
        _sceneDataProvider.Publish(SelectedEnumValue, _stateValue);
    }
}
