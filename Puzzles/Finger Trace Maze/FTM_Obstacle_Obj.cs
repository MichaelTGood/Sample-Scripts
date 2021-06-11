using System.Collections.Generic;
using Lean.Touch;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FTM_Obstacle_Obj : MonoBehaviour
{
    #region Variables
        [SerializeField] private FTMManager ftmManager;
        [SerializeField] private RectTransform rt;
        [SerializeField] private Image image;
        [SerializeField] private PolygonCollider2D polygonCollider;
        [SerializeField] private Rigidbody2D rb2D;

        public RectTransform Rt { get => rt;  }
    public PolygonCollider2D PolygonCollider { get => polygonCollider; }



    #endregion

    private void Awake()
    {
        if(ftmManager == null)
        { ftmManager = FindObjectOfType<FTMManager>(); }

        if(rt == null)
        { rt = GetComponent<RectTransform>(); }

        if(image == null)
        { image = GetComponent<Image>(); }   

        if(polygonCollider == null)
        { polygonCollider = GetComponent<PolygonCollider2D>(); }

        if(rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
        }
        rb2D.isKinematic = true;
    }

    /// <summary> Sets the obstacle up for use, and sets the image used.
    /// </summary>
    public void Set(FTM_Obstacle ftm_obstacle, Texture2D tex)
    {
        transform.localPosition = CCC.Convert.FLOAT_TO_VECTOR3(ftm_obstacle.Position);
        transform.localEulerAngles = CCC.Convert.FLOAT_TO_VECTOR3(ftm_obstacle.Rotation);
        transform.localScale = CCC.Convert.FLOAT_TO_VECTOR3(ftm_obstacle.Scale);
        rt.sizeDelta = CCC.Convert.FLOAT_TO_VECTOR2(ftm_obstacle.Size);

        //-- Clear all existing paths.
        polygonCollider.pathCount = 0;
        polygonCollider.pathCount = ftm_obstacle.ColliderPath.Length;

        //-- Interate through all the paths.
        for (int i = 0; i < ftm_obstacle.ColliderPath.Length; i++)
        {
            //-- A list for all the points on one path.
            List<Vector2> points = new List<Vector2>();

            //-- Foreach point on one path
            foreach(var point in ftm_obstacle.ColliderPath[i])
            {   
                //-- Convert point to Vector2 and add it to the list.
                points.Add(CCC.Convert.FLOAT_TO_VECTOR2(point));
            }
            //-- Set the full list to the collider.
            polygonCollider.SetPath(i, points.ToArray());
            
        }

        image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));;
    }

    /// <summary> Sets the obstacle up for use, assumes it already has the image.
    /// </summary>
    public void Set(FTM_Obstacle ftm_obstacle)
    {
        transform.localPosition = CCC.Convert.FLOAT_TO_VECTOR3(ftm_obstacle.Position);
        transform.localEulerAngles = CCC.Convert.FLOAT_TO_VECTOR3(ftm_obstacle.Rotation);
        transform.localScale = CCC.Convert.FLOAT_TO_VECTOR3(ftm_obstacle.Scale);
        rt.sizeDelta = CCC.Convert.FLOAT_TO_VECTOR2(ftm_obstacle.Size);

        //-- Clear all existing paths.
        polygonCollider.pathCount = 0;

        //-- Interate through all the paths.
        for (int i = 0; i < ftm_obstacle.ColliderPath.Length; i++)
        {
            //-- A list for all the points on one path.
            List<Vector2> points = new List<Vector2>();

            //-- Foreach point on one path
            foreach(var point in ftm_obstacle.ColliderPath[i])
            {   
                //-- Convert point to Vector2 and add it to the list.
                points.Add(CCC.Convert.FLOAT_TO_VECTOR2(point));
            }
            //-- Set the full list to the collider.
            polygonCollider.SetPath(i, points.ToArray());
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER in 2D!");
        ftmManager.AttemptFailed();
    }

    [ResponsiveButtonGroup(), Button(ButtonSizes.Medium), GUIColor(0.75f, 0, 0, 1), PropertySpace(SpaceAfter=50)]
    [PropertyOrder(0)]
    private void AttachComponents()
    {
        if(ftmManager == null)
        { ftmManager = FindObjectOfType<FTMManager>(); }

        if(rt == null)
        { rt = GetComponent<RectTransform>(); }

        if(image == null)
        { image = GetComponent<Image>(); }   

        if(polygonCollider == null)
        { polygonCollider = GetComponent<PolygonCollider2D>(); }

        if(rb2D == null)
        { rb2D = GetComponent<Rigidbody2D>(); }
        rb2D.isKinematic = true;
    }
}