/*-------------------------
File: PlayerController.cs
Author: Chandler Mays
-------------------------*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
//---------------------------------
using EldwynGrove.Components;
using EldwynGrove.Input;
using EldwynGrove.Inventories;
using EldwynGrove.Navigation;

namespace EldwynGrove.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TileCursor m_tileCursor;

        private Camera m_mainCamera;
        private EGInputActions m_inputActions;
        private MovementComponent m_movementComponent;
        private GatheringComponent m_gatheringComponent;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_tileCursor, nameof(m_tileCursor));
            m_mainCamera = Camera.main;

            m_movementComponent = GetComponent<MovementComponent>();
            Utilities.CheckForNull(m_movementComponent, nameof(m_movementComponent));

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

        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            if (Touch.activeFingers.Count >= 2)
                return;

            Vector2 screenPos = m_inputActions.Gameplay.TouchPosition.ReadValue<Vector2>();

            if (IsTouchOverUI(screenPos))
                return;

            Vector3 worldPos = m_mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            HandleInteractAt(worldPos);
        }

        private void OnTouchReleased(InputAction.CallbackContext context)
        {
            //...
        }

        /*-----------------------------------------------------------------------
        | --- HandleInteractAt: Called when player taps/clicks on the world --- |
        -----------------------------------------------------------------------*/
        private void HandleInteractAt(Vector3 worldPos)
        {
            Vector2Int tappedCoords = ForageManager.Instance.GetCoordsFromWorld(worldPos);

            if (ForageManager.Instance.HasForageAt(tappedCoords))
            {
                TryMoveToForage(tappedCoords);
                return;
            }

            TryMoveTo(worldPos);
        }

        /*------------------------------------------------------------------------------------------
        | --- TryMoveTo: Attempts to move player to world position, showing destination cursor --- |
        ------------------------------------------------------------------------------------------*/
        private void TryMoveTo(Vector3 worldPos)
        {
            bool canMove = m_movementComponent.MoveTo(worldPos, OnReachedDestination);
            m_tileCursor.ShowAtWorldPosition(worldPos, canMove);
        }

        /*-------------------------------------------------------------------------------
        | --- TryMoveToForage: Finds adjacent tile to forage and moves player there --- |
        -------------------------------------------------------------------------------*/
        private void TryMoveToForage(Vector2Int forageCoords)
        {
            // If the player is already on an adjacent tile, harvest in place
            Vector2Int playerCoords = ForageManager.Instance.GetCoordsFromWorld(transform.position);
            Vector2Int[] offsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int offset in offsets)
            {
                if (playerCoords == forageCoords + offset)
                {
                    m_tileCursor.Hide();
                    OnReachedForage(forageCoords);
                    return;
                }
            }

            // Otherwise, find the best adjacent tile and move there
            Vector2Int? adjacentCoord = GetBestAdjacentCoord(forageCoords);

            if (!adjacentCoord.HasValue)
            {
                Debug.LogWarning($"[PlayerController] No walkable tile adjacent to forage at {forageCoords}.");
                return;
            }

            Vector3 adjacentWorld = ForageManager.Instance.GetWorldCenter(adjacentCoord.Value);
            Vector3 forageWorld = ForageManager.Instance.GetWorldCenter(forageCoords);

            bool canMove = m_movementComponent.MoveTo(adjacentWorld, () => OnReachedForage(forageCoords));
            m_tileCursor.ShowAtWorldPosition(forageWorld, canMove);
        }

        /*------------------------------------------------------------------------------------------------
        | --- OnReachedForage: Called when player reaches forage tile, gathers item and hides cursor --- |
        ------------------------------------------------------------------------------------------------*/
        private void OnReachedForage(Vector2Int forageCoords)
        {
            m_tileCursor.Hide();

            Vector3 forageWorld = ForageManager.Instance.GetWorldCenter(forageCoords);
            Vector2 directionToForage = (forageWorld - transform.position).normalized;
            m_movementComponent.SetDirection(directionToForage);

            ForageItem item = ForageManager.Instance.RemoveForage(forageCoords);

            if (item == null)
            {
                Debug.LogWarning("[PlayerController] Reached forage tile but item was already removed.");
                return;
            }

            m_gatheringComponent.Gather(item);
        }

        /*------------------------------------------------------------------------------------------------------
        | --- OnReachedDestination: Called when player reaches non-forage destination, simply hides cursor --- |
        ------------------------------------------------------------------------------------------------------*/
        private void OnReachedDestination()
        {
            m_tileCursor.Hide();
        }

        /*-----------------------------------------------------------------------------------
        | --- GetBestAdjacentCoord: Finds the best adjacent walkable tile to the forage --- |
        -----------------------------------------------------------------------------------*/
        private Vector2Int? GetBestAdjacentCoord(Vector2Int forageCoord)
        {
            Vector2Int[] offsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            Vector2Int? best = null;
            float bestDist = float.MaxValue;

            foreach (Vector2Int offset in offsets)
            {
                Vector2Int candidate = forageCoord + offset;
                Node node = GridManager.Instance.GetNode(candidate);

                if (node == null || !node.Walkable)
                    continue;

                float dist = Vector3.Distance(transform.position, node.WorldPosition);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = candidate;
                }
            }

            return best;
        }
    }
}