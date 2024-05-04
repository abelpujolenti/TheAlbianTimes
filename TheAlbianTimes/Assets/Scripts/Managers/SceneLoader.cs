using UnityEngine.SceneManagement;

namespace Managers
{

    public enum ScenesName
    {
        MAIN_MENU = 0,
        WORKSPACE_SCENE = 1,
        PUBLISH_SCENE = 2,
        STATS_SCENE = 3,
        DIALOGUE_SCENE = 4
    }

    public class SceneLoader
    {
        private ScenesName _currentSceneName;

        public void SetScene(ScenesName name)
        {
            SceneManager.LoadScene((int)name);
            
            /*UnloadScene(_currentSceneName);
            _currentSceneName = name;
            LoadScene(name);*/
        }
        private void LoadScene(ScenesName name)
        {
            if (!SceneManager.GetSceneByBuildIndex((int)name).isLoaded)
            {
                SceneManager.LoadScene((int)name, LoadSceneMode.Additive);
            }
        }
        private void UnloadScene(ScenesName name)
        {
            if (SceneManager.GetSceneByBuildIndex((int)name).isLoaded)
            {
                SceneManager.UnloadSceneAsync((int)name);
            }
        }
    }
}
