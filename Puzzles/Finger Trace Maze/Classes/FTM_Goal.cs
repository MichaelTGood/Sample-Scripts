/// <summary> This class is used for storing data for box colliders for a FTM puzzle, such as goals and collision boxes.
/// Use for data storage, not instatiated objects.
/// </summary>
[System.Serializable]
public class FTM_Goal
{
    public float[] Position;
    public float[] Rotation;
    public float[] GoalSize;
    public float[][] ColliderPath;

    public FTM_Goal(){}

    public FTM_Goal(float[] position, float[] rotation, float[] goalSize, float[][] colliderPath)
    {
        Position = position;
        Rotation = rotation;
        GoalSize = goalSize;
        ColliderPath = colliderPath;
    }
}