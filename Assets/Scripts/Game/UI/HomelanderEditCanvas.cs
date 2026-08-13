using UnityEngine;
using UnityEngine.Video;

namespace Game.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class HomelanderEditCanvas : MonoBehaviour
    {
        [HideInInspector] public Camera targetCamera;
        
        [SerializeField] private float distance;
        
        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            FitToCamera();
        }

        private void FitToCamera()
        {
            if (null == targetCamera)
            {
                return;
            }
            
            transform.position = targetCamera.transform.position + targetCamera.transform.forward * distance;
            transform.rotation = targetCamera.transform.rotation;

            float worldHeight;
            
            if (targetCamera.orthographic)
            {
                worldHeight = targetCamera.orthographicSize * 2f;
            }
            else
            {
                worldHeight = 2f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            }

            float scale = worldHeight / _rectTransform.rect.height;
            _rectTransform.localScale = Vector3.one * scale;
            
            float requiredWidth = _rectTransform.rect.height * targetCamera.aspect;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, requiredWidth);
        }
    }
}
