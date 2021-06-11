/// <summary> The type for a Multiple Choice Question
/// </summary>
[System.Serializable]
public class MCQ
{
        public string Question;
        public string CorrectAnswer;
        public string[] IncorrectOptions;

    public MCQ() {}

    public MCQ(string question, string correctAnswer, string[] incorrectOptions)
    {
        Question = question;
        CorrectAnswer = correctAnswer;
        IncorrectOptions = incorrectOptions;
    }

}