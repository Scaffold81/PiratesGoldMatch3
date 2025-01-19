using Game.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnumProvider : MonoBehaviour
{
    [SerializeField]
    private EnumType selectedEnumType;
    private Enum selectedEnumValue;

    [Dropdown("enumNames")]
    public string selectedEnumValueTemp;
    private List<string> enumNames = new();
    [SerializeField]
    private int selectedIndex;

    public EnumType SelectedEnumType { get => selectedEnumType; set => selectedEnumType = value; }

    public Enum SelectedEnumValue { get => selectedEnumValue; set => selectedEnumValue = value; }

    private void OnValidate()
    {
        EnumSelection(SelectedEnumType);
    }

    private void Awake()
    {
        EnumSelection(SelectedEnumType);
        // Debug.Log("EnumProvider " + SelectedEnumValue);
    }

    public void EnumSelection(EnumType enumType)
    {
        SelectedEnumType = enumType;
        switch (SelectedEnumType)
        {
            case EnumType.EventNames:
                SelectEnumValue(typeof(EventNames));
                break;
            case EnumType.Player—urrency:
                SelectEnumValue(typeof(Player—urrency));
                break;
        }
    }

    private void SelectEnumValue(Type type)
    {
        enumNames = new List<string>(Enum.GetNames(type));
        selectedIndex = enumNames.IndexOf(selectedEnumValueTemp);
        if (selectedIndex < 0)
        {
            selectedIndex = 0;
        }
        SelectedEnumValue = (Enum)Enum.Parse(type, enumNames[selectedIndex]);
    }

}

public enum EnumType
{
    EventNames,
    Player—urrency,

}
