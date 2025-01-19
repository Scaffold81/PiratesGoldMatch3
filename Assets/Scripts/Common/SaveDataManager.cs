using Core.Data;
using Game.Common;
using Game.Enums;
using System.Reactive.Disposables;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField]
    private DefaultValues defaultValues;

    private SceneDataProvider _sceneDataProvider;

    private CompositeDisposable _disposables = new();

    private SaveSystem _saveSystem;
    private void Awake()
    {
        _saveSystem = new SaveSystem();
    }

    private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;
        LoadAndPublishSaves();
    }

    public void LoadAndPublishSaves()
    {
        var piastres = GetNumericDataOrDefault(SaveSlotNames.Piastres.ToString(), defaultValues.piastresDefault);
        _sceneDataProvider.Publish(Player—urrency.Piastres, piastres);
       
        var doubloons = GetNumericDataOrDefault(SaveSlotNames.Doubloons.ToString(), defaultValues.doubloonsDefault);
        _sceneDataProvider.Publish(Player—urrency.Doubloons, doubloons);
    }

    private float GetNumericDataOrDefault(string key, float defaultValue)
    {
        var dataString = _saveSystem.LoadDataOrDefault(key, defaultValue.ToString());
        return NumericParser.GetFloat(dataString);
    }

    private void OnDestroy()
    { 
        _disposables.Dispose();
    }
}
