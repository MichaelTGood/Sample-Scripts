using UnityEngine;

/// <summary> This class is used for storing data for hero unit for a FTM puzzle.
/// Use for data storage, not instatiated objects.
/// </summary>
[System.Serializable]
public class FTM_Hero : FTM_Collider
{
    public float[] Position;
    public float[] Rotation;
    public float[] Scale;

    public FTM_Hero(){}
    public FTM_Hero(float[] position, float[] rotation, float[] scale, float[] size)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
        ColliderSize = size;
    }
}