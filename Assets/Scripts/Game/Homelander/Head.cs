using Game.NPC;
using UnityEngine;

namespace Game.Homelander
{
    public class Head : MonoBehaviour
    {
        [SerializeField] private Transform gazeOrigin;
        [SerializeField] private float maxLazerDistance;
        [SerializeField] private GameObject laserPrefab;
        [SerializeField] private float forwardOffset = 0.25f;
        [SerializeField] private float horizontalOffset = 0.25f;
        [SerializeField] private float verticalOffset = 0.2f;
        [SerializeField] private Transform cameraTransform;
        
        private Laser _leftLazer;
        private Laser _rightLazer;

        private void Start()
        {
            transform.position = cameraTransform.position;
            
            _leftLazer = Instantiate(laserPrefab, transform).GetComponent<Laser>();
            _rightLazer = Instantiate(laserPrefab, transform).GetComponent<Laser>();
        }

        public void ShootOutOfEyes()
        {
            Vector3 targetPoint;
            
            Ray ray = new Ray(gazeOrigin.position, gazeOrigin.forward);
    
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPoint = hit.point;

                if (hit.transform.gameObject.CompareTag("NPC"))
                {
                    NpcManager.Instance.Kill(hit.collider, hit.point);
                }
            }
            else
            {
                targetPoint = ray.origin + ray.direction * maxLazerDistance;
            }
            
            DrawLasersFromEyes(targetPoint);
        }
    
        public void StopShooting()
        {
            _leftLazer.Disable();
            _rightLazer.Disable();
        }

        private void DrawLasersFromEyes(Vector3 targetPoint)
        {
            Vector3 leftEye = 
                gazeOrigin.position
                + gazeOrigin.forward * forwardOffset
                - gazeOrigin.right * (SkyworthVrConstants.EyeSeparationMeters + horizontalOffset)
                - gazeOrigin.up * verticalOffset;
            Vector3 rightEye = 
                gazeOrigin.position
                + gazeOrigin.forward * forwardOffset
                + gazeOrigin.right * (SkyworthVrConstants.EyeSeparationMeters + horizontalOffset)
                - gazeOrigin.up * verticalOffset;
            
            _leftLazer.Draw(leftEye, targetPoint);
            _rightLazer.Draw(rightEye, targetPoint);
        }
    }
}
