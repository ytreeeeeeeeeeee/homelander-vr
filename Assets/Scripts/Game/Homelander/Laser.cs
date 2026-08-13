using UnityEngine;

namespace Game.Homelander
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private LineRenderer coreLine;
        [SerializeField] private LineRenderer glowLine;

        public void Draw(Vector3 startPosition, Vector3 endPosition)
        {
            coreLine.positionCount = 2;
            coreLine.SetPosition(0, startPosition + Vector3.up * 0.001f);
            coreLine.SetPosition(1, endPosition + Vector3.up * 0.001f);
        
            glowLine.positionCount = 2;
            glowLine.SetPosition(0, startPosition);
            glowLine.SetPosition(1, endPosition);
        }

        public void Disable()
        {
            coreLine.positionCount = 0;
            glowLine.positionCount = 0;
        }
    }
}
