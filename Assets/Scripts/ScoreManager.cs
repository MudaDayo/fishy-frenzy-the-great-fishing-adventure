using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public Text scoreP1Text;
    public Text scoreP2Text;
    public Text resultText;

    [SerializeField]
    float timer = 0;

    [SerializeField]
    float endGameTimer = 5;

    private int scoreP1 = 0;
    private int scoreP2 = 0;
    public int winningScore = 2; // Set the winning score

    private bool gameEnded = false; // Track if the game has ended

    void Start()
    {
        UpdateScoreTexts();
    }

    // Update the score texts to reflect the current scores
    void UpdateScoreTexts()
    {
        scoreP1Text.text = scoreP1.ToString();
        scoreP2Text.text = scoreP2.ToString();
    }

    // Increment the score for the given player
    public void IncrementScore(string playerTag)
    {
        if (gameEnded) return; 

        if (playerTag == "Player1")
        {
            Debug.Log("P1 + 1");
            scoreP1++;
        }
        else if (playerTag == "Player2")
        {
            scoreP2++;
            Debug.Log("P2 + 1");
        }
        UpdateScoreTexts();
        CheckWinCondition();
    }

    // Check if either player has reached the winning score
    void CheckWinCondition()
    {
        if (scoreP1 >= winningScore)
        {
            // Player 1 wins
            ShowResult("Player 1 Wins!", "Player 2 Loses!");
            ResetPlayersPosition();
            if (timer > endGameTimer)
            {
                gameEnded = true;
                
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                ResetPlayersPosition();
                
            }
            timer += Time.deltaTime;
        }
        else if (scoreP2 >= winningScore)
        {
            // Player 2 wins
            ShowResult("Player 2 Wins!", "Player 1 Loses!");
            ResetPlayersPosition();
            if (timer > endGameTimer)
            {
                gameEnded = true;
                
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                ResetPlayersPosition();
            }
            timer += Time.deltaTime;
        }
    }

    
    void ShowResult(string winText, string loseText)
    {
        resultText.text = winText;
        // Optionally, you can add some delay or animation before resetting the game
        StartCoroutine(ResetGame());
    }

    // Reset the game (optional)
    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(3); // Wait for 3 seconds
        // Reset the scores and UI texts
        scoreP1 = 0;
        scoreP2 = 0;
        UpdateScoreTexts();
        resultText.text = "";
        gameEnded = false; // Reset the game ended flag
        
        ResetPlayersPosition();
        if (scoreP1 >= winningScore)
        {
            scoreP1--;
        }
        else if (scoreP2 >= winningScore)
        {
            scoreP2--;
        }
        // Reset the players' positions

    }

    // Reset the players' positions
    void ResetPlayersPosition()
    {
        Debug.Log("ResetPosition");
        // Find all player objects and reset their positions
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            player.ResetPosition();
            player.ResetBoost();
        }
    }
}
