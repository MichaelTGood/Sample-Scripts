/// <summary> Defined by a UniqueID in the dictionary in PuzzleManager
/// </summary>
[System.Serializable]
public class FTM 
{   
    public Puzzle_Image BG;
    public Puzzle_Image Hero;
    public Puzzle_Image Obs;
    public int Timer;
    public int Attempts;
    public int ObstacleCount;
    public string Header;
    public string Subheader;
    public string Rules;
    public FTM_Data Data;

    public FTM(){}

    public FTM(Puzzle_Image bG, Puzzle_Image hero, Puzzle_Image obs, int timer, int attempts, int obstacleCount, string header, string subheader, string rules, FTM_Data data)
    {
        BG = bG;
        Hero = hero;
        Obs = obs;
        Timer = timer;
        Attempts = attempts;
        ObstacleCount = obstacleCount;
        Header = header;
        Subheader = subheader;
        Rules = rules;
        Data = data;
    }
}