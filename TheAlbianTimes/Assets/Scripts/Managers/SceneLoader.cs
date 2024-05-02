using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneLoader
    {
        private string _currentScene = "";

        public void SetScene(string name)
        {
            UnloadScene(_currentScene);
            _currentScene = name;
            LoadScene(name);
        }
        public void LoadScene(string name)
        {
            if (!SceneManager.GetSceneByName(name).isLoaded)
            {
                SceneManager.LoadScene(name, LoadSceneMode.Additive);
            }
        }
        public void UnloadScene(string name)
        {
            if (SceneManager.GetSceneByName(name).isLoaded)
            {
                SceneManager.UnloadSceneAsync(name);
            }
        }
    }
}
