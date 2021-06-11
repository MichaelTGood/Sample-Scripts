using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTM_LineController : MonoBehaviour
{
    #region Variables

        [SerializeField] private LineRenderer lineRenderer;
        /// <summary> The amount to subtract from the Pos Z of each added point.
        /// </summary>
        [SerializeField] private float depthAdjust;

    #endregion

    public void AddPointToLine(Vector3 newPoint)
    {
        newPoint.z -= depthAdjust;
        
        lineRenderer.SetPosition(lineRenderer.positionCount++, newPoint);
    }
}
