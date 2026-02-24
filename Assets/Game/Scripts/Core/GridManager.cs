using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        [Header("Tilemap")]
        [SerializeField] private Tilemap m_groundTilemap;

        [Header("Pathfinding")]
        [SerializeField] private LayerMask m_obstacleLayer;
        private float m_nodeRadius = 0.5f;

        private Dictionary<Vector2Int, Node> m_grid = new();
        public void RebuildGrid() => BuildGrid();

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

        private void BuildGrid()
        {
            m_grid.Clear();
            BoundsInt bounds = m_groundTilemap.cellBounds;

            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                if (!m_groundTilemap.HasTile(cellPos))
                    continue;

                Vector3 worldPos = m_groundTilemap.GetCellCenterWorld(cellPos);
                bool walkable = !Physics2D.OverlapCircle(worldPos, m_nodeRadius - 0.05f, m_obstacleLayer);
                Vector2Int coord = new Vector2Int(cellPos.x, cellPos.y);
                m_grid[coord] = new Node(coord, worldPos, walkable);
            }
        }

        public Node GetNode(Vector2Int coord)
        {
            return m_grid.TryGetValue(coord, out Node node) ? node : null;
        }

        public Node GetNodeFromWorld(Vector3 worldPos)
        {
            Vector3Int cellPos = m_groundTilemap.WorldToCell(worldPos);
            return GetNode(new Vector2Int(cellPos.x, cellPos.y));
        }

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