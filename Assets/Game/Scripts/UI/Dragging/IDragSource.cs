namespace EldwynGrove.UI.Dragging
{
    public interface IDragSource<T> where T : class
    {
        /// <summary>Get the m_item being dragged.</summary>
        T GetItem();

        /// <summary>Get the m_quantity of the m_item being dragged.</summary>
        int GetQuantity();

        /// <summary>Remove a given number of items from the source.</summary>
        void RemoveItems(int quantity);
    }
}