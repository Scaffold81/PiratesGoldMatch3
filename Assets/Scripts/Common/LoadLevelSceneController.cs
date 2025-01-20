using Game.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSceneController : MonoBehaviour
{
    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        var scene = PlayerPrefs.GetInt(EventNames.TargetScene.ToString(), 1);
        SceneManager.LoadSceneAsync(scene);
        PlayerPrefs.SetInt(EventNames.TargetScene.ToString(), 1);
    }
}
