using Array2DEditor.Configurations;
using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DString : Array2D<string>
    {
        [SerializeField]
        CellRowString[] cells = new CellRowString[Array2DConfigurations.DefaultGridSize];

        protected override CellRow<string> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}
