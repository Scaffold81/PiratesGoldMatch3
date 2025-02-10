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
        var level = (LevelConfigSO)_sceneDataProvider.GetValue(SaveSlotNames.LevelConfig);
        var currentLevel = level.sublevels[level.currentSublevelIndex];
       
        var path = "Prefabs/Levels/" + currentLevel.levelName; // ���� � �������
        GameObject levelPrefab = Resources.Load<GameObject>(path);

        if (levelPrefab != null)
        {
           var lvl= Instantiate(levelPrefab);// ��� �������� ���������� �������
           lvl.transform.SetAsFirstSibling();
        }
        else
        {
            Debug.LogError("Failed to load level prefab: " + currentLevel.levelName);
        }
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
