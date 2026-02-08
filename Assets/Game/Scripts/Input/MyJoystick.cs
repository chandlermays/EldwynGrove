using UnityEngine;
using UnityEngine.EventSystems;

namespace EldwynGrove.Input
{
    public class MyJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform m_background;
        [SerializeField] private RectTransform m_handle;
        [SerializeField] private float m_handleRange = 1f;
        [SerializeField] private float m_deadZone = 0f;

        private Vector2 m_inputDirection = Vector2.zero;
        private RectTransform m_baseRect;
        private Canvas m_canvas;
        private Camera m_camera;

        public Vector2 InputDirection => m_inputDirection;

        private void Start()
        {
            m_baseRect = GetComponent<RectTransform>();
            Utilities.CheckForNull(m_baseRect, nameof(m_baseRect));

            m_canvas = GetComponentInParent<Canvas>();
            Utilities.CheckForNull(m_canvas, nameof(m_canvas));

            if (m_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                m_camera = m_canvas.worldCamera;
            }
            else
            {
                m_camera = null;
            }

            Vector2 center = new(0.5f, 0.5f);
            m_background.pivot = center;
            m_handle.pivot = center;
            m_handle.anchorMin = center;
            m_handle.anchorMax = center;
            m_handle.anchoredPosition = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                m_camera = m_canvas.worldCamera;
            }
            else
            {
                m_camera = null;
            }

            Vector2 position = RectTransformUtility.WorldToScreenPoint(m_camera, m_background.position);
            Vector2 radius = m_background.sizeDelta / 2f;
            m_inputDirection = (eventData.position - position) / (radius * m_canvas.scaleFactor);
            HandleInput(m_inputDirection.magnitude, m_inputDirection.normalized, radius, m_camera);
            m_handle.anchoredPosition = m_inputDirection * radius * m_handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_inputDirection = Vector2.zero;
            m_handle.anchoredPosition = Vector2.zero;
        }

        private void HandleInput(float magnitude, Vector2 normalized, Vector2 radius, Camera m_camera)
        {
            if (magnitude > m_deadZone)
            {
                if (magnitude > 1f)
                {
                    m_inputDirection = normalized;
                }
            }
            else
            {
                m_inputDirection = Vector2.zero;
            }
        }
    }
}