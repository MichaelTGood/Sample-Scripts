using UnityEngine;

/// <summary> This class is used for storing data for Obstacles for a FTM puzzle.
/// Use for data storage, not instatiated objects.
/// </summary>
[System.Serializable]
public class FTM_Obstacle
{
    public float[] Position;
    public float[] Rotation;
    public float[] Scale;
    public float[] Size;
    public float[][][] ColliderPath;


    public FTM_Obstacle(){}

    public FTM_Obstacle(float[] position, float[] rotation, float[] scale, float[] size, float[][][] colliderPath)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
        Size = size;
        ColliderPath = colliderPath;
    }
}