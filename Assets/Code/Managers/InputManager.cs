using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : SingletonBehaviour<InputManager>
{
    public LineRenderer LineRenderer;

    private LayerMask _blockLayerMask;

    private List<Block> _selectedBlocks;
    private Block _lastSelected;
    private bool _inputHeld;

    private IGridManager _gridManager;

    private void Awake()
    {
        _blockLayerMask = LayerMask.GetMask(Layers.BLOCK);
        Messenger<GameState, GameState>.AddListener(Message.PreGameStateChange, OnGameStatePreChange);
    }

    private void OnDisable()
    {
        Messenger<GameState, GameState>.RemoveListener(Message.PreGameStateChange, OnGameStatePreChange);
    }

    public void OnGameStatePreChange(GameState newGameState, GameState previousGameState)
    {
        if (newGameState == GameState.Gameplay)
        {
            _selectedBlocks = new List<Block>();
            _gridManager = GridManager.instance;
        }
        else if(newGameState == GameState.Tutorial)
        {
            _selectedBlocks = new List<Block>();
            _gridManager = TutorialGridManager.instance;
        }
        else if(newGameState == GameState.GameOverLose)
        {
            UpdateLineRenderer(new List<Block>());
        }
    }

    private void Update()
    {
        if (GameManager.instance.GameState != GameState.Gameplay && GameManager.instance.GameState != GameState.Tutorial)
            return;

        if(_gridManager.AnimationsPlaying)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit blockHit = PerformRaycast(Input.mousePosition, _blockLayerMask);
            if (blockHit.collider == null)
            {
                _inputHeld = false;
                return;
            }
            var selectedBlock = blockHit.collider.GetComponent<Block>();

            if(selectedBlock.IsDefender || _gridManager.AreNeighbours(selectedBlock, _gridManager.DefenderBlock))
            {
                if (!selectedBlock.IsDefender)
                {
                    _selectedBlocks.Add(_gridManager.DefenderBlock);
                    _gridManager.DefenderBlock.SetSelected(true);
                }
                _selectedBlocks.Add(selectedBlock);
                selectedBlock.SetSelected(true);
                _lastSelected = selectedBlock;
                FeedbackManager.instance.LightFeedback();
                UpdateLineRenderer();
                _inputHeld = true;
            }
            else
            {
                FeedbackManager.instance.BadFeedback();
                _inputHeld = false;
            }
        }
        else if(Input.GetMouseButton(0))
        {
            if(!_inputHeld)
                return;

            RaycastHit blockHit = PerformRaycast(Input.mousePosition, _blockLayerMask);
            if (blockHit.collider == null)
                return;

            var selectedBlock = blockHit.collider.GetComponent<Block>();
            if (selectedBlock == _lastSelected)
                return;

            if (_selectedBlocks.Contains(selectedBlock))
            {
                //przedostatni
                if(_selectedBlocks.IndexOf(selectedBlock) == _selectedBlocks.Count - 2)
                {
                    _selectedBlocks[_selectedBlocks.Count - 1].SetSelected(false);
                    _selectedBlocks.RemoveAt(_selectedBlocks.Count - 1);
                    _lastSelected = selectedBlock;
                    UpdateLineRenderer();
                    FeedbackManager.instance.LightFeedback();

                    var firstAnimator = _selectedBlocks[0].Animator.GetCurrentAnimatorStateInfo(0);
                    for (int i = 0; i < _selectedBlocks.Count; i++)
                    {
                        _selectedBlocks[i].Animator.Play(firstAnimator.fullPathHash, 0, firstAnimator.normalizedTime);
                    }
                }
            }
            else
            {
                if(_gridManager.AreNeighbours(selectedBlock, _lastSelected) && CanSelect(selectedBlock))
                {
                    selectedBlock.SetSelected(true);
                    _selectedBlocks.Add(selectedBlock);
                    _lastSelected = selectedBlock;
                    UpdateLineRenderer();
                    FeedbackManager.instance.LightFeedback();

                    var firstAnimator = _selectedBlocks[0].Animator.GetCurrentAnimatorStateInfo(0);
                    for (int i = 0; i < _selectedBlocks.Count; i++)
                    {
                        _selectedBlocks[i].Animator.Play(firstAnimator.fullPathHash, 0, firstAnimator.normalizedTime);
                    }
                }
            }

            if(_selectedBlocks.Any())
                _selectedBlocks[0].SetupNumberPreviewOnly(_selectedBlocks);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (!_inputHeld)
                return;

            if(_selectedBlocks.Count > 1)
            {
                _gridManager.DefenderBlock.SetSelected(false);
                _gridManager.PerformMerge(_selectedBlocks);
            }
            else
            {
                FeedbackManager.instance.BadFeedback();
                for (int i = 0; i < _selectedBlocks.Count; i++)
                {
                    _selectedBlocks[i].SetSelected(false);
                }
            }
            _selectedBlocks[0].ClearNumberPreview();
            _selectedBlocks = new List<Block>();
        }
    }

    private void UpdateLineRenderer()
    {
        UpdateLineRenderer(_selectedBlocks);
    }

    public void UpdateLineRenderer(List<Block> blocks)
    {
        if (blocks.Count < 2)
        {
            LineRenderer.enabled = false;
            return;
        }

        var linePositions = new List<Vector3>();

        for (int i = 0; i < blocks.Count; i++)
        {
            linePositions.Add(blocks[i].transform.position + GameplayConfiguration.instance.LineOffset);
        }

        LineRenderer.positionCount = linePositions.Count;
        LineRenderer.SetPositions(linePositions.ToArray());
        LineRenderer.enabled = true;
    }

    private bool CanSelect(Block blockToSelect)
    {
        if(_lastSelected.IsDefender || _lastSelected.IsMinusBlock || _lastSelected.IsMultiplier)
            return true;
        else
            return blockToSelect.Number == _lastSelected.Number || blockToSelect.Number == _lastSelected.Number + 1 
                || blockToSelect.IsMinusBlock || blockToSelect.IsMultiplier;
    }

    private RaycastHit PerformRaycast(Vector3 mousePos, LayerMask layerMask)
    {
        var ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Physics.SphereCast(ray, 0.3f, out hit, 200, layerMask);
        return hit;
    }
}