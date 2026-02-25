using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
//---------------------------------

namespace EldwynGrove
{
    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform m_target;
        [SerializeField] private BoxCollider2D m_cameraBounds;
        [SerializeField] private float m_zoomSensitivity = 0.1f;
        [SerializeField] private Vector2 m_zoomRange = new Vector2(2f, 10f);

        private Camera m_camera;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_target, nameof(m_target));
            Utilities.CheckForNull(m_cameraBounds, nameof(m_cameraBounds));

            m_camera = GetComponent<Camera>();
            Utilities.CheckForNull(m_camera, nameof(m_camera));
        }

        /*------------------------------------------------------------------------
        | --- LateUpdate: Called after all Update functions have been called --- |
        ------------------------------------------------------------------------*/
        private void LateUpdate()
        {
            HandlePinchZoom();

            Vector3 targetPosition = m_target.position;
            Bounds bounds = m_cameraBounds.bounds;

            // Calculate the camera's half dimensions in world space
            float cameraHalfHeight = m_camera.orthographicSize;
            float cameraHalfWidth = m_camera.aspect * cameraHalfHeight;

            // Clamp the target position within the bounds
            float clampedX = Mathf.Clamp(targetPosition.x, bounds.min.x + cameraHalfWidth, bounds.max.x - cameraHalfWidth);
            float clampedY = Mathf.Clamp(targetPosition.y, bounds.min.y + cameraHalfHeight, bounds.max.y - cameraHalfHeight);

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

        /*--------------------------------------------------------------------
        | --- HandlePinchZoom: Adjusts orthographic size via two fingers --- |
        --------------------------------------------------------------------*/
        private void HandlePinchZoom()
        {
            if (Touch.activeFingers.Count != 2)
                return;

            Touch touch0 = Touch.activeFingers[0].currentTouch;
            Touch touch1 = Touch.activeFingers[1].currentTouch;

            // Current and previous positions of both fingers
            Vector2 currentPos0 = touch0.screenPosition;
            Vector2 currentPos1 = touch1.screenPosition;
            Vector2 previousPos0 = currentPos0 - touch0.delta;
            Vector2 previousPos1 = currentPos1 - touch1.delta;

            float previousDistance = Vector2.Distance(previousPos0, previousPos1);
            float currentDistance = Vector2.Distance(currentPos0, currentPos1);
            float delta = previousDistance - currentDistance;

            if (Mathf.Approximately(delta, 0f))
                return;

            float newSize = m_camera.orthographicSize + delta * m_zoomSensitivity;
            m_camera.orthographicSize = Mathf.Clamp(newSize, m_zoomRange.x, m_zoomRange.y);
        }
    }
}