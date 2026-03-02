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
            Vector2Int? adjacentCoord = GetBestAdjacentCoord(forageCoords);

            if (!adjacentCoord.HasValue)
            {
                Debug.LogWarning($"[PlayerInteractionHandler] No walkable tile adjacent to forage at {forageCoords}.");
                return;
            }

            Vector3 adjacentWorld = ForageManager.Instance.GetWorldCenter(adjacentCoord.Value);
            Vector3 forageWorld = ForageManager.Instance.GetWorldCenter(forageCoords);

            // Capture forageCoords in the closure — no more field dependency
            bool canMove = m_movementComponent.MoveTo(adjacentWorld, () => OnReachedForage(forageCoords));
            m_tileCursor.ShowAtWorldPosition(forageWorld, canMove);
        }

        /*------------------------------------------------------------------------------------------------
        | --- OnReachedForage: Called when player reaches forage tile, gathers item and hides cursor --- |
        ------------------------------------------------------------------------------------------------*/
        private void OnReachedForage(Vector2Int forageCoords)
        {
            m_tileCursor.Hide();
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

                if (node == null || !node.Walkable) continue;

                float dist = Vector3.Distance(transform.position, node.WorldPosition);
                if (dist < bestDist) { bestDist = dist; best = candidate; }
            }

            return best;
        }
    }
}