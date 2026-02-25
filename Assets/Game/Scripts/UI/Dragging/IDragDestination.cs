namespace EldwynGrove.UI.Dragging
{
    public interface IDragDestination<T> where T : class
    {
        /// <summary>Max items that can be accepted.</summary>
        int GetMaxItemsCapacity(T item);

        /// <summary>Adds items to the destination.</summary>
        void AddItems(T item, int quantity);
    }
}