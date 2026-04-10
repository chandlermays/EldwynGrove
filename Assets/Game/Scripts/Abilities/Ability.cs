/*-------------------------
File: Ability.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------
using EldwynGrove.Inventories;
using EldwynGrove.Core;

namespace EldwynGrove.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Eldwyn Grove/Abilities/New Ability")]
    public class Ability : ActionItem
    {
        [Header("Ability Config")]
        [SerializeField] private float m_cooldownTime = 5f;
        [SerializeField] private TargetingStrategy m_targetingStrategy;
        [SerializeField] private EffectStrategy[] m_effects;
        [SerializeField] private FilteringStrategy[] m_filters;

        /*-------------------------------------------------------------- 
        | --- Use: Initiates the targeting process for the ability --- |
        --------------------------------------------------------------*/
        public override bool Use(GameObject user)
        {
            Cooldowns cooldowns = user.GetComponent<Cooldowns>();
            Utilities.CheckForNull(cooldowns, nameof(cooldowns));

            if (cooldowns.GetRemainingCooldown(this) > 0f)
                return false;

            AbilityConfig config = new(user);

            ActionManager actionManager = user.GetComponent<ActionManager>();
            Utilities.CheckForNull(actionManager, nameof(actionManager));

            actionManager.StartAction(config);

            m_targetingStrategy.StartTargeting(config, () => TargetAcquired(config));
            return true;
        }

        /*------------------------------------------------------------------------------------------------ 
        | --- TargetAcquired: Applies filters to the acquired targets and starts the effects on them --- |
        ------------------------------------------------------------------------------------------------*/
        private void TargetAcquired(AbilityConfig config)
        {
            if (config.IsCancelled)
                return;

            Cooldowns cooldowns = config.User.GetComponent<Cooldowns>();
            cooldowns.StartCooldown(this, m_cooldownTime);

            foreach (FilteringStrategy filterStrategy in m_filters)
            {
                config.Targets = filterStrategy.Filter(config.Targets);
            }

            foreach (EffectStrategy effect in m_effects)
            {
                effect.StartEffect(config, EffectCompleted);
            }
        }

        /*------------------------------------------------------------------- 
        | --- EffectCompleted: Callback for when an effect is completed --- |
        -------------------------------------------------------------------*/
        private void EffectCompleted()
        {
            //...
        }
    }
}