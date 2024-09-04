using System.Collections.Generic;

internal interface IGridManager
{
    bool AnimationsPlaying { get; }
    Block DefenderBlock { get; }

    bool AreNeighbours(Block selectedBlock, Block defenderBlock);
    void PerformMerge(List<Block> selectedBlocks);
}