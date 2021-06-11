using System.Collections.Generic;
using Lean.Common;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class FTM_Hero_Obj : MonoBehaviour
{
    #region Variables

        [SerializeField] private FTMManager ftmManager;
        [SerializeField] private Image heroImage;
        [SerializeField] private BoxCollider2D col2D;

        [Header("Lean Follow")]
        [SerializeField] private Lean.Common.LeanFollow leanFollow;
        /// <summary> The amount to subtract from the Pos Z of each added point.
        /// </summary>
        [SerializeField] private float depthAdjust = 0.1f;

        [Space(25)]
        [Header("Light Flasher")]
        [Header("Time")]
        [SerializeField] private float flashInterval = 0.5f;
        [SerializeField] private float timer = 0;

        [Header("Light Objects")]
        [SerializeField] private List<LeanBox> frontLight;
        [SerializeField] private List<LeanBox> backLight;
        [SerializeField] private LeanBox roofLeftLight;     //-- Blue
        [SerializeField] private LeanBox roofRightLight;    //-- Red

        [Header("Light Colors")]
        [SerializeField] private Color red = new Color(1,0,0,0.2f);
        [SerializeField] private Color blue = new Color(0,0,1,0.6f);

        public BoxCollider2D Col2D { get => col2D; }


    #endregion

    public void Set(FTM_Hero ftm_hero, Texture2D tex)
    {
        transform.localPosition = CCC.Convert.FLOAT_TO_VECTOR3(ftm_hero.Position);
        transform.localEulerAngles = CCC.Convert.FLOAT_TO_VECTOR3(ftm_hero.Rotation);
        transform.localScale = CCC.Convert.FLOAT_TO_VECTOR3(ftm_hero.Scale);
        col2D.size = CCC.Convert.FLOAT_TO_VECTOR2(ftm_hero.ColliderSize);
        heroImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public void Set(FTM_Hero ftm_hero)
    {
        transform.localPosition = CCC.Convert.FLOAT_TO_VECTOR3(ftm_hero.Position);
        transform.localEulerAngles = CCC.Convert.FLOAT_TO_VECTOR3(ftm_hero.Rotation);
        transform.localScale = CCC.Convert.FLOAT_TO_VECTOR3(ftm_hero.Scale);
        col2D.size = CCC.Convert.FLOAT_TO_VECTOR2(ftm_hero.ColliderSize);
    }

    private void Awake()
    {
        foreach(var light in frontLight)
        { light.color = red; }
        
        foreach(var light in backLight)
        { light.color = blue; }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= flashInterval)
        {
            if(frontLight[0].color == red)
            {

                foreach(var light in frontLight)
                { light.color = blue; }
                
                foreach(var light in backLight)
                { light.color = red; }

                roofLeftLight.enabled = true;
                roofRightLight.enabled = false;
            }
            else
            {
                foreach(var light in frontLight)
                { light.color = red; }
                
                foreach(var light in backLight)
                { light.color = blue; }

                roofLeftLight.enabled = false;
                roofRightLight.enabled = true;
            }


            timer -= flashInterval;
        }

    }

    public void AddPointToFollow(Vector3 newPoint)
    {
        newPoint.z -= depthAdjust;
        
        leanFollow.AddPosition(newPoint);
    }

}