/*-------------------------
File: IStatModifier.cs
Author: Chandler Mays
-------------------------*/
using System.Collections.Generic;
//---------------------------------

namespace EldwynGrove.Attributes
{
    public interface IStatModifier
    {
        IEnumerable<float> GetAdditiveModifiers(Stat stat);
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}