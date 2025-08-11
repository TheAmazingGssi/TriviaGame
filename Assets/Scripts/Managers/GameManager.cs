using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance;

    //Checking other players things
    bool lookingForNewRival = false;
    [SerializeField] float checkTiming;
    float lastCheck = 0;

    //refrences
    [SerializeField] SceneLoader sceneLoader;

    //variables
    private int playerId = -1;
    float startOfThinkingTime = 0;
    int playerScore = 0;
    float answerTime = 0;

    //Monobehaviour
    private void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if(lookingForNewRival && Time.time >= lastCheck + checkTiming)
        {
            lastCheck = Time.time;
            LookForRival();
            Debug.Log("Checking for an enemy");
        }
    }

    //Main Menu methods
    async void LookForRival()
    {
        Tuple<bool, PlayerInformation> rivalTuple = await DatabaseComunicator.CheckRival(playerId);
        if (rivalTuple.Item1)
        {
            StartQuiz();
        }
    }
    public async void ReadyPlayer()
    {
        playerId = await DatabaseComunicator.EnterSelfToDatabase(MainMenuUIManager.Instance.playerName);
        if (playerId == -1)
        {
            Debug.LogError("Something went wrong");
            return;
        }
        
        Tuple<bool, PlayerInformation> rivalTuple = await DatabaseComunicator.CheckRival(playerId);
        if (rivalTuple.Item1)
        {
            StartQuiz();
        }
        else
        {
            MainMenuUIManager.Instance.OpenWaitingPanel();
            lookingForNewRival = true;
        }
    }  
    private void StartQuiz()
    {
        DatabaseComunicator.UpdateQuestionProgression(playerId, 1);
        DatabaseComunicator.queryID = 0; //getting new question will raise it to 1 before getting the info.
                                         //The first get new question is called on QuizUIManager's Start()
                                         //so it would be guranteed to happen in the loaded new scene
        sceneLoader.GoToNextScene();
    }

    //In game methods
    public async void GetNewQuestion()
    {
        DatabaseComunicator.queryID++;
        Question question = await DatabaseComunicator.GetQuestion();
        QuizUIManager.Instance.SetQuestionText(question);
        startOfThinkingTime = Time.time;
    }
    public async void OnAnswerClick(int id)
    {
        answerTime += Time.time - startOfThinkingTime;
        if (id == DatabaseComunicator.ActiveQuestion.correctAnswer - 1)
        {
            // success
            playerScore++;
        }

        //push data into the server
        DatabaseComunicator.UpdatePlayerScore(playerId, playerScore, answerTime);

        //pull information of the other player from server
        Tuple<bool, PlayerInformation> rivalInfo = await DatabaseComunicator.CheckRival(playerId);

        if (rivalInfo.Item1)
        {
            QuizUIManager.Instance.OpenWaitingPanel();
            //raise flag to start polling for if the enemy answered
        }
        else
        {
            GetNewQuestion();
        }

    }
}
