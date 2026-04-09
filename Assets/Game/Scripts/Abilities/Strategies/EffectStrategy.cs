/*-------------------------
File: EffectStrategy.cs
Author: Chandler Mays
-------------------------*/
using System;
using UnityEngine;
//---------------------------------

namespace EldwynGrove.Abilities
{
    public abstract class EffectStrategy : ScriptableObject
    {
        public abstract void StartEffect(AbilityConfig config, Action onComplete);
    }
}