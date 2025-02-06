using Core.Data;
using UnityEngine;

public class DoubloonsProcessor
{
    private SceneDataProvider _sceneDataProvider;

    public DoubloonsProcessor(SceneDataProvider sceneDataProvider)
    {
        _sceneDataProvider = sceneDataProvider;
    }

    public bool ProcessForDoubloons(float cost)
    {
        var doubloons = (float)_sceneDataProvider.GetValue(Player—urrency.Doubloons);

        if (doubloons >= cost)
        {
            doubloons -= cost;
            _sceneDataProvider.Publish(Player—urrency.Doubloons, doubloons);
            return true;
        }

        return false;
    }
}
