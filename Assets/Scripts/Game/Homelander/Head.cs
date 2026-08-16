using Game.NPC;
using Game.UI;
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
        [SerializeField] private LaserAudio laserAudio;
        
        private Laser _leftLaser;
        private Laser _rightLazer;
        private bool _isLaserActive;

        private void Start()
        {
            transform.position = cameraTransform.position;
            
            _leftLaser = Instantiate(laserPrefab, transform).GetComponent<Laser>();
            _rightLazer = Instantiate(laserPrefab, transform).GetComponent<Laser>();
            
            laserAudio.OnCompleted += DisableLasers;
        }

        private void LateUpdate()
        {
            if (_isLaserActive)
            {
                UpdateLasers();
            }
        }

        private void OnDisable()
        {
            laserAudio.OnCompleted -= DisableLasers;
        }

        public void ShootOutOfEyes()
        {
            if (_isLaserActive)
            {
                return;
            }
            
            _isLaserActive = true;
            
            laserAudio.Play();
            
            UpdateLasers();
            EyeCanvasController.Instance.DrawLaserOverlay();
        }
    
        public void StopShooting()
        {
            if (!_isLaserActive)
            {
                return;
            }
            
            laserAudio.Stop();
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
            
            _leftLaser.Draw(leftEye, targetPoint);
            _rightLazer.Draw(rightEye, targetPoint);
        }

        private void UpdateLasers()
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

        private void DisableLasers()
        {
            _leftLaser.Disable();
            _rightLazer.Disable();
            
            EyeCanvasController.Instance.DisableLaserOverlay();
            
            _isLaserActive = false;
        }
    }
}
