using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreP1Text;
    public Text scoreP2Text;
    public Text resultText;

    private int scoreP1 = 0;
    private int scoreP2 = 0;
    public int winningScore = 5; // Set the winning score

    private bool gameEnded = false; // Flag to track if the game has ended

    void Start()
    {
        UpdateScoreTexts();
    }

    // Update the score texts to reflect the current scores
    void UpdateScoreTexts()
    {
        scoreP1Text.text = "Player1 Score: " + scoreP1.ToString();
        scoreP2Text.text = "Player2 Score: " + scoreP2.ToString();
    }

    // Increment the score for the given player
    public void IncrementScore(string playerTag)
    {
        if (gameEnded) return; // If the game has ended, do not update the score

        if (playerTag == "Player1")
        {
            scoreP1++;
        }
        else if (playerTag == "Player2")
        {
            scoreP2++;
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
            gameEnded = true; // Set the flag to indicate that the game has ended
        }
        else if (scoreP2 >= winningScore)
        {
            // Player 2 wins
            ShowResult("Player 2 Wins!", "Player 1 Loses!");
            gameEnded = true; // Set the flag to indicate that the game has ended
        }
    }

    // Show the result on the screen
    void ShowResult(string winText, string loseText)
    {
        resultText.text = winText;
        // Optionally, you can add some delay or animation before resetting the game
        // For example: StartCoroutine(ResetGame());
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
    }
}
