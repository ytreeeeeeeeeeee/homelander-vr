using TMPro;
using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(Canvas))]
    public class EyeCanvas : MonoBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] public GameObject editSurface;
        [SerializeField] public GameObject laserOverlay;
        [SerializeField] public TMP_Text npcCounter;
        
        public Canvas Canvas { get; private set; }
        
        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
            Canvas.planeDistance = distance;
        }
    }
}
