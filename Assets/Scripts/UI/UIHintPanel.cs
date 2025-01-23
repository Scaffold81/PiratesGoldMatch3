using Core.Data;
using Game.Enums;
using Game.ScriptableObjects;
using RxExtensions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using UnityEngine;

public class UIHintPanel : MonoBehaviour
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
        Subscribe();
    }

    private void Subscribe()
    {
        _sceneDataProvider.Receive<bool>(EventNames.GetHintForDoubloons).Subscribe(newValue =>
        {
            GetHintForDoubloons();

        }).AddTo(_disposables);

        _sceneDataProvider.Receive<bool>(EventNames.GetHintForAdv).Subscribe(newValue =>
        {
            GetHintForAward();

        }).AddTo(_disposables);
    }

    private void GetHintForAward()
    {
        var hitMark = (float)_sceneDataProvider.GetValue(Player—urrency.HintMark);
        hitMark += 1; 
        _sceneDataProvider.Publish(Player—urrency.HintMark, hitMark);
        _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.HintPanel);
    }

    private void GetHintForDoubloons()
    {
        var hitMark = (float)_sceneDataProvider.GetValue(Player—urrency.HintMark);
        var doubloons = (float)_sceneDataProvider.GetValue(Player—urrency.Doubloons);
        var defaultValues=(DefaultValuesSO)_sceneDataProvider.GetValue(EventNames.DefaultValues);
       
        if (_doubloonsProcessor.ProcessForDoubloons(defaultValues.hintMarkCostDoubloons))
        {
            hitMark += 1; 
            _sceneDataProvider.Publish(Player—urrency.HintMark, hitMark);
        }
        _sceneDataProvider.Publish(EventNames.UIPanelStateChange, EventNames.HintPanel);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
