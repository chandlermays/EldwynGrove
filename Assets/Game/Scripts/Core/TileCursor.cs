/*-------------------------
File: TileCursor.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;

namespace EldwynGrove.Player
{
    public class TileCursor : MonoBehaviour
    {
        [SerializeField] private Grid m_grid;
        [SerializeField] private Color m_validPathColor;
        [SerializeField] private Color m_invalidPathColor;

        private SpriteRenderer m_spriteRenderer;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_grid, nameof(m_grid));

            m_spriteRenderer = GetComponent<SpriteRenderer>();
            Utilities.CheckForNull(m_spriteRenderer, nameof(m_spriteRenderer));

            m_spriteRenderer.enabled = false;
        }

        /*--------------------------------------------------------------------------------------------------------------
        | --- ShowAtWorldPosition: Displays the cursor at a specific world position with color indicating validity --- |
        --------------------------------------------------------------------------------------------------------------*/
        public void ShowAtWorldPosition(Vector3 worldPos, bool isValid)
        {
            worldPos.z = 0f;

            Vector3Int cellPos = m_grid.WorldToCell(worldPos);
            transform.position = m_grid.GetCellCenterWorld(cellPos);

            m_spriteRenderer.color = isValid ? m_validPathColor : m_invalidPathColor;
            m_spriteRenderer.enabled = true;
        }

        /*------------------------------------------
        | --- Hide: Hides the cursor from view --- |
        ------------------------------------------*/
        public void Hide()
        {
            m_spriteRenderer.enabled = false;
        }
    }
}