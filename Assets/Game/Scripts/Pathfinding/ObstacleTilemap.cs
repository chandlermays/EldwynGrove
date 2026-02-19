using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EldwynGrove.Navigation
{
    public class ObstacleTilemap : MonoBehaviour
    {
        [SerializeField] private List<Tilemap> m_obstacleTilemaps = new();

        private HashSet<Vector3Int> m_obstacleTilePositions = new();

        public bool IsTileObstacle(Vector2Int position) => IsTileObstacle((Vector2)position);

        private void Awake()
        {
            InitializeObstacleTiles();
        }

        private void InitializeObstacleTiles()
        {
            m_obstacleTilePositions.Clear();

            foreach (Tilemap tilemap in m_obstacleTilemaps)
            {
                BoundsInt bounds = tilemap.cellBounds;
                TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

                for (int x = 0; x < bounds.size.x; x++)
                {
                    for (int y = 0; y < bounds.size.y; y++)
                    {
                        TileBase tile = allTiles[x + y * bounds.size.x];
                        if (tile != null)
                        {
                            m_obstacleTilePositions.Add(new Vector3Int(bounds.x + x, bounds.y + y, 0));
                        }
                    }
                }
            }
        }

        public bool IsTileObstacle(Vector2 position)
        {
            if (m_obstacleTilemaps.Count == 0) return false;

            Vector3Int gridPos = m_obstacleTilemaps[0].WorldToCell(position);
            return m_obstacleTilePositions.Contains(gridPos);
        }

    }
}