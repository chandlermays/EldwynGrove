/*-------------------------
File: ForageManager.cs
Author: Chandler Mays
-------------------------*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//---------------------------------

namespace EldwynGrove.Inventories
{
    public class ForageManager : MonoBehaviour
    {
        public static ForageManager Instance { get; private set; }

        [System.Serializable]
        public struct ForageEntry
        {
            [Tooltip("The TileBase asset painted onto the Forage tilemap.")]
            public TileBase Tile;

            [Tooltip("The ForageItem SO asset that this tile represents.")]
            public ForageItem Item;
        }

        [SerializeField] private Tilemap m_forageTilemap;
        [SerializeField] private List<ForageEntry> m_forageEntries;

        private Dictionary<Vector2Int, ForageItem> m_forageMap = new();
        private Dictionary<TileBase, ForageItem> m_tileToItem = new();

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            BuildLookups();
        }

        /*-----------------------------------------------------------------------------------------
        | --- BuildLookups: Precomputes mappings from tiles to items and coordinates to items --- |
        -----------------------------------------------------------------------------------------*/
        private void BuildLookups()
        {
            m_tileToItem.Clear();
            m_forageMap.Clear();

            foreach (ForageEntry entry in m_forageEntries)
            {
                if (entry.Tile != null && entry.Item != null)
                {
                    m_tileToItem[entry.Tile] = entry.Item;
                }
            }

            BoundsInt bounds = m_forageTilemap.cellBounds;
            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                TileBase tile = m_forageTilemap.GetTile(cellPos);
                if (tile == null)
                    continue;

                if (m_tileToItem.TryGetValue(tile, out ForageItem item))
                {
                    Vector2Int coords = new Vector2Int(cellPos.x, cellPos.y);
                    m_forageMap[coords] = item;
                }
            }
        }

        /*-----------------------------------------------------------------------------
        | --- GetForageAt: Retrieves the ForageItem at the given tile coordinates --- |
        -----------------------------------------------------------------------------*/
        public ForageItem GetForageAt(Vector2Int coords)
        {
            return m_forageMap.TryGetValue(coords, out ForageItem item) ? item : null;
        }

        /*--------------------------------------------------------------------------------
        | --- HasForageAt: Checks if there is a forage item at the given coordinates --- |
        --------------------------------------------------------------------------------*/
        public bool HasForageAt(Vector2Int coords)
        {
            return m_forageMap.ContainsKey(coords);
        }

        /*------------------------------------------------------------------------
        | --- RemoveForage: Removes the forage item at the given coordinates --- |
        ------------------------------------------------------------------------*/
        public ForageItem RemoveForage(Vector2Int coords)
        {
            if (!m_forageMap.TryGetValue(coords, out ForageItem item))
                return null;

            m_forageMap.Remove(coords);
            m_forageTilemap.SetTile(new Vector3Int(coords.x, coords.y, 0), null);
            return item;
        }

        /*----------------------------------------------------------------------------------------------------
        | --- GetWorldCenter: Gets the world position of the center of the tile at the given coordinates --- |
        ----------------------------------------------------------------------------------------------------*/
        public Vector3 GetWorldCenter(Vector2Int coords)
        {
            return m_forageTilemap.GetCellCenterWorld(new Vector3Int(coords.x, coords.y, 0));
        }

        /*-------------------------------------------------------------------------------------------------
        | --- GetCoordsFromWorld: Converts a world position to tile coordinates on the forage tilemap --- |
        -------------------------------------------------------------------------------------------------*/
        public Vector2Int GetCoordsFromWorld(Vector3 worldPos)
        {
            Vector3Int cell = m_forageTilemap.WorldToCell(worldPos);
            return new Vector2Int(cell.x, cell.y);
        }
    }
}