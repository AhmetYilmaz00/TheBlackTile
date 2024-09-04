using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public List<TileData> TilesData;
    public int Score;
    public int MinusBlocksGenerated;

    public LevelData()
    {
        TilesData = new List<TileData>();
    }

    [Serializable]
    public class TileData
    {
        public Vector2Int Position;
        public int Value;
        public bool IsDefender;
        public bool IsMultiplier;

        public TileData(Vector2Int position, int value)
        {
            Position = position;
            Value = value;
        }
    }
}
