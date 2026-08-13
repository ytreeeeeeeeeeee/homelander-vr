using UnityEngine;

namespace Game.Homelander
{
    public class RotationController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform head;
        [SerializeField] private Transform model;
        
        private Quaternion _initialRotation;
        
        public void Awake()
        {
            _initialRotation = head.localRotation;
        }

        private void LateUpdate()
        {
            RotateModel();
            RotateHead();
        }

        private void RotateModel()
        {
            float yaw = cameraTransform.eulerAngles.y;
            
            model.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        private void RotateHead()
        {
            Quaternion camRelativeToBody = Quaternion.Inverse(model.rotation) * cameraTransform.rotation;
            
            head.localRotation = _initialRotation * camRelativeToBody;
        }
    }
}
