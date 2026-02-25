using UnityEngine;

namespace EldwynGrove.Player
{
    public class TileCursor : MonoBehaviour
    {
        [SerializeField] private Grid m_grid;
        [SerializeField] private Color m_validPathCoor;
        [SerializeField] private Color m_invalidPathColor;

        private SpriteRenderer m_spriteRenderer;

        private void Awake()
        {
            Utilities.CheckForNull(m_grid, nameof(m_grid));

            m_spriteRenderer = GetComponent<SpriteRenderer>();
            Utilities.CheckForNull(m_spriteRenderer, nameof(m_spriteRenderer));

            m_spriteRenderer.enabled = false;
        }

        public void ShowAtWorldPosition(Vector3 worldPos, bool isValid)
        {
            worldPos.z = 0f;

            Vector3Int cellPos = m_grid.WorldToCell(worldPos);
            transform.position = m_grid.GetCellCenterWorld(cellPos);

            m_spriteRenderer.color = isValid ? m_validPathCoor : m_invalidPathColor;
            m_spriteRenderer.enabled = true;
        }

        public void Hide()
        {
            m_spriteRenderer.enabled = false;
        }
    }
}