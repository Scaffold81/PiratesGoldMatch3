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
    private LevelConfigSO _configSO;
    private void Start()
    {
        _sceneDataProvider = SceneDataProvider.Instance;

        if (SceneDataProvider.Instance != null)
            Subscribes();
        else
            Debug.LogError("SceneDataProvider provider not found. Please check SceneDataProvider in your scene");
        CreateLevel(_configSO);
    }
    private void Subscribes()
    {
        _sceneDataProvider.Receive<LevelConfigSO>(SaveSlotNames.LevelConfig).Subscribe(newValue =>
        {
            _configSO=newValue;

        }).AddTo(_disposables);
    }

    private void CreateLevel(LevelConfigSO newValue)
    {
        var path = "Prefabs/Levels/Level"; // ���� � �������
        GameObject levelPrefab = Resources.Load<GameObject>(path);

        if (levelPrefab != null)
        {
           var lvl= Instantiate(levelPrefab);// ��� �������� ���������� �������
           lvl.transform.SetAsFirstSibling();
        }
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
