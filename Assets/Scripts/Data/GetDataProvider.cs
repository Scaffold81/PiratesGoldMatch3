using Core.Data;
using Game.Enums;
using System;
using UnityEngine;

public class GetDataProvider : EnumProvider
{
    [SerializeField]
    private DataProvidersNames _dataProvidersNames;

    protected DataProviderBase DataProvider { get;private set;}

    protected void GetProvider()
    {
        Type selectedType = Type.GetType(_dataProvidersNames.ToString());
        DataProvider = (DataProviderBase)FindObjectOfType(selectedType);

        if (DataProvider == null)
        {
            Debug.LogError("Object of type: " + _dataProvidersNames.ToString() + " No found");
            return;
        }
    }
}
