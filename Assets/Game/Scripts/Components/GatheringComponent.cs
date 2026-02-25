using UnityEngine;
//---------------------------------
using EldwynGrove.Inventories;

namespace EldwynGrove
{
    public class GatheringComponent : EntityComponent
    {
        private static readonly int s_animChop = Animator.StringToHash("Chop");
        private static readonly int s_animMine = Animator.StringToHash("Mine");
        private static readonly int s_animReap = Animator.StringToHash("Reap");

        protected override void Awake()
        {
            base.Awake();
        }

        public void Gather(ForageItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("[GatheringComponent] Attempted to gather with a null item.");
                return;
            }

            int trigger = item.GatherType switch
            {
                GatherType.kChop => s_animChop,
                GatherType.kMine => s_animMine,
                GatherType.kReap => s_animReap,
                _ => s_animReap
            };

            Animator.SetTrigger(trigger);
        }
    }
}