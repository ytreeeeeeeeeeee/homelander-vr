using Game.NPC;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text npcCounter;
        
        private void Start()
        {
            NpcManager.Instance.OnNpcKilled += OnNpcKilled;
            GameController.Instance.OnGameWin += HideNpcCounter;
            
            UpdateNpcCounter();
        }

        private void OnDisable()
        {
            NpcManager.Instance.OnNpcKilled -= OnNpcKilled;
            GameController.Instance.OnGameWin -= HideNpcCounter;
        }

        private void OnNpcKilled(Npc _)
        {
            UpdateNpcCounter();
        }

        private void UpdateNpcCounter()
        {
            npcCounter.text = $"{NpcManager.Instance.KilledNpcCount}/{NpcManager.Instance.NpcCount}";
        }
        
        private void HideNpcCounter()
        {
            npcCounter.gameObject.SetActive(false);
        }
    }
}
