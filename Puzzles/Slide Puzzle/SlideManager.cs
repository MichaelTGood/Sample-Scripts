using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SlideManager : MonoBehaviour
{
    #region Variables

        [SerializeField] private enum PuzzleState {Solved, Shuffling, InPlay};
        [SerializeField] private PuzzleState state;

        [SerializeField] private Texture2D image;
        [SerializeField] private bool traditionalSlidePuzzle = true;
        [SerializeField] private Toggle traditionalPuzzleSwitch;

        [SerializeField] private float defaultMoveDuration = 0.2f;
        [SerializeField] private float shuffleMoveDuration = 0.1f;

        [SerializeField] private SlideBlock emptyBlock;
        [SerializeField] private SlideBlock[,] blocks;
        [SerializeField] private int shuffleMovesRemaining;

        [SerializeField] private Vector2Int previousShuffleOffset;

        private Queue<SlideBlock> inputs;
        private bool blockIsMoving;

        [SerializeField] private Slide thisPuzzle;

        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] private Camera puzzleCamera;

    #endregion

    private void Awake()
    {
        if(null != GameObject.FindObjectOfType<PuzzleManager>())
        { 
            puzzleManager = GameObject.FindObjectOfType<PuzzleManager>();
            thisPuzzle = puzzleManager.CurrentSlide;
        }
        else
        {
            Debug.Log("No <color=\"purple\">PuzzleManager>/color> found.");
        }
        
        

    }

    private void Start()
    {
        StartCoroutine(CreatePuzzle());
    }

    private IEnumerator CreatePuzzle()
    {
        traditionalPuzzleSwitch.isOn = thisPuzzle.Traditional;

        yield return GetPuzzleImage();

        yield return CreateBlocks();

        //-- Moves the puzzle up, so the Map scene is not visible.
        transform.localPosition = new Vector3(0, 500f, 0);

        //-- Resets the camera, based on the size of the puzzle.
        Vector3 cameraPosition = puzzleCamera.transform.localPosition;
        cameraPosition.z = -thisPuzzle.Blocks[0] * 2;
        puzzleCamera.transform.localPosition = cameraPosition;        

        yield return StartShuffle();
    }

    private IEnumerator GetPuzzleImage()
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(CCC.Azure.Images + thisPuzzle.Image.Address + puzzleManager.DataManager.GameManager.ImagesSAS);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        { Debug.Log(uwr.error);}
        else
        {
            image = DownloadHandlerTexture.GetContent(uwr);
        }

        yield return null;
    }

    /// <summary> Creates 3D blocks, parents them to this gameObject ("SlideManager").
    /// </summary>
    private object CreateBlocks()
    {
        blocks = new SlideBlock[thisPuzzle.Blocks[0],thisPuzzle.Blocks[1]];

        Texture2D[,] imageSlices = ImageSlicer.GetSlices(image, thisPuzzle.Blocks[0],thisPuzzle.Blocks[1]);

        for (int y = 0; y < thisPuzzle.Blocks[1]; y++)
        {
            for (int x = 0; x < thisPuzzle.Blocks[0]; x++)
            {
                GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blockObject.transform.position = -Vector2.one * (thisPuzzle.Blocks[0] - 1) * 0.5f + new Vector2(x, y);
                blockObject.transform.parent = transform;

                SlideBlock block = blockObject.AddComponent<SlideBlock>();

                //-- Assign Actions
                block.onBlockPressed += PlayerMoveBlockInput;
                block.onFinishedMoving += onBlockFinishedMoving;

                //-- Set is coord and image
                block.Init(new Vector2Int(x,y), imageSlices[x,y]);
                blocks[x,y] = block;

                //-- If it's the bottom right block, turn it off.
                if (y == thisPuzzle.EmptyBlock[1] && x == thisPuzzle.EmptyBlock[0])
                {
                    emptyBlock = block;
                }
            }
        }



        inputs = new Queue<SlideBlock>();

        return null;
    }



    public void PlayerMoveBlockInput(SlideBlock blockToMove)
    {
        if (state == PuzzleState.InPlay)
        {
            inputs.Enqueue(blockToMove);
            MakeNextPlayerMove();
        }
    }



    private void MakeNextPlayerMove()
    {
        while (inputs.Count > 0 && !blockIsMoving)
        {
            MoveBlock(inputs.Dequeue(), defaultMoveDuration);
        }
    }





    private void MoveBlock(SlideBlock blockToMove, float duration)
    {
        if (thisPuzzle.Traditional)
        {
            if ((blockToMove.coord - emptyBlock.coord).sqrMagnitude == 1)
            {
                blocks[blockToMove.coord.x, blockToMove.coord.y] = emptyBlock;
                blocks[emptyBlock.coord.x, emptyBlock.coord.y] = blockToMove;

                Vector2Int targetCoord = emptyBlock.coord;
                emptyBlock.coord = blockToMove.coord;
                blockToMove.coord = targetCoord;

                Vector2 targetPosition = emptyBlock.transform.position;
                emptyBlock.transform.position = blockToMove.transform.position;
                // blockToMove.transform.localposition = targetPosition;
                StartCoroutine(blockToMove.AnimateMove(targetPosition, duration));
                blockIsMoving = true;
            } 
        }
        else
        {
                blocks[blockToMove.coord.x, blockToMove.coord.y] = emptyBlock;
                blocks[emptyBlock.coord.x, emptyBlock.coord.y] = blockToMove;

                Vector2Int targetCoord = emptyBlock.coord;
                emptyBlock.coord = blockToMove.coord;
                blockToMove.coord = targetCoord;

                Vector2 targetPosition = emptyBlock.transform.position;
                emptyBlock.transform.position = blockToMove.transform.position;
                // blockToMove.transform.position = targetPosition;
                StartCoroutine(blockToMove.AnimateMove(targetPosition, duration));
                blockIsMoving = true;
        }
    }



    private void onBlockFinishedMoving()
    {
        blockIsMoving = false;
        CheckIfSolved();

        if(state == PuzzleState.InPlay)
        {
            MakeNextPlayerMove();
        }
        else if (state == PuzzleState.Shuffling)
        {

            if (shuffleMovesRemaining > 0)
            {
                MakeNextShuffleMove();
            }
            else
            {
                state = PuzzleState.InPlay;
            }

        }
    }



    public void SetTraditionalPuzzle(bool value)
    {
        thisPuzzle.Traditional = value;
    }



    //-- Moves the emplyBlock and one randomly selected neighbor.
    private void MakeNextShuffleMove()
    {
        Vector2Int[] offsets = {new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1)};
        int randomIndex = UnityEngine.Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if (offset != previousShuffleOffset * -1)
            {
                Vector2Int moveBlockCoord = emptyBlock.coord + offset;

                if (moveBlockCoord.x >= 0 && moveBlockCoord.x < thisPuzzle.Blocks[0] && moveBlockCoord.y >= 0 && moveBlockCoord.y < thisPuzzle.Blocks[1])
                {
                    MoveBlock(blocks[moveBlockCoord.x, moveBlockCoord.y], shuffleMoveDuration);
                    shuffleMovesRemaining--;
                    previousShuffleOffset = offset;
                    break;
                }
            }
        }

    }

    private void CheckIfSolved()
    {
        foreach (SlideBlock block in blocks)
        {
            if (!block.IsAtStartingCoord())
            {
                return;
            }
        }

        state = PuzzleState.Solved;
        emptyBlock.gameObject.SetActive(true);

        puzzleManager.UnloadPuzzle(true);
    }

    public void PuzzleFailed()
    {
        puzzleManager.UnloadPuzzle(false);
    }


    public object StartShuffle()
    {
        if (state == PuzzleState.Solved)
        {
            state = PuzzleState.Shuffling;
            shuffleMovesRemaining = thisPuzzle.ShuffleLength;

            emptyBlock.gameObject.SetActive(false);

            MakeNextShuffleMove();
        }

        return null;
    }






}