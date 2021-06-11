[System.Serializable]
public class Slide
{

    /// <summary> The remote url for the image to be sliced and used in the slide puzzle.
    /// </summary>
    public Puzzle_Image Image;
    /// <summary> The diminsion of the puzzle, in blocks. [0] = Width, [1] = Height.
    /// </summary>
    public int[] Blocks;
    /// <summary> The 0-based coordinate of the empty block. [0] = Width, [1] = Height.
    /// </summary>
    public int[] EmptyBlock;
    /// <summary> How many tiles should be moved during the initial shuffle.
    /// </summary>
    public int ShuffleLength;
    /// <summary> Can only move blocks next to the empty space?
    /// </summary>
    public bool Traditional;
    /// <summary> Does this puzzle have a timer?
    /// </summary>
    public bool Timer;
    /// <summary> The length of the timer, if HasTimer is True;
    /// </summary>
    public int TimerLength;

    public Slide(){}

    public Slide(Puzzle_Image image, int[] blocks, int[] emptyBlock, int shuffleLength, bool traditional, bool timer, int timerLength)
    {
        // UniqueId = uniqueId;
        Image = image;
        Blocks = blocks;
        EmptyBlock = emptyBlock;
        ShuffleLength = shuffleLength;
        Traditional = traditional;
        Timer = timer;
        TimerLength = timerLength;
    }
}