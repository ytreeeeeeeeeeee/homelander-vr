using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class ClickController : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private GraphicRaycaster raycaster;
        
        private readonly PointerEventData _pointerData = new(null);
        private readonly List<RaycastResult> _results = new();

        private void Awake()
        {
            _pointerData.pointerId = -1;
        }

        private void Update()
        {
            if (SkyworthVrInput.GetButtonDown(SkyworthVrButton.Confirm))
            {
                ClickCenter();
            }
        }

        private void ClickCenter()
        {
            _results.Clear();

            _pointerData.position = new Vector2(Screen.width * 0.5f, Screen.height  * 0.5f);
            
            raycaster.Raycast(_pointerData, _results);

            if (0 == _results.Count)
            {
                return;
            }
            
            GameObject go = _results[0].gameObject;
            ExecuteEvents.ExecuteHierarchy(go, _pointerData, ExecuteEvents.pointerClickHandler);
        }
    }
}
