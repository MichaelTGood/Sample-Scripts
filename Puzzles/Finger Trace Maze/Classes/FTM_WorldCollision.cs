/// <summary> This class is used for storing data for box colliders for a FTM puzzle, such as goals and collision boxes.
/// Use for data storage, not instatiated objects.
/// </summary>
[System.Serializable]
public class FTM_WorldCollision
{
    public float[] Position;
    public float[] Scale;
    public float[][][] ColliderPath;


    public FTM_WorldCollision(){}

    public FTM_WorldCollision(float[] position, float[] scale, float[][][] colliderPath)
    {
        Position = position;
        Scale = scale;
        ColliderPath = colliderPath;
    }
}