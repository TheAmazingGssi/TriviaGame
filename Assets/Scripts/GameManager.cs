using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    private string playerName = "john";
    [SerializeField] TextMeshProUGUI nameInput;

    private int playerId = -1;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async void StartGameButton()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7106/api/Trivia/AddPlayer_" + nameInput);
        Debug.Log("waiting for response");
        await www.SendWebRequest();
        Debug.Log("got response");

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            playerId = int.Parse(www.downloadHandler.text);
            Debug.Log(playerId);
        }
    }

    public void ChangeName(string name)
    {
        playerName = name;
    }    
}
