/*-------------------------
File: IDragContainer.cs
Author: Chandler Mays
-------------------------*/
namespace EldwynGrove.UI.Dragging
{
    public interface IDragContainer<T> : IDragDestination<T>, IDragSource<T> where T : class
    { }
}