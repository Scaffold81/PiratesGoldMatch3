using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using RxExtensions;
using System;
using System.Reactive.Disposables;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelConfigSO levelConfig;
    private SceneDataProvider _sceneDataProvider;
    private CompositeDisposable _disposables = new();

    // Start is called before the first frame update
    void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;

        if (SceneDataProvider.Instance != null)
            Subscribes();
        else
            Debug.LogError("SceneDataProvider provider not found. Please check SceneDataProvider in your scene");
    }
    private void Subscribes()
    {
        _sceneDataProvider.Receive<LevelConfig>(SaveSlotNames.LevelConfig).Subscribe(newValue =>
        {
            if (newValue is LevelConfig)
            {
                levelConfig = ScriptableObject.CreateInstance<LevelConfigSO>();
                levelConfig.config = newValue;
                CreateLevel(newValue);
            }
        }).AddTo(_disposables);
    }

    private void CreateLevel(LevelConfig newValue)
    {
        print(newValue.levelName);
        var path = "Prefabs/Levels/" + newValue.levelName; // Путь к префабу
        GameObject levelPrefab = Resources.Load<GameObject>(path);

        if (levelPrefab != null)
        {
            Instantiate(levelPrefab);// для создания экземпляра префаба
        }
        else
        {
            Debug.LogError("Failed to load level prefab: " + newValue.levelName);
        }
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
