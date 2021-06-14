using UnityEngine;
using TMPro;

public class MCQButton : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textObject;

    [SerializeField] private bool correctAnswer = false;

    // public TextMeshProUGUI TextObject { get => textObject; }
    public bool CorrectAnswer { get => correctAnswer; set => correctAnswer = value; }

    public MCQButton(TextMeshProUGUI textObject, bool correctAnswer)
    {
        this.textObject = textObject;
        this.correctAnswer = correctAnswer;
    }

    public void Set(string answer, bool correctAnswer)
    {
        this.name = answer;
        textObject.text = answer;
        this.correctAnswer = correctAnswer;
        this.gameObject.SetActive(true);
    }


}