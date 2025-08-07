using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuizUIManager : MonoBehaviour
{
    public static QuizUIManager Instance;
    [Header("Question Panel")]
    [SerializeField] GameObject questionPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    public ButtonHandler[] buttons;
    [Header("Waiting Panel")]
    [SerializeField] GameObject waitingPanel;


    private void Start()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        
    }
    public void OpenWaitingPanel()
    {
        questionPanel.SetActive(false);
        waitingPanel.SetActive(true);
    }
    public void OpenQuestionPanel()
    {
        waitingPanel.SetActive(false);
        questionPanel.SetActive(true);
    }

    public void SetQuestionText(Question question)
    {
        questionText.text = question.questionText;
        buttons[0].text.text = question.answer1Text;
        buttons[1].text.text = question.answer2Text;
        buttons[2].text.text = question.answer3Text;
        buttons[3].text.text = question.answer4Text;
    }
}
