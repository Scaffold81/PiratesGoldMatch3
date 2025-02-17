using Core.Data;
using Game.Enums;
using Game.UI;
using UnityEngine;

public class UICustomButton : UICustomButtonBase
{
    [SerializeField]
    private bool _stateValue;

    private void Start()
    {
        Init();
    }

    protected override void OnClick()
    {
        _sceneDataProvider.Publish(SelectedEnumValue, _stateValue);
    }
}