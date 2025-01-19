using Core.Data;
using Game.Gameplay.Nodes;
using UnityEngine;
using System;

public class GameManagerBase : MonoBehaviour
{
    private SceneDataProvider _sceneDataProvider;

    private float _doubloons;

    private float _health;
    private float _armor;

    public float Doubloons
    {
        get { return _doubloons; }
        set { }
    }

    public float Health
    {
        get { return _health; }
        set { }
    }

    public float Armor
    {
        get { return _armor; }
        set { }
    }

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
        _sceneDataProvider.Receive<string>(Player—urrency.Piastres).Subscribe(newValue =>
        {

        });
    }

    public void AddPiastres(NodeReward reward)
    {
        var piastres = (float?)_sceneDataProvider.GetValue(Player—urrency.Piastres) ?? 0;
        piastres += reward.rewardValue;
        _sceneDataProvider.Publish(Player—urrency.Piastres, piastres);
    }
}

