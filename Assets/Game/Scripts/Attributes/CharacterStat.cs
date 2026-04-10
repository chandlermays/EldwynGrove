/*-------------------------
File: CharacterStat.cs
Author: Chandler Mays
-------------------------*/
namespace EldwynGrove.Attributes
{
    public enum Stat
    {
        kHealth,
        kDamage,
        kDefense,
        kExperienceReward,
        kExperienceToLevelUp,
        kTotalAttributePoints,
        //...
    }

    public enum Attribute
    {
        kStrength,          // Increase melee damage
        kDexterity,         // Increase ranged damage
        kVitality,          // Increase maximum health
        //...
    }
}