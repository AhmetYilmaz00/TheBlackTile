using System.Collections.Generic;
using System.Linq;
using Code.AIM_Studio;
using Elympics;
using UnityEngine;

namespace Code.Managers
{
    public class InputManager : SingletonBehaviour<InputManager>, IInitializable
    {
        public LineRenderer LineRenderer;
        public bool inputWait;
        private GameManagerAim _gameManagerAim;

        private LayerMask _blockLayerMask;

        [SerializeField] private List<Block> selectedBlocks;
        private Block _lastSelected;
        private bool _inputHeld;

        private IGridManager _gridManager;
        private PlayerInputControllerAim _playerInputControllerAim;

        public void Initialize()
        {
            _gameManagerAim = FindObjectOfType<GameManagerAim>();
            _blockLayerMask = LayerMask.GetMask(Layers.BLOCK);
            Messenger<GameState, GameState>.AddListener(Message.PreGameStateChange, OnGameStatePreChange);
            _playerInputControllerAim = FindObjectOfType<PlayerInputControllerAim>();
        }

        private void OnDisable()
        {
            Messenger<GameState, GameState>.RemoveListener(Message.PreGameStateChange, OnGameStatePreChange);
        }

        public void OnGameStatePreChange(GameState newGameState, GameState previousGameState)
        {
            if (newGameState == GameState.Gameplay)
            {
                selectedBlocks = new List<Block>();
                _gridManager = GridManager.instance;
            }
            else if (newGameState == GameState.Tutorial)
            {
                selectedBlocks = new List<Block>();
                _gridManager = TutorialGridManager.instance;
            }
            else if (newGameState == GameState.GameOverLose)
            {
                UpdateLineRenderer(new List<Block>());
            }
        }


        private void Update()
        {
            if (!_playerInputControllerAim)
            {
                _playerInputControllerAim = FindObjectOfType<PlayerInputControllerAim>();
            }


            if (GameManager.instance.GameState != GameState.Gameplay &&
                GameManager.instance.GameState != GameState.Tutorial)
                return;
            if (_gridManager.AnimationsPlaying)
            {
                return;
            }
            
            if (inputWait)
            {
                return;
            }

            if (Input.GetMouseButtonUp(0) || _playerInputControllerAim.serverMouseButtonState == 2)
            {
                if (!_inputHeld)
                    return;


                if (selectedBlocks.Count > 1)
                {
                    _gridManager.DefenderBlock.SetSelected(false);
                    _gameManagerAim.currentHandBlocks = selectedBlocks.Count;
                    _gridManager.PerformMerge(selectedBlocks);
                }
                else
                {
                    FeedbackManager.instance.BadFeedback();
                    for (int i = 0; i < selectedBlocks.Count; i++)
                    {
                        selectedBlocks[i].SetSelected(false);
                    }
                }

                _gridManager.DefenderBlock.ClearNumberPreview();
                selectedBlocks = new List<Block>();
            }
            else if (Input.GetMouseButtonDown(0) || _playerInputControllerAim.serverMouseButtonState == 1)
            {
                _playerInputControllerAim.serverMouseButtonState = 0;
                RaycastHit blockHit;
                if (_gameManagerAim.IsServer())
                {
                    var input = new Vector3(_playerInputControllerAim.serverMousePositionX,
                        _playerInputControllerAim.serverMousePositionY, 0);
                    blockHit = PerformRaycast(input, _blockLayerMask);

                    _gameManagerAim.ClearAllData();
                }
                else
                {
                    blockHit = PerformRaycast(Input.mousePosition, _blockLayerMask);
                }

                if (blockHit.collider == null)
                {
                    Debug.Log("blockHit.collider is Null ");


                    _inputHeld = false;
                    return;
                }


                var selectedBlock = blockHit.collider.GetComponent<Block>();

                if (selectedBlock
                    .IsDefender) // || _gridManager.AreNeighbours(selectedBlock, _gridManager.DefenderBlock))
                {
                    if (!selectedBlock.IsDefender)
                    {
                        selectedBlocks.Add(_gridManager.DefenderBlock);
                        _gridManager.DefenderBlock.SetSelected(true);
                    }

                    if (!selectedBlocks.Contains(selectedBlock))
                    {
                        selectedBlocks.Add(selectedBlock);
                        selectedBlock.SetSelected(true);
                        _lastSelected = selectedBlock;
                        FeedbackManager.instance.LightFeedback();
                        UpdateLineRenderer();
                        _inputHeld = true;
                    }
                }
                else
                {
                    FeedbackManager.instance.BadFeedback();
                    _inputHeld = false;
                }
            }
            else if (Input.GetMouseButton(0) || _playerInputControllerAim.serverIsDraggingState)
            {
                if (!_inputHeld)
                    return;


                RaycastHit blockHit;
                if (_gameManagerAim.IsServer())
                {
                    blockHit = PerformRaycast(
                        new Vector3(_playerInputControllerAim.serverMousePositionX,
                            _playerInputControllerAim.serverMousePositionY, 0), _blockLayerMask);
                }
                else
                {
                    _playerInputControllerAim.serverIsDraggingState = true;
                    blockHit = PerformRaycast(Input.mousePosition, _blockLayerMask);
                }

                if (blockHit.collider == null)
                    return;

                var selectedBlock = blockHit.collider.GetComponent<Block>();
                if (selectedBlock == _lastSelected)
                    return;

                if (selectedBlocks.Contains(selectedBlock))
                {
                    //przedostatni
                    if (selectedBlocks.IndexOf(selectedBlock) == selectedBlocks.Count - 2)
                    {
                        selectedBlocks[selectedBlocks.Count - 1].SetSelected(false);
                        selectedBlocks.RemoveAt(selectedBlocks.Count - 1);
                        _lastSelected = selectedBlock;
                        UpdateLineRenderer();
                        FeedbackManager.instance.LightFeedback();

                        var firstAnimator = selectedBlocks[0].Animator.GetCurrentAnimatorStateInfo(0);
                        for (int i = 0; i < selectedBlocks.Count; i++)
                        {
                            selectedBlocks[i].Animator
                                .Play(firstAnimator.fullPathHash, 0, firstAnimator.normalizedTime);
                        }
                    }
                }
                else
                {
                    if (_gridManager.AreNeighbours(selectedBlock, _lastSelected) && CanSelect(selectedBlock))
                    {
                        selectedBlock.SetSelected(true);
                        selectedBlocks.Add(selectedBlock);
                        _lastSelected = selectedBlock;
                        UpdateLineRenderer();
                        FeedbackManager.instance.LightFeedback();

                        var firstAnimator = selectedBlocks[0].Animator.GetCurrentAnimatorStateInfo(0);
                        for (int i = 0; i < selectedBlocks.Count; i++)
                        {
                            selectedBlocks[i].Animator
                                .Play(firstAnimator.fullPathHash, 0, firstAnimator.normalizedTime);
                        }
                    }
                }

                if (selectedBlocks.Any())
                {
                    if (selectedBlocks[0] == _gridManager.DefenderBlock)
                    {
                        selectedBlocks[0].SetupNumberPreviewOnly(selectedBlocks);
                    }
                    else
                    {
                        selectedBlocks.Clear();
                    }
                }
            }
        }

        private void UpdateLineRenderer()
        {
            UpdateLineRenderer(selectedBlocks);
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
            if (_lastSelected.IsDefender || _lastSelected.IsMinusBlock || _lastSelected.IsMultiplier)
                return true;
            else
                return blockToSelect.Number == _lastSelected.Number || blockToSelect.Number == _lastSelected.Number + 1
                                                                    || blockToSelect.IsMinusBlock ||
                                                                    blockToSelect.IsMultiplier;
        }

        private RaycastHit PerformRaycast(Vector3 mousePos, LayerMask layerMask)
        {
            var ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.SphereCast(ray, 0.3f, out hit, 200, layerMask);
            return hit;
        }
    }
}