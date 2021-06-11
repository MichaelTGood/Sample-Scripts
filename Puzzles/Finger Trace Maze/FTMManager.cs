using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using Lean.Gui;
using Lean.Common;

public class FTMManager : MonoBehaviour
{
    #region Variables  

        #region This Puzzle

            [VerticalGroup("FTMManager")]
            [TitleGroup("FTMManager/This Puzzle", null, TitleAlignments.Centered, true, true, false, 0)]
            [HideLabel]
            [SerializeField] private FTM thisPuzzle;
            
            [ResponsiveButtonGroup("FTMManager/InspectorButtons", UniformLayout = true), Button(ButtonSizes.Large), GUIColor(0, 0.75f, 0, 1), PropertySpace(SpaceBefore=25)]
            [PropertyOrder(1)]
            private void LoadPuzzleInEditor()
            { CreatePuzzle(); }


    [ResponsiveButtonGroup("FTMManager/InspectorButtons", UniformLayout = true), Button(ButtonSizes.Large), GUIColor(.9f, 0, 0, 1), PropertySpace(SpaceAfter=25)]
            [PropertyOrder(1)]
            private void UnloadPuzzleInEditor()
            {

            }
        
        #endregion

        [TabGroup("Objects")]
        [SerializeField] private bool editorUsesBuildCanvas;
        
        [Header("Canvases")]
        [TabGroup("Objects"), PropertySpace]
        [SerializeField] private GameObject GameCanvas;
        [TabGroup("Objects")]
        [SerializeField] private GameObject BuildCanvas;
        [Header("Game Objects")]
        [TabGroup("Objects"), PropertySpace]
        [SerializeField] private FTM_Hero_Obj hero;
        [TabGroup("Objects")]
        [SerializeField] private LineRenderer lineRenderer;
        [TabGroup("Objects")]
        [SerializeField] private Image background;
        [TabGroup("Objects")]
        [SerializeField] private Camera puzzleCamera;
        [TabGroup("Objects")]
        [SerializeField] private FTM_WorldCollision_Obj worldCollider;
        [TabGroup("Objects")]
        [SerializeField] private ObstacleManager obstacleManager;
        [TabGroup("Objects")]
        [SerializeField] private FTM_Obstacle_Obj obstaclePrefab;
        [TabGroup("Objects")]
        [SerializeField] private Transform goalParent;
        [TabGroup("Objects")]
        [SerializeField] private FTM_Goal_Obj goalPrefab;
        [TabGroup("Objects")]
        [SerializeField] private TextMeshProUGUI timerDisplay;
        [TabGroup("Objects")]
        [SerializeField] private TextMeshProUGUI attemptsDisplay;
        [TabGroup("Objects")]
        [SerializeField] private LeanButton resetButton;
        [TabGroup("Objects")]
        [SerializeField] private CanvasGroup resetButtonCanvas;
        [TabGroup("Objects")]
        [SerializeField] private PuzzleRules puzzleRules;

        [TabGroup("Variables")]
        [SerializeField] private Vector3 heroPos;
        [TabGroup("Variables")]
        [SerializeField] private Vector3 heroRot;
        [TabGroup("Variables")]
        [SerializeField] private List<FTM_Obstacle> obstacles = new List<FTM_Obstacle>();

        [TabGroup("Variables")]
        [SerializeField] private Color successGreen = Color.green;
        [TabGroup("Variables")]
        [SerializeField] private Color goalYellow = new Color(0.85f, 0.7833f, 0.0133f, 1);
        [TabGroup("Variables")]
        [SerializeField] private Color failRed = new Color(0.66f, 0, 0, 1);

        [Space]
        [HideInInspector]
        [SerializeField] private PuzzleManager puzzleManager;
        public PuzzleManager PuzzleManager { get => puzzleManager; set => puzzleManager = value; }

        private bool attemptingPuzzle = false;
        private float timer;
        private float timerBounce = 0f;
        private float timerBounceLimit = .85f;
        private bool timerColorChange = false;
        private int attempts;
        private System.Random rand = new System.Random();





    #endregion

    private void Awake()
    {
        puzzleManager = GameObject.FindObjectOfType<PuzzleManager>();
        thisPuzzle = puzzleManager.currentFTM;

    }

    private IEnumerator Start()
    {
        CreatePuzzle();
        
        //-- Moved to CreatePuzzle > CreateAndSetObstacles()
        // SetObstacles();
        
        ResetButtonOn(false);

        Debug.Log("Cheap Startup in FTMMAnager");
        timer = thisPuzzle.Timer;
        timerDisplay.text = (timer).ToString("0.0");
        attempts = thisPuzzle.Attempts;
        UpdateAttemptCount();
        
        yield return null;
    }

    private void Update()
    {
        if (attemptingPuzzle)
        {

            timer -= Time.deltaTime;
            timerBounce += Time.deltaTime;
            timerDisplay.text = (timer).ToString("0.0");
            
            //-- Grows/Shrinks the timer while it's counting down
            if (timerBounce > timerBounceLimit)
            {
                timerDisplay.transform.DOPunchScale(new Vector3(.5f, .5f, 0), 0.3f, 0, 0);
                timerBounceLimit = 1f;
                timerBounce = 0f;
            }

            //-- Fades timer to red as it nears the end.
            if (timer < 1.5f && timerColorChange == false)
            {
                timerColorChange = true;
                timerDisplay.DOColor(failRed, 1f).SetId("timerCounter");
            }

            //-- If timer runs out:
            if (timer <= 0)
            {
                timerDisplay.text = "0.0";
                AttemptFailed();
            }
        }
    }



    /// <summary> Resets the FTM Puzzle for the next attempt.
    /// </summary>
    public void ResetPuzzle()
    {
        if (attempts > 0)
        {
            //-- Reset Timer
            timer = thisPuzzle.Timer;
            timerDisplay.text = thisPuzzle.Timer.ToString("0.0");
            timerDisplay.color = Color.white;
            timerBounce = 0f;
            timerBounceLimit = .85f;
            timerColorChange = false;

            //-- Reset Hero
            var rtp = hero.GetComponent<LeanRotateToPosition>();
            rtp.enabled = false;
            hero.Set(thisPuzzle.Data.Hero);
            rtp.enabled = true;

            var ls = hero.GetComponent<Lean.Common.LeanSelectable>();
            ls.enabled = true;

            //-- Reset Line
            lineRenderer.positionCount = 0;

            //-- Pick and Place Obstacles
            SetObstacles();

            ResetButtonOn(false);
        }
    }

    /// <summary> Runs the Hero unit, after the player has input a path to follow.
    /// </summary>
    public void AttemptFTM()
    {
        if (attempts > 0)
        { 
            attemptingPuzzle = true; 
            var ls = hero.GetComponent<Lean.Common.LeanSelectable>();
            ls.enabled = false;
        }
    }

    #region  CONDITIONS

        /// <summary> If the player failed the current attempt.
        /// </summary>
        public void AttemptFailed()
        {
            //-- Stop the puzzle and updates
            attemptingPuzzle = false;

            //-- Stop Timer from Reddening
            if (DOTween.IsTweening("timerCounter"))
            {
                DOTween.Kill("timerCounter");
            }

            //-- Stop the hero from moving.
            var follow = hero.GetComponent<LeanFollow>();
            follow.enabled = false;

            //-- Update attempts
            attempts--;

            //-- Animates the Attempt counter update
            Sequence attemptChange = DOTween.Sequence();
            attemptChange.AppendInterval(0.5f);
            attemptChange.Append(attemptsDisplay.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0), 1, 0, 0));
            attemptChange.InsertCallback(0.8f, UpdateAttemptCount);

            //-- Check for remaining attempts
            if (attempts > 0)
            {
                //-- Check for reason of failure.
                if (timer <= 0)
                {
                    puzzleRules.SetPuzzleRules("Your directions were too slow! Backup couldn't make it in time!", "", "<color=\"red\" />Too Long!");
                }
                else if (hero.GetComponent<LeanFollow>().Positions.Count <= 0)
                {
                    puzzleRules.SetPuzzleRules("Your directions were not enough! They were not able to find their way all the way to the suspect!", "","<color=\"red\" />Not Enough Information!");
                }
                else
                {
                    puzzleRules.SetPuzzleRules("You directed backup into traffic! They can't get through!", "", "<color=\"red\" />Blocked!");
                }

                //-- Turn on the button to reset the puzzle.
                ResetButtonOn(true);
            }
            else
            {
                attemptsDisplay.color = failRed;
                FTMFailed();
            }

        }

        /// <summary> The fail state if the player never got to the goal before attempts ran out.
        /// </summary>
        public void FTMFailed()
        {
            // TODO Fail State
            Debug.Log("FTM PUZZLE FAILED -- NOT YET IMPLIMENTED");

            puzzleRules.SetPuzzleRules("The suspect got away, because you were not able to give clear enough directions.", "You did not get backup \nthere in time!", "<color=\"red\">FAILED</color>");
            puzzleRules.CloseButton.onClick.AddListener(() => puzzleManager.UnloadPuzzle(false));
        }

        /// <summary> The success state if the player got to the goal in time.
        /// </summary>
        public void FTMSuccess()
        {
            // TODO Success State
            attemptingPuzzle = false;
            hero.GetComponent<LeanFollow>().enabled = false;
            DOTween.Kill("timerCounter");
            Debug.Log("FTM PUZZLE SUCCESS!");
            puzzleRules.SetPuzzleRules("Because of your clear directions, you managed to get back up there in time, and they apprehended the suspect! Good job!", "You got 'em!", "<color=\"green\">SUCCESS!</color>");
            puzzleRules.CloseButton.onClick.AddListener(() => puzzleManager.UnloadPuzzle(true));

        }

        /// <summary> Displays or removes the Reset Puzzle button.
        /// </summary>
        /// <param name="value"> Do you want it on or off?!</param>
        private void ResetButtonOn(bool value)
        {
            resetButtonCanvas.alpha = Convert.ToInt32(value);
            resetButtonCanvas.blocksRaycasts = value;
            resetButtonCanvas.interactable = value;
        }

        /// <summary> Updates the displayed number of attempts to match the FTMManger's count.
        /// </summary>
        private void UpdateAttemptCount()
        {
            attemptsDisplay.text = attempts.ToString();
        }

        /// <summary> Randomly picks and places obstacles. It will destroy any previously existing obstacles.
        /// </summary>
        private void SetObstacles()
        {
            // -- Disable any previously used obstacles
            foreach (var child in obstacleManager.Obstacles)
            {
                child.gameObject.SetActive(false);
            }

            //-- List to keep track of randomly picked number/obstacles.
            List<int> picked = new List<int>();

            for (int i = 0; i < thisPuzzle.ObstacleCount; i++)
            {
                //-- Pick a random number/obstacle.
                int randNo = rand.Next(obstacleManager.Obstacles.Count);
                Debug.Log($"Random obs number: {randNo}");

                while (picked.IndexOf(randNo) != -1)
                {
                    Debug.Log("Repicking the random number");
                    randNo = rand.Next(obstacleManager.Obstacles.Count);
                    Debug.Log($"Random obs number: {randNo}");
                }
                //-- Add random number to list, to check against in following random number picks.
                picked.Add(randNo);

                //-- Set and activate the obstacle.
                // obstacleManager.Obstacles[i].Set(obstacleManager.Obstacles[randNo]);
                obstacleManager.Obstacles[randNo].gameObject.SetActive(true);
                
                
            }
        }
        
    #endregion

    #region Puzzle Prep

        private void CreatePuzzle()
        {
            StartCoroutine(SetBG());

            StartCoroutine(SetHero());

            StartCoroutine(CreateAndSetObstacles());

            ParsePuzzleData();

            //-- Sets Timer and Displayed Timer
            timerDisplay.text = thisPuzzle.Timer.ToString("0.0");
            timerDisplay.color = Color.white;
            timer = thisPuzzle.Timer;

            //-- Sets Attempts and Displayed Attempts
            attemptsDisplay.text = thisPuzzle.Attempts.ToString();
            attemptsDisplay.color = Color.white;
            attempts = thisPuzzle.Attempts;

            //-- Sets and Displays Rules
            puzzleRules.SetPuzzleRules(thisPuzzle.Rules, thisPuzzle.Subheader, thisPuzzle.Header, true);
            Debug.Log("Add a \"<color=\"green\">While()</color>\" in PuzzlePrep to check for downloaded Textures.");

        }

        private IEnumerator SetBG()
        {
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(CCC.Azure.Images + thisPuzzle.BG.Address + puzzleManager.DataManager.GameManager.ImagesSAS);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            { Debug.LogError(uwr.error);}
            else
            {
                var image = DownloadHandlerTexture.GetContent(uwr);
                
                background.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f));
            }

            yield return null;
        }

        private IEnumerator SetHero()
        {
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(CCC.Azure.Images + thisPuzzle.Hero.Address + puzzleManager.DataManager.GameManager.ImagesSAS);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            { Debug.LogError(uwr.error);}
            else
            {
                var image = DownloadHandlerTexture.GetContent(uwr);
                
                hero.Set(thisPuzzle.Data.Hero, image);
            }
            yield return null;
        }

        private IEnumerator CreateAndSetObstacles()
        {
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(CCC.Azure.Images + thisPuzzle.Obs.Address + puzzleManager.DataManager.GameManager.ImagesSAS);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            { Debug.LogError(uwr.error);}
            else
            {
                var image = DownloadHandlerTexture.GetContent(uwr);

                //-- Make sure the obstacle list is empty
                // if(0 < obstacleManager.Obstacles.Count)
                // {
                //     foreach(var child in obstacleManager.Obstacles)
                //     {
                //         Destroy(child.gameObject);
                //     }
                //     obstacleManager.Obstacles.Clear();
                // }

                // //-- Make sure there are no pre-existing obstacles/children
                // if(0 < obstacleManager.GetComponentsInChildren<Transform>().Length)
                // {
                //     foreach(var child in obstacleManager.GetComponentsInChildren<Transform>())
                //     {
                //         if(child != obstaclePrefab && child != obstacleManager)
                //         { Destroy(child.gameObject); }
                //     }
                // }

                foreach(var ob in thisPuzzle.Data.Obstacles)
                {
                    var ob_obj = Instantiate(obstaclePrefab, obstacleManager.transform);
                    ob_obj.Set(ob, image);
                    obstacleManager.Obstacles.Add(ob_obj);
                }

                SetObstacles();
            }

        }
        
        /// <summary> Sets Camera, Goals, WorldCollision.
        /// </summary>
        private void ParsePuzzleData()
        {
            puzzleCamera.transform.localPosition = CCC.Convert.FLOAT_TO_VECTOR3(thisPuzzle.Data.Camera.Position);
            puzzleCamera.transform.localEulerAngles = CCC.Convert.FLOAT_TO_VECTOR3(thisPuzzle.Data.Camera.Rotation);
            puzzleCamera.nearClipPlane = thisPuzzle.Data.Camera.Clipping[0];
            puzzleCamera.farClipPlane = thisPuzzle.Data.Camera.Clipping[1];

            foreach(var goal in thisPuzzle.Data.Goals)
            {
                var goal_obj = Instantiate(goalPrefab, goalParent);
                goal_obj.gameObject.SetActive(true);
                goal_obj.Set(goal);
            }

            worldCollider.Set(thisPuzzle.Data.WorldCollision);


        }

    #endregion

}
