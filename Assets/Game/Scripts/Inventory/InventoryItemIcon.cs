using TMPro;
using UnityEngine;
using UnityEngine.UI;
//---------------------------------

namespace EldwynGrove.Inventories
{
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] private GameObject m_textContainer;
        [SerializeField] private TextMeshProUGUI m_quantityText;

        private Image m_iconImage;

        /*---------------------------------------------------
        | --- SetItem: Set the icon of an InventoryItem --- |
        ---------------------------------------------------*/
        public void SetItem(InventoryItem item)
        {
            SetItem(item, 0);
        }

        /*--------------------------------------------------------------------------------
        | --- SetItem: Set the icon and quantity of an InventoryItem, if more than 1 --- |
        --------------------------------------------------------------------------------*/
        public void SetItem(InventoryItem item, int quantity)
        {
            m_iconImage = GetComponent<Image>();

            if (item == null)
            {
                m_iconImage.enabled = false;
            }
            else
            {
                // Icon image is null here. It shouldn't be.
                m_iconImage.enabled = true;
                m_iconImage.sprite = item.Icon;
            }

            if (m_quantityText)
            {
                if (quantity <= 1)
                {
                    m_textContainer.SetActive(false);
                }
                else
                {
                    m_textContainer.SetActive(true);
                    m_quantityText.text = quantity.ToString();
                }
            }
        }
    }
}