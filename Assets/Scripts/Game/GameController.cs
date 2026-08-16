using System;
using Game.NPC;
using Game.UI;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        public event Action OnGameWin;
    
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            NpcManager.Instance.OnAllNpcKilled += WinGame;
            EyeCanvasController.Instance.OnOneSecondToEndEdit += StartLoadingMainMenu;
        }

        private void OnDisable()
        {
            NpcManager.Instance.OnAllNpcKilled -= WinGame;
            EyeCanvasController.Instance.OnOneSecondToEndEdit -= StartLoadingMainMenu;
        }
    
        private void WinGame()
        {
            OnGameWin?.Invoke();
        }

        private void StartLoadingMainMenu()
        {
            SceneLoader.Instance.LoadScene(SceneLoader.MenuSceneName);
        }
    }
}
