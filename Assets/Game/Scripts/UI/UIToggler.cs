using UnityEngine;

namespace EldwynGrove.UI
{
    public class UIToggler : MonoBehaviour
    {
        [SerializeField] private GameObject m_uiPrefab;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_uiPrefab, nameof(m_uiPrefab));
        }

        /*-----------------------------------------------------
        | --- Start: Called before the first frame update --- |
        -----------------------------------------------------*/
        private void Start()
        {
            m_uiPrefab.SetActive(false);
        }

        /*--------------------------------------------
        | --- ToggleUI: Toggle the UI visibility --- |
        --------------------------------------------*/
        public void ToggleUI()
        {
            m_uiPrefab.SetActive(!m_uiPrefab.activeSelf);
        }
    }
}