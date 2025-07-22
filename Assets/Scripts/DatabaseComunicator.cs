using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseComunicator : MonoBehaviour
{
    public int queryID;

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private ButtonHandler[] buttons;

    private Question question;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(GetText());
        GetText();
        foreach (var button in buttons)
        {
            button.OnButtonClick += OnButtonClick;
        }
    }

    async void GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7106/api/Trivia/GetQuestion_" + queryID);
        Debug.Log("waiting for response");
        await www.SendWebRequest();
        Debug.Log("got response");

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            
            question = JsonUtility.FromJson<Question>(www.downloadHandler.text);
            Debug.Log(question);
            questionText.text = question.questionText;
            buttons[0].text.text = question.answer1Text;
            buttons[1].text.text = question.answer2Text;
            buttons[2].text.text = question.answer3Text;
            buttons[3].text.text = question.answer4Text;

           
            //UpdateQuestionUI(www.downloadHandler.text);
        }

    }

    private void OnButtonClick(int id)
    {
        if (id == question.correctAnswer - 1)
        {
            // success
            Debug.Log("GREAT SUCCESS");
        }
        else
        {
            //failure
            Debug.Log("WRONG");
        }
    }
}
