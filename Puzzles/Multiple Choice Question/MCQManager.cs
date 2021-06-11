using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MCQManager : MonoBehaviour
{
    #region Variables

        [SerializeField] private MCQ thisPuzzle;

        [Header("")]
        [SerializeField] private TextMeshProUGUI question;
        [SerializeField] private MCQButton buttonPrefab;

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
        var correctButton = Instantiate(buttonPrefab, list.transform);
        correctButton.name = thisPuzzle.CorrectAnswer;
        correctButton.TextObject.text = thisPuzzle.CorrectAnswer;
        correctButton.CorrectAnswer = true;

        //-- Instantiate buttons for incorrect answers
        foreach (var answer in thisPuzzle.IncorrectOptions)
        {
            var instance = Instantiate(buttonPrefab, list.transform);

            instance.name = answer;
            instance.TextObject.text = answer;
            instance.CorrectAnswer = false;
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
        if(answer.CorrectAnswer)
            { puzzleManager.UnloadPuzzle(true); }
        else
            { puzzleManager.UnloadPuzzle(false); }
    }

}