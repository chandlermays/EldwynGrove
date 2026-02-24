using UnityEngine;
using UnityEngine.InputSystem;
//---------------------------------
using EldwynGrove.Combat;
using EldwynGrove.Core;
using EldwynGrove.Input;
using EldwynGrove.Inventory;
using EldwynGrove.Navigation;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Camera m_mainCamera;
        private MovementComponent m_movementComponent;
        private HealthComponent m_healthComponent;
        private EGInputActions m_inputActions;

        private bool m_isTouching;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            m_mainCamera = Camera.main;
            Utilities.CheckForNull(m_mainCamera, nameof(m_mainCamera));

            m_movementComponent = GetComponent<MovementComponent>();
            Utilities.CheckForNull(m_movementComponent, nameof(m_movementComponent));

            m_healthComponent = GetComponent<HealthComponent>();
            Utilities.CheckForNull(m_healthComponent, nameof(m_healthComponent));
        }

        /*-----------------------------------------------------
        | --- Start: Called before the first frame update --- |
        -----------------------------------------------------*/
        private void Start()
        {
            m_inputActions = InputManager.Instance.InputActions;
            m_inputActions.Gameplay.TouchPress.performed += OnTouchStarted;
            m_inputActions.Gameplay.TouchPress.canceled += OnTouchReleased;
        }

        /*--------------------------------------------------------------------
        | --- OnDestroy: Called when the MonoBehaviour will be destroyed --- |
        --------------------------------------------------------------------*/
        private void OnDestroy()
        {
            m_inputActions.Gameplay.TouchPress.performed -= OnTouchStarted;
            m_inputActions.Gameplay.TouchPress.canceled -= OnTouchReleased;
        }

        /*-----------------------------------------
        | --- Update: Called upon every frame --- |
        -----------------------------------------*/
        private void Update()
        {
            if (!m_isTouching) return;

            Vector2 screenPos = m_inputActions.Gameplay.TouchPosition.ReadValue<Vector2>();
            Vector3 worldPos = m_mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            HandleMoveTo(worldPos);
        }

        private void HandleMoveTo(Vector3 worldPos)
        {
            Vector2Int tappedCoords = ForageManager.Instance.GetCoordsFromWorld(worldPos);

            if (ForageManager.Instance.HasForageAt(tappedCoords))
            {
                Vector2Int? adjacentCoord = GetBestAdjacentCoord(tappedCoords);

                if (adjacentCoord.HasValue)
                {
                    Vector3 adjacentWorld = ForageManager.Instance.GetWorldCenter(adjacentCoord.Value);
                    m_movementComponent.MoveTo(adjacentWorld, OnReachedForageTile);
                }
                else
                {
                    Debug.LogWarning($"[PlayerController] No walkable tile adjacent to forage at {tappedCoords}.");
                }

                return;
            }

            m_movementComponent.MoveTo(worldPos);
        }

        private Vector2Int? GetBestAdjacentCoord(Vector2Int forageCoord)
        {
            Vector2Int[] cardinalOffsets =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            Vector2Int? best = null;
            float bestDist = float.MaxValue;

            foreach (Vector2Int offset in cardinalOffsets)
            {
                Vector2Int candidateCoord = forageCoord + offset;
                Node node = GridManager.Instance.GetNode(candidateCoord);

                if (node == null || !node.Walkable)
                    continue;

                float dist = Vector3.Distance(transform.position, node.WorldPosition);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = candidateCoord;
                }
            }

            return best;
        }

        private void OnReachedForageTile()
        {
            Debug.Log("[PlayerController] Perform harvest action.");
        }

        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            m_isTouching = true;
        }

        private void OnTouchReleased(InputAction.CallbackContext context)
        {
            m_isTouching = false;
        }
    }
}