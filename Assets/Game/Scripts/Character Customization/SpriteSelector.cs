using Newtonsoft.Json.Linq;
using UnityEngine;
//---------------------------------
using EldwynGrove.Saving;

namespace EldwynGrove.UI
{
    public class SpriteSelector : MonoBehaviour, ISaveable
    {
        [System.Serializable]
        public class StyleOptions
        {
            public Sprite[] sprites;
        }

        [SerializeField] private SpriteRenderer m_bodyPart;
        [SerializeField] private StyleOptions[] m_styles;

        private int m_styleIndex;
        private int m_colorIndex;

        public void NextStyle()
        {
            m_styleIndex = (m_styleIndex + 1) % m_styles.Length;
            m_colorIndex = Mathf.Clamp(m_colorIndex, 0, m_styles[m_styleIndex].sprites.Length - 1);
            UpdateSprite();
        }

        public void PreviousStyle()
        {
            m_styleIndex = (m_styleIndex - 1 + m_styles.Length) % m_styles.Length;
            m_colorIndex = Mathf.Clamp(m_colorIndex, 0, m_styles[m_styleIndex].sprites.Length - 1);
            UpdateSprite();
        }

        public void NextColor()
        {
            Sprite[] colors = m_styles[m_styleIndex].sprites;
            m_colorIndex = (m_colorIndex + 1) % colors.Length;
            UpdateSprite();
        }

        public void PreviousColor()
        {
            Sprite[] colors = m_styles[m_styleIndex].sprites;
            m_colorIndex = (m_colorIndex - 1 + colors.Length) % colors.Length;
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            m_bodyPart.sprite = m_styles[m_styleIndex].sprites[m_colorIndex];
        }

        public JToken CaptureState()
        {
            return new JObject
            {
                ["styleIndex"] = m_styleIndex,
                ["colorIndex"] = m_colorIndex
            };
        }

        public void RestoreState(JToken state)
        {
            m_styleIndex = state["styleIndex"].ToObject<int>();
            m_colorIndex = state["colorIndex"].ToObject<int>();
            UpdateSprite();
        }
    }
}