using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(Canvas))]
    public class EyeCanvas : MonoBehaviour
    {
        // расстояние, на котором должен находится относительно камеры
        // на расстоянии ~0.3 canvas перестает отображаться
        [SerializeField] private float distance;
        [SerializeField] public GameObject editSurface;
        [SerializeField] public GameObject laserOverlay;
        
        public Canvas Canvas { get; private set; }
        
        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
            Canvas.planeDistance = distance;
        }
    }
}
