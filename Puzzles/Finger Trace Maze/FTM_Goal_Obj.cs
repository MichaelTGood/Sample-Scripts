using System;
using Lean.Gui;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class FTM_Goal_Obj : MonoBehaviour
{
    #region Variables

        [SerializeField] private FTMManager ftmmanager;
        [SerializeField] private RectTransform rt;
        [SerializeField] private PolygonCollider2D polygonCollider;
        [SerializeField] private Rigidbody2D rb2D;
        [SerializeField] private LeanBox box;
        [SerializeField] private LeanBox glow;

        /// <summary> The lowest the alpha of the glow will be set to.
        /// </summary>
        [SerializeField] private float glowMin = 0.25f;
        /// <summary> The highest the alpha of the glow will be set to.
        /// </summary>
        [SerializeField] private float glowMax = 0.75f;
        /// <summary> The speed, in seconds, of the glow adjusting.
        /// </summary>
        [SerializeField] private float glowSpeed = 1;

    public RectTransform Rt { get => rt; }
    public PolygonCollider2D PolygonCollider { get => polygonCollider;  }


    #endregion

    /// <summary> Sets up the Goal based on provided puzzle parameters 
    /// </summary>
    public void Set(FTM_Goal ftm_goal)
    {
        //-- Set based on provide puzzle parameters
        transform.localPosition = CCC.Convert.FLOAT_TO_VECTOR3(ftm_goal.Position);
        transform.localEulerAngles = CCC.Convert.FLOAT_TO_VECTOR3(ftm_goal.Rotation);
        rt.sizeDelta = CCC.Convert.FLOAT_TO_VECTOR2(ftm_goal.GoalSize);


        //-- Convert float[][] to Vector2[] and set the collider shape
        List<Vector2> newPath = new List<Vector2>();
        foreach(var path in ftm_goal.ColliderPath)
        {
            newPath.Add(CCC.Convert.FLOAT_TO_VECTOR2(path));
        }
        polygonCollider.SetPath(0, newPath.ToArray());

        //-- Goal colors
        box.color = CCC.Color.GoalYellow();
        glow.color = CCC.Color.GoalYellow();
    }

    private void Awake()
    {
        if(ftmmanager == null)
        { ftmmanager = FindObjectOfType<FTMManager>(); }

        if(rt == null)
        { rt = GetComponent<RectTransform>(); }

        if(polygonCollider == null)
        { polygonCollider = GetComponent<PolygonCollider2D>(); }

        if(rb2D == null)
        { rb2D = GetComponent<Rigidbody2D>(); }
        rb2D.isKinematic = true;

        if(box == null)
        { box = GetComponent<LeanBox>(); }
        
        if(glow == null)
        { 
            var boxes = GetComponentsInChildren<LeanBox>();
            foreach(var comp in boxes)
            {
                if(comp.name.Contains("Goal Glow"))
                {
                    glow = comp;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ftmmanager.FTMSuccess();
    }

    private void OnEnable()
    {
        var glowColor = glow.color;
        glowColor.a = glowMin;
        glow.color = glowColor;
        DOTween.ToAlpha(()=> glow.color, x=> glow.color = x, glowMax, glowSpeed).SetId("Goal Glow").SetLoops(-1, LoopType.Yoyo);

    }

    private void OnDisable()
    {
        DOTween.Kill("Goal Glow");
    }

    [ResponsiveButtonGroup(), Button(ButtonSizes.Medium), GUIColor(0, 0.75f, 0, 1), PropertySpace(SpaceBefore=50)]
    [PropertyOrder(0)]
    private void SetCollider()
    {

        Vector2[] colPath = new Vector2[] {
            new Vector2(rt.sizeDelta.x/2, rt.sizeDelta.y/2),
            new Vector2(-rt.sizeDelta.x/2, rt.sizeDelta.y/2),
            new Vector2(-rt.sizeDelta.x/2, -rt.sizeDelta.y/2),
            new Vector2(rt.sizeDelta.x/2, -rt.sizeDelta.y/2)
        };

        polygonCollider.SetPath(0,colPath);
    }

    [ResponsiveButtonGroup(), Button(ButtonSizes.Medium), GUIColor(0.75f, 0, 0, 1), PropertySpace(SpaceAfter=50)]
    [PropertyOrder(0)]
    private void AttachComponents()
    {
        if(ftmmanager == null)
        { ftmmanager = FindObjectOfType<FTMManager>(); }

        if(rt == null)
        { rt = GetComponent<RectTransform>(); }

        if(polygonCollider == null)
        { polygonCollider = GetComponent<PolygonCollider2D>(); }

        if(rb2D == null)
        { rb2D = GetComponent<Rigidbody2D>(); }
        rb2D.isKinematic = true;

        if(box == null)
        { box = GetComponent<LeanBox>(); }
        
        if(glow == null)
        { 
            var boxes = GetComponentsInChildren<LeanBox>();
            foreach(var comp in boxes)
            {
                if(comp.name.Contains("Goal Glow"))
                {
                    glow = comp;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
    }

}