using System;
using Unity.VisualScripting;

[System.Serializable]
public class Question
{
    public string questionText;
    public string answer1Text;
    public string answer2Text;
    public string answer3Text;
    public string answer4Text;
    public int correctAnswer;

    override public string ToString()
    {
        return $"{questionText} {answer1Text} {answer2Text} {answer3Text} {answer4Text} {correctAnswer}";
    }
}
