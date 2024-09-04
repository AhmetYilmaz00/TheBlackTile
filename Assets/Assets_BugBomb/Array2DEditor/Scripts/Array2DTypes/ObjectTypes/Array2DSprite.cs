using Array2DEditor.Configurations;
using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DSprite : Array2D<Sprite>
    {
        [SerializeField]
        CellRowSprite[] cells = new CellRowSprite[Array2DConfigurations.DefaultGridSize];

        protected override CellRow<Sprite> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}
