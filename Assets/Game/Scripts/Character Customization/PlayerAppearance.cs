using UnityEngine;
using Newtonsoft.Json.Linq;
//---------------------------------
using EldwynGrove.Saving;

namespace EldwynGrove.Player
{
    public class PlayerAppearance : MonoBehaviour, ISaveable
    {
        [System.Serializable]
        public class StyleOptions
        {
            public Sprite[] sprites;
        }

        [SerializeField] private SpriteRenderer m_renderer;
        [SerializeField] private StyleOptions[] m_styles;

        private int m_styleIndex;
        private int m_colorIndex;

        private void Awake()
        {
     //       Utilities.CheckForNull(m_renderer, nameof(m_renderer));
        }

        private void ApplyAppearance()
        {
            if (m_styleIndex < m_styles.Length &&
                m_colorIndex < m_styles[m_styleIndex].sprites.Length)
            {
                m_renderer.sprite = m_styles[m_styleIndex].sprites[m_colorIndex];
            }
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
            ApplyAppearance();
        }
    }
}