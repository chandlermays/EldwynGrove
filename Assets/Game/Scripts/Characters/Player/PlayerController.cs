using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
//---------------------------------
using EldwynGrove.Combat;
using EldwynGrove.Core;
using EldwynGrove.Input;
using EldwynGrove.Inventories;
using EldwynGrove.Navigation;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Camera m_mainCamera;
        private MovementComponent m_movementComponent;
        private HealthComponent m_healthComponent;
        private GatheringComponent m_gatheringComponent;
        private EGInputActions m_inputActions;

        private bool m_isTouching;

        private Vector2Int m_pendingForageCoords;

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

            m_gatheringComponent = GetComponent<GatheringComponent>();
            Utilities.CheckForNull(m_gatheringComponent, nameof(m_gatheringComponent));
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

            if (IsTouchOverUI(screenPos)) return;

            Vector3 worldPos = m_mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            HandleMoveTo(worldPos);
        }

        /*-----------------------------------------------------------------
        | --- IsTouchOverUI: Checks if the touch is over a UI element --- |
        -----------------------------------------------------------------*/
        private bool IsTouchOverUI(Vector2 screenPos)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = screenPos };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            return results.Count > 0;
        }

        /*-------------------------------------------------------------------------------
        | --- HandleMoveTo: Handles moving the player to the touched world position --- |
        -------------------------------------------------------------------------------*/
        private void HandleMoveTo(Vector3 worldPos)
        {
            Vector2Int tappedCoords = ForageManager.Instance.GetCoordsFromWorld(worldPos);

            if (ForageManager.Instance.HasForageAt(tappedCoords))
            {
                Vector2Int? adjacentCoord = GetBestAdjacentCoord(tappedCoords);

                if (adjacentCoord.HasValue)
                {
                    m_pendingForageCoords = tappedCoords;
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

        /*-----------------------------------------------------------------------------------------------
        | --- GetBestAdjacentCoord: Finds the best adjacent walkable tile to the forage coordinates --- |
        -----------------------------------------------------------------------------------------------*/
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

        /*---------------------------------------------------------------------------------------------
        | --- OnReachedForageTile: Called when the player reaches the tile adjacent to the forage --- |
        ---------------------------------------------------------------------------------------------*/
        private void OnReachedForageTile()
        {
            ForageItem item = ForageManager.Instance.GetForageAt(m_pendingForageCoords);

            if (item == null)
            {
                Debug.LogWarning("[PlayerController] Reached forage tile, but no forage item found.");
                return;
            }

            m_gatheringComponent.Gather(item);
        }

        /*-------------------------------------------------------------------------------
        | --- OnTouchStarted: 
        -------------------------------------------------------------------------------*/
        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            m_isTouching = true;
        }

        /*-------------------------------------------------------------------------------
        | --- OnTouchReleased: 
        -------------------------------------------------------------------------------*/
        private void OnTouchReleased(InputAction.CallbackContext context)
        {
            m_isTouching = false;
        }
    }
}