using UnityEngine;
//---------------------------------
using EldwynGrove.Core;
using EldwynGrove.Inventories;
using EldwynGrove.Navigation;

namespace EldwynGrove.Player
{
    public class PlayerInteractionHandler : MonoBehaviour
    {
        private TileCursor m_tileCursor;
        private MovementComponent m_movementComponent;
        private GatheringComponent m_gatheringComponent;

        /*---------------------------------------------------------------
        | --- Initialize: Sets up references to required components --- |
        ---------------------------------------------------------------*/
        public void Initialize(TileCursor cursor,MovementComponent movement, GatheringComponent gathering)
        {
            m_tileCursor = cursor;
            m_movementComponent = movement;
            m_gatheringComponent = gathering;
        }

        /*-----------------------------------------------------------------------
        | --- HandleInteractAt: Called when player taps/clicks on the world --- |
        -----------------------------------------------------------------------*/
        public void HandleInteractAt(Vector3 worldPos)
        {
            Vector2Int tappedCoords = ForageManager.Instance.GetCoordsFromWorld(worldPos);

            if (ForageManager.Instance.HasForageAt(tappedCoords))
            {
                Debug.Log($"[PlayerInteractionHandler] Handling forage interaction at world position {worldPos}.");
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
            Debug.Log($"[PlayerInteractionHandler] Handling movement interaction to world position {worldPos}.");

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
                Debug.LogWarning($"[PlayerInteractionHandler] No walkable tile adjacent to forage at {forageCoords}.");
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
                Debug.LogWarning("[PlayerInteractionHandler] Reached forage tile but item was already removed.");
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