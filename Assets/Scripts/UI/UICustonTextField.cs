using TMPro;
using System;
using Core.Data;

public class UICustonTextField : EnumProvider
{
    private SceneDataProvider _sceneDataProvider;
    private TMP_Text _text;

    private void Awake()
    {
        EnumSelection(SelectedEnumType);
        _text = GetComponent<TMP_Text>();
        UpdateText(0.ToString());
    }

    private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;
        if (_sceneDataProvider != null)
        {
            Subscribes();
        }
    }

    private void Subscribes()
    {
        _sceneDataProvider.Receive<float>(SelectedEnumValue).Subscribe(newValue =>
        {
            UpdateText(newValue.ToString());
        });
    }

    private void UpdateText<T>(T value)
    {
        _text.text = value.ToString();
    }
}
