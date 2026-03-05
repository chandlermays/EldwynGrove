using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//---------------------------------

namespace EldwynGrove.Core
{
    public class Node
    {
        public Vector2Int GridCoord { get; }
        public Vector3 WorldPosition { get; }
        public bool Walkable { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public Node Parent { get; set; }

        public Node(Vector2Int gridCoord, Vector3 worldPosition, bool walkable)
        {
            GridCoord = gridCoord;
            WorldPosition = worldPosition;
            Walkable = walkable;
        }
    }

    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Tilemaps")]
        [SerializeField] private Tilemap[] m_walkableTilemaps;

        [Header("Pathfinding")]
        [SerializeField] private LayerMask m_obstacleLayer;
        private float m_nodeRadius = 0.5f;

        private Dictionary<Vector2Int, Node> m_grid = new();
        public void RebuildGrid() => BuildGrid();

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            BuildGrid();
        }

        /*---------------------------------------------------------------------------
        | --- BuildGrid: Constructs the grid based on the tilemap and obstacles --- |
        ---------------------------------------------------------------------------*/
        private void BuildGrid()
        {
            m_grid.Clear();

            foreach (Tilemap tilemap in m_walkableTilemaps)
            {
                if (tilemap == null) continue;

                BoundsInt bounds = tilemap.cellBounds;

                foreach (Vector3Int cellPos in bounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(cellPos))
                        continue;

                    Vector2Int coord = new Vector2Int(cellPos.x, cellPos.y);

                    // Skip if already added by a previously processed tilemap
                    if (m_grid.ContainsKey(coord))
                        continue;

                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                    bool walkable = !Physics2D.OverlapCircle(worldPos, m_nodeRadius - 0.05f, m_obstacleLayer);
                    m_grid[coord] = new Node(coord, worldPos, walkable);
                }
            }
        }

        /*----------------------------------------------------------------------
        | --- GetNode: Retrieves the node at the specified grid coordinate --- |
        ----------------------------------------------------------------------*/
        public Node GetNode(Vector2Int coord)
        {
            return m_grid.TryGetValue(coord, out Node node) ? node : null;
        }

        /*--------------------------------------------------------------------------------
        | --- GetNodeFromWorld: Retrieves the node corresponding to a world position --- |
        --------------------------------------------------------------------------------*/
        public Node GetNodeFromWorld(Vector3 worldPos)
        {
            foreach (Tilemap tilemap in m_walkableTilemaps)
            {
                if (tilemap == null) continue;

                Vector3Int cellPos = tilemap.WorldToCell(worldPos);
                Node node = GetNode(new Vector2Int(cellPos.x, cellPos.y));

                if (node != null) return node;
            }
            return null;
        }

        /*-----------------------------------------------------------------------------
        | --- GetNeighbors: Retrieves walkable neighboring nodes for a given node --- |
        -----------------------------------------------------------------------------*/
        public List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int direction in directions)
            {
                Node neighbor = GetNode(node.GridCoord + direction);

                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
            return neighbors;
        }
    }
}