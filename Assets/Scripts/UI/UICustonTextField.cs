using TMPro;
using System;
using Core.Data;
using RxExtensions;

public class UICustonTextField : EnumProvider
{
    private TMP_Text _text;

    private void Awake()
    {
        EnumSelection(SelectedEnumType);
        _text = GetComponent<TMP_Text>();
        UpdateText(0.ToString());
    }

    private void Start()
    {
        Init();
    }

    protected override void Subscribe()
    {
        _sceneDataProvider.Receive<float>(SelectedEnumValue).Subscribe(newValue =>
        {
            UpdateText(newValue.ToString());
        }).AddTo(_disposables);
    }

    private void UpdateText<T>(T value)
    {
        _text.text = value.ToString();
    }
}
