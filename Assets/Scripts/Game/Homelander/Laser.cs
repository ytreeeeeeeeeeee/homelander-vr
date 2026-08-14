using UnityEngine;

namespace Game.Homelander
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private LineRenderer glowLine;

        private void Awake()
        {
            glowLine.enabled = false;
            glowLine.positionCount = 2;
        }

        public void Draw(Vector3 startPosition, Vector3 endPosition)
        {
            glowLine.enabled = true;
            glowLine.SetPosition(0, startPosition);
            glowLine.SetPosition(1, endPosition);
        }

        public void Disable()
        {
            glowLine.enabled = false;
        }
    }
}
