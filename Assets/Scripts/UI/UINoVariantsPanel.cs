using System;
using Core.Data;
using Game.Enums;
using System.Reactive.Disposables;
using UnityEngine;
using RxExtensions;
using Game.ScriptableObjects;

public class UINoVariantsPanel : MonoBehaviour
{
    private SceneDataProvider _sceneDataProvider;
    private DoubloonsProcessor _doubloonsProcessor;
    private CompositeDisposable _disposables = new();
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _sceneDataProvider = SceneDataProvider.Instance;
        _doubloonsProcessor = new DoubloonsProcessor(_sceneDataProvider);
        Subscribes();
    }

    private void Subscribes()
    {
        _sceneDataProvider.Receive<bool>(EventNames.RefreshForAdv).Subscribe(newValue =>
        {
            RefreshForAdv();
        }).AddTo(_disposables);
        
        _sceneDataProvider.Receive<bool>(EventNames.RefreshForDoubloons).Subscribe(newValue =>
        {
            RefreshForDoubloons();
        }).AddTo(_disposables);

        _sceneDataProvider.Receive<bool>(EventNames.AdmitDefeat).Subscribe(newValue =>
        {
            AdmitDefeat();
        }).AddTo(_disposables);
    }

    private void AdmitDefeat()
    {
        _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
        _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.LosePanel);
    }

    private void RefreshForDoubloons()
    {
        var defaultValues = (DefaultValuesSO)_sceneDataProvider.GetValue(EventNames.DefaultValues);

        if (_doubloonsProcessor.ProcessForDoubloons(defaultValues.refreshCostForDoubloons))
        {
            _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.NoVariantsPanel);
            _sceneDataProvider.Publish(EventNames.Refresh, true);
        }
    }

    private void RefreshForAdv()
    {
        print("RefreshForAdv");
        _sceneDataProvider.Publish(EventNames.Refresh, true); 
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
