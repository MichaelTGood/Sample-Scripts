using UnityEngine;

/// <summary> This class is used for storing data for camera for a FTM puzzle.
/// Use for data storage, not instatiated objects.
/// </summary>
[System.Serializable]
public class FTM_Camera
{
    public float[] Position;
    public float[] Rotation;

    /// <summary> Camera's Clipping Planes. 0 = Near, 1 = Far
    /// </summary>
    public float[] Clipping;

    public FTM_Camera(){}

    public FTM_Camera(float[] position, float[] rotation, float[] clipping)
    {
        Position = position;
        Rotation = rotation;
        Clipping = clipping;
    }
}

