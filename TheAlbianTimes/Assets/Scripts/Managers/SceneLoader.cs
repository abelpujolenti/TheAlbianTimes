using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private string _currentScene = "";
    public void LoadScene(string name)
    {
        if (SceneManager.GetSceneByName(_currentScene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(_currentScene);
        }
        _currentScene = name;
        if (!SceneManager.GetSceneByName(name).isLoaded)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
        }
    }
}
