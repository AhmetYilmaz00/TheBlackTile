﻿using Array2DEditor.Configurations;
using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DGameObject : Array2D<GameObject>
    {
        [SerializeField]
        CellRowGameObject[] cells = new CellRowGameObject[Array2DConfigurations.DefaultGridSize];

        protected override CellRow<GameObject> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}
