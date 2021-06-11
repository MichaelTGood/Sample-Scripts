[System.Serializable]
public class FTM_Data
{
    public FTM_Camera Camera;
    public FTM_Hero Hero;
    public FTM_Goal[] Goals;
    public FTM_Obstacle[] Obstacles;
    public FTM_WorldCollision WorldCollision;

    public FTM_Data() {}

    public FTM_Data(FTM_Camera camera, FTM_Hero hero, FTM_Goal[] goals, FTM_Obstacle[] obstacles, FTM_WorldCollision worldCollision)
    {
        Camera = camera;
        Hero = hero;
        Goals = goals;
        Obstacles = obstacles;
        WorldCollision = worldCollision;
    }
}