using Core.Data;
using Game.Common;
using Game.Enums;
using Game.ScriptableObjects;
using RxExtensions;
using System;
using System.Reactive.Disposables;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField]
    private DefaultValuesSO defaultValues;

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
        _sceneDataProvider.Publish(EventNames.DefaultValues, defaultValues);
        Subscribe();
        LoadAndPublishSaves();
    }

    private void Subscribe()
    {
        _sceneDataProvider.Receive<float>(Player—urrency.Piastres).Subscribe(value =>
        {
            Save(Player—urrency.Piastres.ToString(), value.ToString());

        }).AddTo(_disposables);

        _sceneDataProvider.Receive<float>(Player—urrency.HintMark).Subscribe(value =>
        {
            Save(SaveSlotNames.Hints.ToString(), value.ToString());

        }).AddTo(_disposables);

        _sceneDataProvider.Receive<float>(Player—urrency.Doubloons).Subscribe(value =>
        {
            Save(Player—urrency.Doubloons.ToString(), value.ToString());

        }).AddTo(_disposables);

        _sceneDataProvider.Receive<LevelConfig>(SaveSlotNames.LevelConfig).Subscribe(value =>
        {
            SaveLevelConfigJson(SaveSlotNames.LevelConfig, value);

        }).AddTo(_disposables);
    }

    private void SaveLevelConfigJson(SaveSlotNames key, LevelConfig levelConfig )
    {
        _saveSystem.SaveJSON (key.ToString(), levelConfig);
    }

    private void Save(string key, string value)
    {
        _saveSystem.SaveData(key, value);
    }

    public void LoadAndPublishSaves()
    {
        var piastres = GetNumericDataOrDefault(SaveSlotNames.Piastres.ToString(), defaultValues.piastresDefault);
        _sceneDataProvider.Publish(Player—urrency.Piastres, piastres);
       
        var doubloons = GetNumericDataOrDefault(SaveSlotNames.Doubloons.ToString(), defaultValues.doubloonsDefault);
        _sceneDataProvider.Publish(Player—urrency.Doubloons, doubloons);

        var hintMark = GetNumericDataOrDefault(SaveSlotNames.Hints.ToString(), defaultValues.hintMarkDefault);
        _sceneDataProvider.Publish(Player—urrency.HintMark, hintMark);

        var levelConfig = _saveSystem.LoadJSON<LevelConfig>(SaveSlotNames.LevelConfig.ToString());
        _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, levelConfig);
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
