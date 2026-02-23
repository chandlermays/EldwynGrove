using Newtonsoft.Json.Linq;
using UnityEngine;
//---------------------------------
using EldwynGrove.Saving;

namespace EldwynGrove.Combat
{
    public class HealthComponent : EntityComponent, ISaveable
    {
        [System.Serializable]
        private struct HealthSaveData
        {
            public float health;
            public bool isDead;
        }

        [Header("Health Settings")]
        [SerializeField] private float m_health;

        private bool m_isDead = false;

        public bool IsDead => m_isDead;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        protected override void Awake()
        {
            base.Awake();
        }

        /*-------------------------------------------------------------------------
        | --- CaptureState: Captures the current state of the Entity's Health --- |
        -------------------------------------------------------------------------*/
        public JToken CaptureState()
        {
            HealthSaveData data = new()
            {
                health = m_health,
                isDead = m_isDead
            };
            return JToken.FromObject(data);
        }

        /*--------------------------------------------------------------------------
        | --- RestoreState: Restores the Entity's Health state from saved data --- |
        --------------------------------------------------------------------------*/
        public void RestoreState(JToken state)
        {
            HealthSaveData data = state.ToObject<HealthSaveData>();
            m_health = data.health;
            m_isDead = data.isDead;
        }
    }
}