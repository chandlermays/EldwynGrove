using TMPro;
using UnityEngine;
using UnityEngine.UI;
//---------------------------------
using EldwynGrove.Saving;
using EldwynGrove.SceneManagement;

namespace EldwynGrove.UI
{
    public class LoadGameUI : MonoBehaviour
    {
        [SerializeField] private Transform m_contentRoot;
        [SerializeField] private GameObject m_buttonPrefab;
        [SerializeField] private Button m_playButton;
        [SerializeField] private Button m_deleteButton;

        private string m_selectedSaveFile;

        /*---------------------------------------------------------------------
        | --- OnEnable: Called when the object becomes enabled and active --- |
        ---------------------------------------------------------------------*/
        private void OnEnable()
        {
            RefreshSaveList();
            DisableButtons(false);
        }

        /*---------------------------------------------------------------------
        | --- SelectSaveFile: Handle selection of a save file from the UI --- |
        ---------------------------------------------------------------------*/
        public void SelectSaveFile(string saveFile)
        {
            m_selectedSaveFile = saveFile;
            DisableButtons(true);
        }

        /*------------------------------------------------------------------
        | --- PlaySelected: Start the game with the selected save file --- |
        ------------------------------------------------------------------*/
        public void PlaySelected()
        {
            if (string.IsNullOrEmpty(m_selectedSaveFile))
                return;

            SaveLoadController saveLoadController = FindFirstObjectByType<SaveLoadController>();
            if (saveLoadController == null)
                return;

            saveLoadController.LoadGame(m_selectedSaveFile);
        }

        /*---------------------------------------------------------------------
        | --- DeleteSelected: Delete the selected save and refresh the UI --- |
        ---------------------------------------------------------------------*/
        public void DeleteSelected()
        {
            if (string.IsNullOrEmpty(m_selectedSaveFile))
                return;

            SaveLoadController saveLoadController = FindFirstObjectByType<SaveLoadController>();
            if (saveLoadController == null)
                return;

            SavingSystem saveSystem = saveLoadController.GetComponent<SavingSystem>();
            if (saveSystem == null)
                return;

            saveSystem.Delete(m_selectedSaveFile);

            m_selectedSaveFile = null;
            RefreshSaveList();
            DisableButtons(false);
        }

        /*-------------------------------------------------------------------
        | --- RefreshSaveList: Rebuild the list of available save files --- |
        -------------------------------------------------------------------*/
        private void RefreshSaveList()
        {
            SaveLoadController saveLoadController = FindFirstObjectByType<SaveLoadController>();
            if (saveLoadController == null)
                return;

            foreach (Transform child in m_contentRoot)
            {
                Destroy(child.gameObject);
            }    

            foreach (string saveFile in saveLoadController.ListSaveFiles())
            {
                GameObject buttonInstances = Instantiate(m_buttonPrefab, m_contentRoot);
                TMP_Text buttonText = buttonInstances.GetComponentInChildren<TMP_Text>();
                buttonText.text = saveFile;

                Button button = buttonInstances.GetComponent<Button>();
                button.onClick.AddListener(() => SelectSaveFile(saveFile));
            }
        }

        /*-------------------------------------------------------------------------
        | --- DisableButtons: Toggle the interactable property of the buttons --- |
        -------------------------------------------------------------------------*/
        private void DisableButtons(bool flag)
        {
            if (m_playButton != null)
                m_playButton.interactable = flag;

            if (m_deleteButton != null)
                m_deleteButton.interactable = flag;
        }
    }
}