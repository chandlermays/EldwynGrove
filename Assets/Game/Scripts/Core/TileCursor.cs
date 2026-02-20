using EldwynGrove.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EldwynGrove.Player
{
    public class TileCursor : MonoBehaviour
    {
        [SerializeField] private Grid m_grid;

        private EGInputActions m_inputActions;
        private SpriteRenderer m_spriteRenderer;
        private Camera m_mainCamera;

        private Vector2 m_position;

        private void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            Utilities.CheckForNull(m_spriteRenderer, nameof(m_spriteRenderer));

            Utilities.CheckForNull(m_grid, nameof(m_grid));

            m_mainCamera = Camera.main;
        }

        private void Start()
        {
            m_inputActions = InputManager.Instance.InputActions;

            m_inputActions.Gameplay.TouchPosition.performed += OnTouchPosition;
        }

        private void OnDestroy()
        {
            m_inputActions.Gameplay.TouchPosition.performed -= OnTouchPosition;
        }

        private void Update()
        {
            SnapToMousePosition();
        }

        private void SnapToMousePosition()
        {
            Vector3 worldPos = m_mainCamera.ScreenToWorldPoint(m_position);
            worldPos.z = 0f;

            Vector3Int cellPos = m_grid.WorldToCell(worldPos);
            transform.position = m_grid.GetCellCenterWorld(cellPos);
        }

        private void OnTouchPosition(InputAction.CallbackContext context)
        {
            m_position = context.ReadValue<Vector2>();
        }
    }
}