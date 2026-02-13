using UnityEngine;

namespace EldwynGrove
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform m_target;
        [SerializeField] private BoxCollider2D m_cameraBounds;

        private Camera m_camera;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            m_camera = Camera.main;
            Utilities.CheckForNull(m_camera, nameof(m_camera));

            Utilities.CheckForNull(m_target, nameof(m_target));
            Utilities.CheckForNull(m_cameraBounds, nameof(m_cameraBounds));
        }

        /*------------------------------------------------------------------------
        | --- LateUpdate: Called after all Update functions have been called --- |
        ------------------------------------------------------------------------*/
        private void LateUpdate()
        {
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
    }
}