using UnityEngine;

namespace MainMenu
{
    public class ButtonHandler : MonoBehaviour
    {
        public void GoToGameScene()
        {
            SceneLoader.Instance.LoadScene(SceneLoader.MainSceneName);
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
