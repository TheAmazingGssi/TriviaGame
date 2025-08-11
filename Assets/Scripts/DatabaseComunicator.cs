using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

static public class DatabaseComunicator
{
    static public int queryID = -1;
    static public Question ActiveQuestion;


    static async public Task<int> EnterSelfToDatabase(string nameInput)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7106/api/Trivia/AddPlayer_" + nameInput);
        await www.SendWebRequest();
        int myId = -1;

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            myId = int.Parse(www.downloadHandler.text);
        }
        return myId;
    }
    static async public Task<Question> GetQuestion()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7106/api/Trivia/GetQuestion_" + queryID);
        Debug.Log("waiting for response");
        await www.SendWebRequest();
        Debug.Log("got response");
        Question question = new Question();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            ActiveQuestion = JsonUtility.FromJson<Question>(www.downloadHandler.text);
            question = ActiveQuestion;
        }
        return question;
    }
    //the bool in this tuple represent another player with the active tag was found
    static async public Task<Tuple<bool, PlayerInformation>> CheckRival(int myID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7106/api/Trivia/GetWaitingPlayer_" + myID);
        await www.SendWebRequest();
        bool success = false;
        PlayerInformation rivalInfo = null;
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            rivalInfo = JsonUtility.FromJson<PlayerInformation>(www.downloadHandler.text);
            if(rivalInfo.Name != null)
                success = true;
        }
        return new Tuple<bool, PlayerInformation>(success, rivalInfo);
    }
    static async public void UpdateQuestionProgression(int myID, int currentQuestion)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7106/api/Trivia/UpdatePlayerQuestion_" + myID + "_" + currentQuestion);
        await www.SendWebRequest();
    }
    static async public void UpdatePlayerScore(int myID, int score, float thinkingTime)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7106/api/Trivia/UpdatePlayerScore_" + myID + "_" + score + "_" + thinkingTime);
        await www.SendWebRequest();
    }

}
