/*-------------------------
File: FactionComponent.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Components
{
    public enum Faction
    {
        kPlayer,
        kNPC,
        kEnemy,
        kNeutral
    }

    public class FactionComponent : MonoBehaviour
    {
        [SerializeField] private Faction m_faction = Faction.kNeutral;

        public Faction Faction => m_faction;

        /*-----------------------------------------------------------------------
        | --- IsHostileTo: Check if this faction is hostile to another faction |
        -----------------------------------------------------------------------*/
        public bool IsHostileTo(FactionComponent other)
        {
            if (other == null)
                return false;

            return IsHostileTo(other.Faction);
        }

        /*-----------------------------------------------------------------------
        | --- IsHostileTo: Check if this faction is hostile to another faction |
        -----------------------------------------------------------------------*/
        public bool IsHostileTo(Faction otherFaction)
        {
            // Same faction is never hostile (no friendly fire)
            if (m_faction == otherFaction)
                return false;

            // Player and NPCs are allies; never hostile to each other
            if ((m_faction == Faction.kPlayer && otherFaction == Faction.kNPC) ||
                (m_faction == Faction.kNPC && otherFaction == Faction.kPlayer))
                return false;

            // Hostile pairs:
            // - Player vs Enemy
            // - Enemy vs Player
            // - NPC vs Enemy
            // - Enemy vs NPC
            if ((m_faction == Faction.kPlayer && otherFaction == Faction.kEnemy) ||
                (m_faction == Faction.kEnemy && otherFaction == Faction.kPlayer) ||
                (m_faction == Faction.kNPC && otherFaction == Faction.kEnemy) ||
                (m_faction == Faction.kEnemy && otherFaction == Faction.kNPC))
                return true;

            // Neutral is not hostile to anyone by default
            return false;
        }
    }
}