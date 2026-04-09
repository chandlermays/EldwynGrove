/*-------------------------
File: ISaveable.cs
Author: Chandler Mays
-------------------------*/
using Newtonsoft.Json.Linq;
//---------------------------------

namespace EldwynGrove.Saving
{
    public interface ISaveable
    {
        JToken CaptureState();
        void RestoreState(JToken state);
    }
}