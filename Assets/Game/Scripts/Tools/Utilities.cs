/*-------------------------
File: Utilities.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;
//---------------------------------

namespace EldwynGrove
{
    public static class Utilities
    {
        /*----------------------------------------------------------------
        | --- CheckForNull: Utility method to check for null objects --- |
        ----------------------------------------------------------------*/
        public static void CheckForNull<T>(T obj, string name)
        {
            // Handle custom classes
            if (obj == null)
            {
                Debug.LogError($"{name} is null. Please ensure it is assigned in the inspector.");
            }
            // Handle UnityEngine.Object types
            else if (obj is Object unityObj)
            {
                if (unityObj == null)
                {
                    Debug.LogError($"{name} is not a valid Object. Please ensure it is assigned in the inspector.");
                }
                return;
            }
        }

    }
}