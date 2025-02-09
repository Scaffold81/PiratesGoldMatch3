using Core.Data;
using Game.Common;
using Game.Enums;
using Game.ScriptableObjects;
using JetBrains.Annotations;
using RxExtensions;
using System;
using System.Reactive.Disposables;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField]
    private DefaultValuesSO defaultValues;
    [SerializeField]
    private LevelConfigRepositorySO levelConfigRepository;

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

        _sceneDataProvider.Receive<float>(Player—urrency.Doubloons).Subscribe(value =>
        {
            Save(Player—urrency.Doubloons.ToString(), value.ToString());

        }).AddTo(_disposables);

        _sceneDataProvider.Receive<LevelConfigSO>(SaveSlotNames.LevelConfig).Subscribe(value =>
        {
            SaveLevelConfigJson(SaveSlotNames.LevelConfig, value);

        }).AddTo(_disposables); 
        
        _sceneDataProvider.Receive<LevelConfigRepositorySO>(SaveSlotNames.LevelsConfig).Subscribe(value =>
        {
            SaveLevelsConfigJson(SaveSlotNames.LevelsConfig, value);

        }).AddTo(_disposables);
    }

    private void SaveLevelConfigJson(SaveSlotNames key, LevelConfigSO levelConfig )
    {
        _saveSystem.SaveJSON (key.ToString(), levelConfig);
    }

    private void SaveLevelsConfigJson(SaveSlotNames key, LevelConfigRepositorySO levelsConfig)
    {
        _saveSystem.SaveJSON(key.ToString(), levelsConfig);
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

        var levelConfig = _saveSystem.LoadJSON<LevelConfigSO>(SaveSlotNames.LevelConfig.ToString());
        if (levelConfig == null)
        {
            levelConfig = defaultValues.levelConfig;
        }
        _sceneDataProvider.Publish(SaveSlotNames.LevelConfig, levelConfig);

        var levelsConfig = _saveSystem.LoadJSON<LevelConfigRepositorySO>(SaveSlotNames.LevelsConfig.ToString());
        if (levelsConfig == null)
        {
            levelsConfig = defaultValues.levelConfigRepository;
        }
        _sceneDataProvider.Publish(SaveSlotNames.LevelsConfig, levelsConfig);
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
