using System.Collections.Generic;
using Collider2DOptimization;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonColliderOptimizer))]
public class FTM_WorldCollision_Obj : MonoBehaviour
{
    #region Variables

        [SerializeField] private FTMManager fTMManager;
        [SerializeField] private PolygonCollider2D polygonCollider;
        [SerializeField] private Rigidbody2D rb2D;

        public PolygonCollider2D PolygonCollider { get => polygonCollider; }

    #endregion

    private void Awake()
    {
        if(fTMManager == null)
        { fTMManager = FindObjectOfType<FTMManager>(); }

        if(polygonCollider == null)
        { polygonCollider =  GetComponent<PolygonCollider2D>(); }       

        if(rb2D == null)
        { rb2D = GetComponent<Rigidbody2D>(); }
        rb2D.isKinematic = true;
    }

    public void Set(FTM_WorldCollision data)
    {
        transform.localPosition = CCC.Convert.FLOAT_TO_VECTOR3(data.Position);
        transform.localScale = CCC.Convert.FLOAT_TO_VECTOR3(data.Scale);
        
        //-- Clear all existing paths.
        polygonCollider.pathCount = 0;
        polygonCollider.pathCount = data.ColliderPath.Length;

        for (int i = 0; i < data.ColliderPath.Length; i++)
        {
            List<Vector2> points = new List<Vector2>();

            foreach(var point in data.ColliderPath[i])
            {
                points.Add(CCC.Convert.FLOAT_TO_VECTOR2(point));
            }
            polygonCollider.SetPath(i, points.ToArray());
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER in 2D!");
        fTMManager.AttemptFailed();
        
    }

    [ResponsiveButtonGroup(), Button(ButtonSizes.Medium), GUIColor(0.75f, 0, 0, 1), PropertySpace(SpaceAfter=50)]
    [PropertyOrder(0)]
    private void AttachComponents()
    {
        if(fTMManager == null)
        { fTMManager = FindObjectOfType<FTMManager>(); }

        if(rb2D == null)
        { rb2D = GetComponent<Rigidbody2D>(); }
        rb2D.isKinematic = true;

        if(polygonCollider == null)
        { polygonCollider =  GetComponent<PolygonCollider2D>(); }

        GetComponent<PolygonColliderOptimizer>().tolerance = 0.1f;
    }

}
