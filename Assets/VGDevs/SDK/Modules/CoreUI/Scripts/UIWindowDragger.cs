using UnityEngine;
using UnityEngine.EventSystems;

namespace VGDevs
{
    public class UIWindowDragger : UIItemBase, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        //[SerializeField] private bool m_boundToScreen = true;
        
        private bool m_isDragging;
        private Vector2 m_pointerOffset;

        public void OnPointerDown(PointerEventData eventData)
        {
            m_isDragging = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Window.RectTransform, eventData.position, eventData.pressEventCamera, out m_pointerOffset);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_isDragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!m_isDragging)
                return;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Window.Canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 pointerPosition);

            //if (m_boundToScreen)
            //{
            //    // TODO: Find a way to clamp the window.
            //}
            
            Window.RectTransform.localPosition = pointerPosition - m_pointerOffset;
        }
    }
}
