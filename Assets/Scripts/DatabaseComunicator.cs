using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class DatabaseComunicator : MonoBehaviour
{
    public int queryID;

    [SerializeField] private TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7270/api/Trivia/" + queryID);
        Debug.Log("waiting for response");
        yield return www.SendWebRequest();
        Debug.Log("got response");

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            text.text = www.downloadHandler.text;
            //UpdateQuestionUI(www.downloadHandler.text);
        }

    }
}
