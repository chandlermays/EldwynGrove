/*-------------------------
File: IRaycastable.cs
Author: Chandler Mays
-------------------------*/
using EldwynGrove.Player;
//---------------------------------

namespace EldwynGrove.Core
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController playerController);
    }
}