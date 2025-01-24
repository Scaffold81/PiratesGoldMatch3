using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using RxExtensions;
using System;
using System.Reactive.Disposables;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private SceneDataProvider _sceneDataProvider;
    private CompositeDisposable _disposables = new();
    
    private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;

        if (SceneDataProvider.Instance != null)
            Subscribes();
        else
            Debug.LogError("SceneDataProvider provider not found. Please check SceneDataProvider in your scene");
    }
    private void Subscribes()
    {
        _sceneDataProvider.Receive<LevelConfigSO>(SaveSlotNames.LevelConfig).Subscribe(newValue =>
        {
            if (newValue is LevelConfigSO)
            {
                CreateLevel(newValue);
            }
        }).AddTo(_disposables);
    }

    private void CreateLevel(LevelConfigSO newValue)
    {
        var path = "Prefabs/Levels/" + newValue.levelName; // Путь к префабу
        GameObject levelPrefab = Resources.Load<GameObject>(path);

        if (levelPrefab != null)
        {
           var lvl= Instantiate(levelPrefab);// для создания экземпляра префаба
           lvl.transform.SetAsFirstSibling();
        }
        else
        {
            Debug.LogError("Failed to load level prefab: " + newValue.levelName);
        }
    }
    private void CreateUI(LevelConfigSO newValue)
    {
        var path = "Prefabs/UI/" + newValue.gameUIName; // Путь к префабу
        GameObject levelPrefab = Resources.Load<GameObject>(path);

        if (levelPrefab != null)
        {
            Instantiate(levelPrefab);// для создания экземпляра префаба
            _sceneDataProvider.Publish(EventNames.UILoaded, true);
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
