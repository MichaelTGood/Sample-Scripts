using Kkachi.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MCQManager : MonoBehaviour
{
    #region Variables

        [SerializeField] private MCQ thisPuzzle;

        [Header("")]
        [SerializeField] private TextMeshProUGUI question;
        [SerializeField] private List<MCQButton> buttons = new List<MCQButton>();

        //-- Answer Button layout
        [SerializeField] private VerticalLayoutGroup list;
        [SerializeField] private int buttonSpacing_4Answers = 50;
        [SerializeField] private int buttonSpacing_5Answers = 33;


        [SerializeField] private PuzzleManager puzzleManager;
        private static readonly System.Random rand = new System.Random();

    #endregion

    private void Awake()
    {
        puzzleManager = GameObject.FindObjectOfType<PuzzleManager>();
        thisPuzzle = puzzleManager.CurrentMCQ;

        //-- Build Puzzle
        question.text = thisPuzzle.Question;

        //-- Instantiate button for correct answer 
        var correctButton = buttons.NextFromPool();
        correctButton.Set(thisPuzzle.CorrectAnswer, true);

        //-- Instantiate buttons for incorrect answers
        foreach (var answer in thisPuzzle.IncorrectOptions)
        {
            var instance = buttons.NextFromPool();
            instance.Set(answer, false);
        }

        //-- Space the buttons in the LayoutGroup
        if (thisPuzzle.IncorrectOptions.Length + 1 <= 4)
            { list.spacing = buttonSpacing_4Answers; }
        else
            { list.spacing = buttonSpacing_5Answers; }

        RandomizeButtons(thisPuzzle.IncorrectOptions.Length + 1);


    }

    private void RandomizeButtons(int buttonCount)
    {
        int limit = rand.Next(3, 10);

        for(int i = 0; i <= limit; i++)
        {
            var button = list.transform.GetChild(rand.Next(0,buttonCount));

            button.SetAsFirstSibling();
        }

    }

    public void UnloadThisScene(MCQButton answer)
    {
        foreach(var button in buttons)
            { button.gameObject.SetActive(false); }

        if(answer.CorrectAnswer)
            { puzzleManager.UnloadPuzzle(true); }
        else
            { puzzleManager.UnloadPuzzle(false); }
    }

}