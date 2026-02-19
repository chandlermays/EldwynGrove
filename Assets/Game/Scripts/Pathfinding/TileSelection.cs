using UnityEngine;
using UnityEngine.Tilemaps;

using EldwynGrove.Input;

namespace EldwynGrove.Navigation
{
    public class TileSelection : MonoBehaviour
    {
        [SerializeField] private Tilemap m_groundTilemap;
        [SerializeField] private float m_offset = 0.5f;
        [SerializeField] private Vector2 m_gridSize = new Vector2(1f, 1f);

        private Vector2Int m_highlightedTilePosition = Vector2Int.zero;
        public Vector2Int HighlightedTilePosition => m_highlightedTilePosition;

        private EGInputActions m_inputActions;

        private void Awake()
        {
            m_inputActions = new EGInputActions();
        }

        private void OnEnable()
        {
            m_inputActions.Enable();
        }

        private void OnDisable()
        {
            m_inputActions.Disable();
        }

        private void Update()
        {
            Vector2 mouseScreenPos = m_inputActions.Gameplay.Point.ReadValue<Vector2>();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(mouseWorldPos.x / m_gridSize.x) * Mathf.RoundToInt(m_gridSize.x),
                Mathf.FloorToInt(mouseWorldPos.y / m_gridSize.y) * Mathf.RoundToInt(m_gridSize.y));

            m_highlightedTilePosition = gridPos;
            Vector2 worldPos = GridUtils.GridToWorld(gridPos) + new Vector2(m_offset, m_offset);
            transform.position = worldPos;
        }

        public bool IsHighlightedTileClicked(Vector2 clickedPosition)
        {
            Vector2Int gridPos = GridUtils.WorldToGrid(clickedPosition);
            return gridPos == m_highlightedTilePosition;
        }

        public Vector2 GetHighlightedTilePosition()
        {
            return GridUtils.GridToWorld(m_highlightedTilePosition);
        }
    }
}