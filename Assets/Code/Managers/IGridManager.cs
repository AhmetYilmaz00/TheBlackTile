using System.Collections.Generic;

namespace Code.Managers
{
    internal interface IGridManager
    {
        public bool AnimationsPlaying { get; }
        Block DefenderBlock { get; }

        bool AreNeighbours(Block selectedBlock, Block defenderBlock);
        void PerformMerge(List<Block> selectedBlocks);
    }
}