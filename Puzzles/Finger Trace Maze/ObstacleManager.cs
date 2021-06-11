using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    #region Variables

        [ResponsiveButtonGroup(), Button(ButtonSizes.Medium), GUIColor(0, 0.75f, 0, 1), PropertySpace(SpaceBefore=25, SpaceAfter=25)]
        [PropertyOrder(0)]
        private void RelistObstacles()
        { ResetObstacleList(); }


        public List<FTM_Obstacle_Obj> Obstacles = new List<FTM_Obstacle_Obj>();


    #endregion

    private void Awake()
    {
        //-- Checks if elements in obstacle array are valid, clears and recollects if necessary.
        bool resetObstacles = false;

        foreach (var ob in Obstacles)
        {
            if(ob == null)
            {
                resetObstacles = true;
                break;
            }
        }

        if(resetObstacles)
        {
            ResetObstacleList();
        }
    }

    /// <summary> Removes all elements in obstacles List, and recollects them from children.
    /// </summary>
    private void ResetObstacleList()
    {
        Obstacles.Clear();
        Obstacles = new List<FTM_Obstacle_Obj>(GetComponentsInChildren<FTM_Obstacle_Obj>(true));
    }






}