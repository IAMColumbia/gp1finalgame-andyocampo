using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int level;
    public static int movesRemaining;
    public static int matchGoal;
    public static float timeLeft;
    public static string message;
    private static bool gameStarted;
    Timer timer;

    [SerializeField]
    private Text LevelText, MovesRemainingText, MatchGoalText, TimerText, MessageText;

    [SerializeField]
    private Image messagePanel;
    private Color panelColor;

    [SerializeField] PlayerController playerController;
    MouseInput input;

    /// <summary>
    /// Set's up initial level.
    /// </summary>
    private static void SetupNewGame()
    {
        gameStarted = false;
        level = 1;
        movesRemaining = 20;
        matchGoal = 20;
        timeLeft = 60;
        message = "Complete the number of matches before you run out of moves or time runs out! Press Left Mouse Button to start.";
        Time.timeScale = 0;
    }

    void Awake()
    {
        SetupNewGame();
        timer = GetComponent<Timer>();
        input = playerController.GetComponent<MouseInput>();
        panelColor = messagePanel.color;
    }

    private void Start()
    {
        LevelText.text = "Level: " + level.ToString();
        MovesRemainingText.text = "Moves remaining: " + movesRemaining.ToString();
        MatchGoalText.text = "Matches: " + matchGoal.ToString();
        TimerText.text = "Time: ";
        MessageText.text = message;
    }

    void Update()
    {
        UpdateUI();
        StartGame();
    }

    /// <summary>
    /// Updates the text elements in the Scene.
    /// </summary>
    private void UpdateUI()
    {
        LevelText.text = "Level: " + level.ToString();
        MovesRemainingText.text = "Moves remaining: " + movesRemaining.ToString();
        MatchGoalText.text = "Matches: " + matchGoal.ToString();
        TimerText.text = timer.DisplayTimer(timeLeft);
        MessageText.text = message;
    }

    /// <summary>
    /// Starts the game and checks win/lose status.
    /// </summary>
    private void StartGame()
    {
        if(!gameStarted)
        {
            if (input.leftMouseButtonPressed)
            {
                gameStarted = true;
                messagePanel.color = Color.clear;
                message = "";
                playerController.isPlaying = true;
                Time.timeScale = 1;
            }
        }

        if(gameStarted)
        {
            CheckIfPlayerLose();
            CheckIfPlayerWin();
        }
    }

    /// <summary>
    /// Checks if the player has lost.
    /// </summary>
    private void CheckIfPlayerLose()
    {
        if(timeLeft <= 0)
        {
            messagePanel.color = panelColor;
            message = "You lose! Timer ran out. Press Left Mouse Button to restart.";
            playerController.isPlaying = false;
            Time.timeScale = 0;
            if(input.leftMouseButtonPressed)
            {
                SetupNewGame();
            }
        }

        if(movesRemaining <= 0 && matchGoal != 0)
        {
            messagePanel.color = panelColor;
            message = "You lose! No more moves. Press Left Mouse Button to restart.";
            playerController.isPlaying = false;
            if (!TileManager.Instance.isRefilling)
            { Time.timeScale = 0; }
            if (input.leftMouseButtonPressed)
            {
                SetupNewGame();
            }
        }
    }

    /// <summary>
    /// Checks if the player has won.
    /// </summary>
    private void CheckIfPlayerWin()
    {
        if(matchGoal <= 0 && timeLeft > 0 && movesRemaining >= 0)
        {
            messagePanel.color = panelColor;
            message = "You win! Press Left Mouse Button to continue.";
            playerController.isPlaying = false;
            if(!TileManager.Instance.isRefilling)
            { Time.timeScale = 0; }
            if (input.leftMouseButtonPressed)
            { ChangeLevel(); }
        }
    }

    /// <summary>
    /// Changes level.
    /// </summary>
    private void ChangeLevel()
    {
        switch(level)
        {
            case 1:
                SetNewLevel(20, 25, 50); 
                break;
            case 2:
                SetNewLevel(25, 30, 30);
                break;
            case 3:
                SetNewLevel(30, 35, 25);
                break;
            case 4:
                SetNewLevel(35, 40, 15);
                break;
            case 5:
                EndGame();
                break;
        }
        level++;
    }

    /// <summary>
    /// Sets up a new level.
    /// </summary>
    /// <param name="moves">Number of moves left in level.</param>
    /// <param name="matches">Number of matches to get in level.</param>
    /// <param name="time">Seconds left in new level.</param>
    private void SetNewLevel(int moves, int matches, float time)
    {
        movesRemaining = moves;
        matchGoal = matches;
        timeLeft = time;
        messagePanel.color = Color.clear;
        message = "";
        playerController.isPlaying = true;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Ends the game when player has reached level 6
    /// </summary>
    private void EndGame()
    {
        messagePanel.color = Color.clear;
        message = "You've beat the game! Press Left Mouse Button to exit.";
        if(input.leftMouseButtonPressed)
        {
    #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
    #elif UNITY_WEBPLAYER
             Application.OpenURL(webplayerQuitURL);
    #else
             Application.Quit();
    #endif
        }
    }
}
