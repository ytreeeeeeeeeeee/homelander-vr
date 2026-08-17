using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class ClickController : MonoBehaviour
    {
        // ссылка на Main Camera
        [SerializeField] private Camera cam;
        // Raycaster на Canvas, с которым мы будем взаимодейтсовать
        [SerializeField] private GraphicRaycaster raycaster;
        
        // информация о вирутальном курсоре
        private readonly PointerEventData _pointerData = new(null);
        // резльтаты попадания Raycaster
        private readonly List<RaycastResult> _results = new();

        private void Update()
        {
            // фиксируем нажатие на кнопку на шлеме (аналог KeyCode.Return) и проверяем, куда мы сейчас смотрим
            if (SkyworthVrInput.GetButtonDown(SkyworthVrButton.Confirm))
            {
                ClickCenter();
            }
        }

        private void ClickCenter()
        {
            // очищаем результаты прошлой фиксации
            _results.Clear();

            // ставим виртуальный курсор в центр экрана
            _pointerData.position = new Vector2(Screen.width * 0.5f, Screen.height  * 0.5f);
            
            // определяем какие элементы UI находятся под виртуальным курсором
            raycaster.Raycast(_pointerData, _results);

            if (0 == _results.Count)
            {
                return;
            }
            
            // берем самый верхнеуровневый объект, в который попали
            // то есть если у нас структура Button -> Text, то в go мы запишем именно Button
            GameObject go = _results[0].gameObject;
            // идем, начиная от полученного UI-элемента, вверх по иерархии пока не найдем первый элемент,
            // который может обработать клик. Обычно это Button, у которого заполнен OnClick
            ExecuteEvents.ExecuteHierarchy(go, _pointerData, ExecuteEvents.pointerClickHandler);
        }
    }
}
