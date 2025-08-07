using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseComunicator : MonoBehaviour
{
    public int queryID;

    private Question question;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(GetText());
        GetText();
        foreach (var button in QuizUIManager.Instance.buttons)
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
            QuizUIManager.Instance.SetQuestionText(question);
        }

    }

    private void OnButtonClick(int id)
    {
        //pull information of the other player from server
        bool enemyIsStillStrategizing = true;

        if (enemyIsStillStrategizing)
        {
            QuizUIManager.Instance.OpenWaitingPanel();
            //raise flag to start polling for if the enemy answered
        }
        else
        {
            queryID++;
            GetText();
        }

        if (id == question.correctAnswer - 1)
        {
            // success
            //add score to player
            Debug.Log("GREAT SUCCESS");
        }
        else
        {
            //failure
            Debug.Log("WRONG");
        }
        //push data into the server
    }
}
