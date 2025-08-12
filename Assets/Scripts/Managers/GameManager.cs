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
    bool waitingForRivalToProgress = false;
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
            Debug.Log("Looking for rival");
            lastCheck = Time.time;
            LookForRival();
        }
        if(waitingForRivalToProgress && Time.time >= lastCheck + checkTiming)
        {
            Debug.Log("Waiting for rival");
            lastCheck = Time.time;
            WaitForRival();
        }
    }

    //Main Menu methods
    async void LookForRival()
    {
        Tuple<bool, PlayerInformation> rivalTuple = await DatabaseComunicator.CheckRival(playerId, 0);
        if (rivalTuple.Item1)
        {
            StartQuiz();
            lookingForNewRival = false;
        }
    }
    async void WaitForRival()
    {
        Tuple<bool, PlayerInformation> rivalTuple = await DatabaseComunicator.CheckRival(playerId, DatabaseComunicator.queryID);
        if (rivalTuple.Item1)
        {
            GetNewQuestion();
            QuizUIManager.Instance.OpenQuestionPanel();
            waitingForRivalToProgress = false;
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
        
        Tuple<bool, PlayerInformation> rivalTuple = await DatabaseComunicator.CheckRival(playerId, 0);
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
        int totalQuestionAmount = await DatabaseComunicator.GetQuestionAmount();
        DatabaseComunicator.UpdateQuestionProgression(playerId, DatabaseComunicator.queryID); //We need to progress the current question either way
        //Check if the game is over
        if (totalQuestionAmount < DatabaseComunicator.queryID)
        {
            sceneLoader.GoToNextScene();
            return;
        }

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
        Tuple<bool, PlayerInformation> rivalInfo = await DatabaseComunicator.CheckRival(playerId, DatabaseComunicator.queryID);
        if (!rivalInfo.Item1)
        {
            DatabaseComunicator.TurnOnWaitingFlag(playerId);
            QuizUIManager.Instance.OpenWaitingPanel();
            waitingForRivalToProgress = true;
        }
        else
        {
            GetNewQuestion();
        }

    }
    
    //end screen methods
    public async void PrepareWinLoseText() //will be called on the end scene 
    {
        int questionAmount = await DatabaseComunicator.GetQuestionAmount();
        Tuple<bool, PlayerInformation> rivalInfo = await DatabaseComunicator.CheckRival(playerId, questionAmount);
        if (rivalInfo.Item1)
        {
            string text;

            if (playerScore == rivalInfo.Item2.Score) // if they both took the exact same time down to the float level theyre both winners in my book
            {
                if (answerTime <= rivalInfo.Item2.ThinkingTime)
                    text = $"A true test of mind.\n\nYou and the enemy both answered {playerScore} questions correctly, though you did so faster.\nYour total time was {answerTime.ToString("0.00")} while theirs was {rivalInfo.Item2.ThinkingTime.ToString("0.00")}";
                else
                    text = $"A worthy adversary, your next match will be legendary!\n\nYou both answered {playerScore} questions correctly, though they did so quicker than you\nYour total time was {answerTime.ToString("0.00")} while theirs was {rivalInfo.Item2.ThinkingTime.ToString("0.00")}";
            }
            else if (playerScore > rivalInfo.Item2.Score)
                text = $"You have crushed your enemy and humiliated their intelect. You answered {playerScore} question correctly while they only managed {rivalInfo.Item2.Score}";
            else
                text = $"The enemy has bested you. They answered {rivalInfo.Item2.Score} questions correctly, while you only did {playerScore}. Next time they will not be so lucky!";

            EndUIManager.Instance.SetText(text);
        }
    }

    //this method is currently not being called anywhere
    //It should happen whenever the game stops, perhaps it would have worked on the destructor of this class?
    //We sadly dont have time to research it, though we do know we have to deactivate the data at the end of the game
    private void ExitGame() 
    {
        DatabaseComunicator.DeactivateSelf(playerId);
    }
}
