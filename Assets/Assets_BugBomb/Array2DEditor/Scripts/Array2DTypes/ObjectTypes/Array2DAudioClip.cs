using Array2DEditor.Configurations;
using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DAudioClip : Array2D<AudioClip>
    {
        [SerializeField]
        CellRowAudioClip[] cells = new CellRowAudioClip[Array2DConfigurations.DefaultGridSize];

        protected override CellRow<AudioClip> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}