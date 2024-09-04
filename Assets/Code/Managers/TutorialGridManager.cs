using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Array2DEditor;

public class TutorialGridManager : SingletonBehaviour<TutorialGridManager>, IGridManager
{
    public TutorialManager TutorialManager;

    public Array2DString FirstBlocks;
    public Array2DString SecondBlocks;
    public Array2DString ThirdBlocks;

    public bool AnimationsPlaying { get; private set; }

    public Block DefenderBlock { get; private set; }

    private Block[,] _blocks;

    private List<Block> _allBlocks = new List<Block>();

    private int _gridGenerated;
    private int _defenderValue;

    public Block GetBlock(Vector2Int pos)
    {
        return _blocks[pos.x, pos.y];
    }

    public bool AreNeighbours(Block block1, Block block2)
    {
        //max odległość do sqr(2)
        return Vector2Int.Distance(block1.Position, block2.Position) < 1.45f;
    }

    public void PerformMerge(List<Block> selectedBlocks)
    {
        if(_allBlocks.Count != selectedBlocks.Count)
        {
            FeedbackManager.instance.BadFeedback();
            for (int i = 0; i < selectedBlocks.Count; i++)
            {
                selectedBlocks[i].SetSelected(false);
            }
            InputManager.instance.UpdateLineRenderer(new List<Block>());
            return;
        }

        TutorialHandController.instance.StopAnimation();
        StartCoroutine(MergeCoroutine(selectedBlocks));
    }

    internal void DestroyGrid()
    {
        if(DefenderBlock != null)
            _defenderValue = DefenderBlock.Number;
        else
            _defenderValue = 0;
        transform.BB_DestroyAllChildren();
    }

    public IEnumerator MergeCoroutine(List<Block> selectedBlocks)
    {
        AnimationsPlaying = true;

        var defenderBlock = selectedBlocks[0];
        int blockToMergeIndex = 1;

        var lastBlockPos = selectedBlocks.Last().Position;

        //defenderBlock.SetMoving(true);
        while (blockToMergeIndex < selectedBlocks.Count)
        {
            InputManager.instance.UpdateLineRenderer(selectedBlocks.TakeLast(selectedBlocks.Count - blockToMergeIndex).ToList());

            var posZ = transform.position.z;
            var seq = DOTween.Sequence();
            seq.Insert(0, defenderBlock.transform.DOMoveX(selectedBlocks[blockToMergeIndex].transform.position.x, GameplayConfiguration.instance.BlocksMergeSpeed)
                .SetEase(Ease.Linear));
            seq.Insert(0, defenderBlock.transform.DOMoveY(selectedBlocks[blockToMergeIndex].transform.position.y, GameplayConfiguration.instance.BlocksMergeSpeed)
                .SetEase(Ease.Linear));
            seq.Insert(0, defenderBlock.transform.DOMoveZ(posZ - 1f, GameplayConfiguration.instance.BlocksMergeSpeed / 2f)
                .SetEase(Ease.InSine));
            seq.Insert(GameplayConfiguration.instance.BlocksMergeSpeed / 2f, defenderBlock.transform.DOMoveZ(posZ,
                GameplayConfiguration.instance.BlocksMergeSpeed / 2f)
                .SetEase(Ease.OutSine));
            seq.Insert(0, defenderBlock.transform.DOScale(1.2f, GameplayConfiguration.instance.BlocksMergeSpeed / 2f)
                .SetEase(Ease.InSine));
            seq.Insert(GameplayConfiguration.instance.BlocksMergeSpeed / 2f, defenderBlock.transform.DOScale(1f,
                GameplayConfiguration.instance.BlocksMergeSpeed / 2f)
                .SetEase(Ease.OutSine));
            yield return new WaitForSeconds(GameplayConfiguration.instance.BlocksMergeSpeed - 0.05f);

            //defenderBlock.transform.position = Vector3.MoveTowards(defenderBlock.transform.position,
            //    selectedBlocks[blockToMergeIndex].transform.position,
            //    GameplayConfiguration.instance.BlocksMergeSpeed * Time.deltaTime);

            //if(Vector3.Distance(defenderBlock.transform.position, selectedBlocks[blockToMergeIndex].transform.position) < 0.05f)
            //{
            var pos = selectedBlocks[blockToMergeIndex].Position;

            if (selectedBlocks[blockToMergeIndex].IsMultiplier)
            {
                defenderBlock.Setup(defenderBlock.Number * selectedBlocks[blockToMergeIndex].Multiplier);
            }
            else
            {
                defenderBlock.Setup(defenderBlock.Number + selectedBlocks[blockToMergeIndex].Number);
            }
            _blocks[pos.x, pos.y] = null;

            if (selectedBlocks[blockToMergeIndex].IsMinusBlock)
            {
                selectedBlocks[blockToMergeIndex].FractureOnDestroy();
            }
            else
            {
                if (GameplayConfiguration.instance.FractureNormalBlocks)
                {
                    selectedBlocks[blockToMergeIndex].FractureOnDestroy();
                }
                else
                {
                    Destroy(selectedBlocks[blockToMergeIndex].gameObject);
                }
            }

            blockToMergeIndex++;
            yield return new WaitForSeconds(0.05f);
        }
        //defenderBlock.SetMoving(false);
        FeedbackManager.instance.GoodFeeback();

        _blocks[defenderBlock.Position.x, defenderBlock.Position.y] = null;
        defenderBlock.Position = lastBlockPos;
        _blocks[lastBlockPos.x, lastBlockPos.y] = defenderBlock;
        //CheckLoseCondition();

        AnimationsPlaying = false;

        TutorialManager.OnTutorialStageDone(_gridGenerated);
        yield return new WaitForSeconds(0.3f);

        if(_gridGenerated == 2)
            yield break;

        GenerteGrid(_gridGenerated + 1);
    }

    public void GenerteGrid(int gridNumber)
    {
        DestroyGrid();
        _gridGenerated = gridNumber;
        _allBlocks.Clear();

           Array2DString grid = null;
        if(gridNumber == 0)
            grid = FirstBlocks;
        else if(gridNumber == 1)
            grid = SecondBlocks;
        else
            grid = ThirdBlocks;

        transform.position = Vector3.zero;
        Vector2Int gridSize = new Vector2Int(5, 5);
        _blocks = new Block[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Block block;
                string stringValue = grid.GetCell(x,y);
                if(string.IsNullOrEmpty(stringValue))
                    continue;
                else if(stringValue == "D")
                {
                    block = GridManager.GetDefenderBlock();
                    block.Setup(_defenderValue);
                    DefenderBlock = block;
                }
                else
                {
                    int value = int.Parse(stringValue);
                    block = GridManager.GetBlock(value);
                }

                Vector3 point = new Vector3(x * GameplayConfiguration.instance.CellSize.x, y * GameplayConfiguration.instance.CellSize.y, transform.position.z);
                _blocks[x, y] = block;
                _blocks[x, y].Position = new Vector2Int(x, y);
                _blocks[x, y].transform.SetParent(this.transform);
                _blocks[x, y].transform.position = point;

                _allBlocks.Add(block);
            }
        }

        if(!_allBlocks.Contains(DefenderBlock))
            _allBlocks.Add(DefenderBlock);
        transform.position = Vector3.zero - CalculateCentroid() + Vector3.down * 2;
        if(gridNumber == 0)
            transform.position += Vector3.down;

        TutorialManager.OnGridGenerated(gridNumber);
    }

    private Vector3 CalculateCentroid()
    {
        float xMin = 0;
        float yMin = 0;
        float xMax = 0;
        float yMax = 0;
        foreach (Transform child in transform)
        {
            if(child.GetComponent<Block>() == null)
                continue;

            if (xMin > child.position.x)
                xMin = child.position.x;
            if (yMin > child.position.y)
                yMin = child.position.y;
            if (xMax < child.position.x)
                xMax = child.position.x;
            if (yMax < child.position.y)
                yMax = child.position.y;
        }
        return new Vector3((xMax - xMin) / 2, (yMax - yMin) / 2, 0.25f) + new Vector3(xMin, yMin);
    }
}