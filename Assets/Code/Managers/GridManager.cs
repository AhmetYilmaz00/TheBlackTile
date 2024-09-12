using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.AIM_Studio;
using UnityEngine;
using Random = System.Random;

public class GridManager : SingletonBehaviour<GridManager>, IGridManager
{
    public int StartingMaxValue = 5;
    public int BlocksValuesMaxRange;
    [SerializeField] private int seed = 1;
    private GameManagerAim _gameManagerAim;

    public int MaxValue => StartingMaxValue + MinusBlocksGenerated / 7;
    public Block DefenderBlock { get; private set; }
    public bool AnimationsPlaying { get; private set; }
    public int MinusBlocksGenerated;

    private Block[,] _blocks;
    private Vector3[,] _positions;

    private void Awake()
    {
        _gameManagerAim = FindObjectOfType<GameManagerAim>();
        Messenger<GameState, GameState>.AddListener(Message.PreGameStateChange, OnGameStatePreChange);
    }

    private void OnDisable()
    {
        Messenger<GameState, GameState>.RemoveListener(Message.PreGameStateChange, OnGameStatePreChange);
    }

    public void OnGameStatePreChange(GameState newGameState, GameState previousGameState)
    {
        if (newGameState == GameState.GameOverLose)
        {
            MinusBlocksGenerated = 0;
            AnimationsPlaying = false;
        }
    }

    public bool AreNeighbours(Block block1, Block block2)
    {
        //max odległość do sqr(2)
        return Vector2Int.Distance(block1.Position, block2.Position) < 1.45f;
    }

    public static Block GetDefenderBlock()
    {
        var block = Instantiate(AssetsConfiguration.instance.DefenderBlockPrefab);
        int number = 0;
        block.Setup(number);

        return block;
    }

    public static Block GetBlock(int number)
    {
        Block block;
        if (number > 0)
        {
            block = Instantiate(AssetsConfiguration.instance.BlockPrefab);
        }
        else
        {
            block = Instantiate(AssetsConfiguration.instance.MinusBlockPrefab);
        }

        block.Setup(number);
        return block;
    }

    private Block GetRandomBlock()
    {
        var block = Instantiate(AssetsConfiguration.instance.BlockPrefab);
        var min = MaxValue - BlocksValuesMaxRange;
        if (min < 1)
            min = 1;
        var rng = new Random(seed + _gameManagerAim.GetElympicsSeed());
        int number = rng.Next(min, MaxValue + 1);
        block.Setup(number);

        return block;
    }

    private Block GetMinusBlock()
    {
        var block = Instantiate(AssetsConfiguration.instance.MinusBlockPrefab);
        block.IsMinusBlock = true;

        var min = MaxValue - BlocksValuesMaxRange;
        if (min < 1)
            min = 1;
        float meanValue = (min + MaxValue) / 2;
        bool isEarlyGame = MinusBlocksGenerated < GameplayConfiguration.instance.EarlyGameLength;

        int number;
        if (isEarlyGame)
        {
            var diffCurve = GameplayConfiguration.instance.StartingDifficultyCurve
                .Evaluate((float)MinusBlocksGenerated / GameplayConfiguration.instance.EarlyGameLength);
            float sequenceLength = diffCurve.Remap(0, 1, GameplayConfiguration.instance.EarlyGameMin,
                GameplayConfiguration.instance.EarlyGameMax);

            number = -Mathf.RoundToInt(meanValue * sequenceLength);
        }
        else
        {
            var lateGameIndex = (MinusBlocksGenerated - GameplayConfiguration.instance.EarlyGameLength) %
                                GameplayConfiguration.instance.LateGameLoop;
            var diffCurve = GameplayConfiguration.instance.LateGateDiffucultyCurve
                .Evaluate((float)lateGameIndex / GameplayConfiguration.instance.LateGameLoop);
            float sequenceLength = diffCurve.Remap(0, 1, GameplayConfiguration.instance.LateGameMin,
                GameplayConfiguration.instance.LateGameMax);

            number = -Mathf.RoundToInt(meanValue * sequenceLength);
        }

        number = Mathf.RoundToInt(number * UnityEngine.Random.Range(
            GameplayConfiguration.instance.DifficultyVariationPercentage.x,
            GameplayConfiguration.instance.DifficultyVariationPercentage.y));

        block.Setup(number);

        return block;
    }

    private Block GetMultiplierBlock()
    {
        var block = Instantiate(AssetsConfiguration.instance.MultiplierBlockPrefab);
        block.SetupMultiplier(5);

        return block;
    }

    private Vector3 CalculateCentroid()
    {
        float xMin = 0;
        float yMin = 0;
        float xMax = 0;
        float yMax = 0;
        foreach (Transform child in transform)
        {
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

    public void PerformMerge(List<Block> selectedBlocks)
    {
        Messenger<List<Block>>.Broadcast(Message.OnMergeMoveStarted, selectedBlocks);
        StartCoroutine(MergeCoroutine(selectedBlocks));
    }

    public IEnumerator MergeCoroutine(List<Block> selectedBlocks)
    {
        AnimationsPlaying = true;

        var defenderBlock = selectedBlocks[0];
        int blockToMergeIndex = 1;

        var lastBlockPos = selectedBlocks.Last().Position;

        if (defenderBlock.FaceAnimator != null)
        {
            defenderBlock.FaceAnimator.Play("Happy");
        }

        //defenderBlock.SetMoving(true);
        while (blockToMergeIndex < selectedBlocks.Count)
        {
            InputManager.instance.UpdateLineRenderer(selectedBlocks.TakeLast(selectedBlocks.Count - blockToMergeIndex)
                .ToList());

            var posZ = transform.position.z;
            var seq = DOTween.Sequence();
            seq.Insert(0, defenderBlock.transform.DOMoveX(selectedBlocks[blockToMergeIndex].transform.position.x,
                    GameplayConfiguration.instance.BlocksMergeSpeed)
                .SetEase(Ease.Linear));
            seq.Insert(0, defenderBlock.transform.DOMoveY(selectedBlocks[blockToMergeIndex].transform.position.y,
                    GameplayConfiguration.instance.BlocksMergeSpeed)
                .SetEase(Ease.Linear));
            seq.Insert(0, defenderBlock.transform
                .DOMoveZ(posZ - 1f, GameplayConfiguration.instance.BlocksMergeSpeed / 2f)
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

            Messenger<Block>.Broadcast(Message.OnBlockMerged, selectedBlocks[blockToMergeIndex]);
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
            //}

            //pierwszy warunek przegranej
            if (defenderBlock.Number < 0)
            {
                defenderBlock.DestroyOnLose();
                FeedbackManager.instance.BadFeedback();
                this.Invoke(() => GameManager.instance.OnLevelLose(), 1.7f);
                yield break;
            }
        }

        if (defenderBlock.FaceAnimator != null)
        {
            defenderBlock.FaceAnimator.Play("Idle");
        }

        //defenderBlock.SetMoving(false);
        FeedbackManager.instance.GoodFeeback();

        _blocks[defenderBlock.Position.x, defenderBlock.Position.y] = null;
        defenderBlock.Position = lastBlockPos;
        _blocks[lastBlockPos.x, lastBlockPos.y] = defenderBlock;

        yield return new WaitForSeconds(0.1f);
        MoveNumbersDown();
        FillEmptyFields();
        SaveLevelData();
        yield return new WaitForSeconds(GameplayConfiguration.instance.BlocksFallTime + 0.1f);

        //CheckLoseCondition();

        AnimationsPlaying = false;
    }

    private void MoveNumbersDown()
    {
        for (int x = 0; x < _blocks.GetLength(0); x++)
        {
            int nextrow = 0;
            bool moveColumn = false;
            for (int y = 0; y < _blocks.GetLength(1); y++)
            {
                if (_blocks[x, y] == null)
                {
                    if (moveColumn == false)
                    {
                        nextrow = y;
                        moveColumn = true;
                    }
                }
                else
                {
                    if (moveColumn && y != nextrow)
                    {
                        _blocks[x, nextrow] = null;
                        _blocks[x, nextrow] = _blocks[x, y];
                        _blocks[x, nextrow].Position = new Vector2Int(x, nextrow);

                        var fallTime = GameplayConfiguration.instance.BlocksFallTime;
                        _blocks[x, nextrow].transform.DOMove(_positions[x, nextrow], fallTime)
                            .SetEase(GameplayConfiguration.instance.BlocksFallEase);
                        _blocks[x, y] = null;
                        nextrow++;
                    }
                }
            }
        }
    }

    private void FillEmptyFields()
    {
        int emptyFieldsCount = 0;
        for (int x = 0; x < _blocks.GetLength(0); x++)
        {
            for (int y = 0; y < _blocks.GetLength(1); y++)
            {
                if (_blocks[x, y] == null)
                {
                    emptyFieldsCount++;
                }
            }
        }

        int minusBlocksOnGrid = 0;
        float minusBlocksSum = 0;
        for (int x = 0; x < _blocks.GetLength(0); x++)
        {
            for (int y = 0; y < _blocks.GetLength(1); y++)
            {
                if (_blocks[x, y] != null && _blocks[x, y].IsMinusBlock)
                {
                    minusBlocksOnGrid++;
                    minusBlocksSum += Mathf.Abs(_blocks[x, y].Number);
                }
            }
        }

        float defenderToMinusesRatio = DefenderBlock.Number / minusBlocksSum;

        int minusBlockIndex = UnityEngine.Random.Range(0, emptyFieldsCount);
        int index = 0;
        for (int x = 0; x < _blocks.GetLength(0); x++)
        {
            for (int y = 0; y < _blocks.GetLength(1); y++)
            {
                if (_blocks[x, y] == null)
                {
                    Block block;
                    if (minusBlockIndex == index && y > 0)
                    {
                        if (minusBlocksOnGrid >= 10 && defenderToMinusesRatio < 0.3f &&
                            LevelManager.instance.CanSpawnMultiplier)
                        {
                            block = GetMultiplierBlock();
                            LevelManager.instance.OnMultiplierSpawned();
                        }
                        else
                        {
                            block = GetMinusBlock();
                            MinusBlocksGenerated++;
                        }
                    }
                    else
                    {
                        block = GetRandomBlock();
                    }

                    block.transform.SetParent(this.transform);
                    block.transform.position = _positions[x, y] + Vector3.up * 4;
                    _blocks[x, y] = block;
                    _blocks[x, y].Position = new Vector2Int(x, y);

                    var fallTime = GameplayConfiguration.instance.BlocksFallTime;
                    _blocks[x, y].transform.DOMove(_positions[x, y], fallTime)
                        .SetEase(GameplayConfiguration.instance.BlocksFallEase);

                    index++;
                }
            }
        }
    }

    public void SaveLevelData()
    {
        var levelData = new LevelData();
        for (int x = 0; x < _blocks.GetLength(0); x++)
        {
            for (int y = 0; y < _blocks.GetLength(1); y++)
            {
                if (_blocks[x, y].IsMultiplier)
                {
                    var tileData = new LevelData.TileData(new Vector2Int(x, y), _blocks[x, y].Multiplier);
                    tileData.IsMultiplier = true;
                    levelData.TilesData.Add(tileData);
                }
                else
                {
                    var tileData = new LevelData.TileData(new Vector2Int(x, y), _blocks[x, y].Number);
                    tileData.IsDefender = _blocks[x, y].IsDefender;
                    levelData.TilesData.Add(tileData);
                }
            }
        }

        levelData.MinusBlocksGenerated = MinusBlocksGenerated;
        levelData.Score = GuiGameplayPanel.instance.Score;


        GameManager.instance.SaveToLevelData(levelData);
    }

    #region Level Creation

    public void GenerteGridRandom()
    {
        transform.position = Vector3.zero;
        Vector2Int gridSize = GameplayConfiguration.instance.GridSize;
        _blocks = new Block[gridSize.x, gridSize.y];
        _positions = new Vector3[gridSize.x, gridSize.y];

        Vector2Int defenderPos = new Vector2Int(Mathf.FloorToInt(gridSize.x / 2f), 0);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Block block;
                if (x == defenderPos.x && y == defenderPos.y)
                {
                    block = GetDefenderBlock();
                    DefenderBlock = block;
                }
                else
                {
                    block = GetRandomBlock();
                }

                Vector3 point = new Vector3(x * GameplayConfiguration.instance.CellSize.x,
                    y * GameplayConfiguration.instance.CellSize.y, transform.position.z);
                _positions[x, y] = point;
                _blocks[x, y] = block;
                _blocks[x, y].Position = new Vector2Int(x, y);
                _blocks[x, y].transform.SetParent(this.transform);
                _blocks[x, y].transform.position = point;
            }
        }

        transform.position = Vector3.zero - CalculateCentroid() + Vector3.down * 0.5f;
        for (int x = 0; x < gridSize.x; x++)
        for (int y = 0; y < gridSize.y; y++)
            _positions[x, y] = _blocks[x, y].transform.position;
    }

    internal void GenerteGridFromData(LevelData levelData)
    {
        transform.position = Vector3.zero;
        Vector2Int gridSize = GameplayConfiguration.instance.GridSize;
        _blocks = new Block[gridSize.x, gridSize.y];
        _positions = new Vector3[gridSize.x, gridSize.y];

        for (int i = 0; i < levelData.TilesData.Count; i++)
        {
            var tile = levelData.TilesData[i];
            var x = tile.Position.x;
            var y = tile.Position.y;

            Block block;
            if (tile.IsDefender)
            {
                block = GetDefenderBlock();
                DefenderBlock = block;
                block.Setup(tile.Value);
            }
            else if (tile.IsMultiplier)
            {
                block = GetMultiplierBlock();
                block.SetupMultiplier(tile.Value);
            }
            else if (tile.Value < 0)
            {
                block = GetMinusBlock();
                block.Setup(tile.Value);
            }
            else
            {
                block = GetRandomBlock();
                block.Setup(tile.Value);
            }

            Vector3 point = new Vector3(x * GameplayConfiguration.instance.CellSize.x,
                y * GameplayConfiguration.instance.CellSize.y, transform.position.z);
            _positions[x, y] = point;
            _blocks[x, y] = block;
            _blocks[x, y].Position = new Vector2Int(x, y);
            _blocks[x, y].transform.SetParent(this.transform);
            _blocks[x, y].transform.position = point;
        }

        MinusBlocksGenerated = levelData.MinusBlocksGenerated;

        transform.position = Vector3.zero - CalculateCentroid() + Vector3.down * 0.5f;
        for (int x = 0; x < gridSize.x; x++)
        for (int y = 0; y < gridSize.y; y++)
            _positions[x, y] = _blocks[x, y].transform.position;
    }


    internal void DestroyCurrentLevel()
    {
        transform.BB_DestroyAllChildren();
    }

    #endregion
}