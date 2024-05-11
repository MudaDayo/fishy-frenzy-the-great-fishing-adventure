using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreP1Text;
    public Text scoreP2Text;

    private int scoreP1 = 0;
    private int scoreP2 = 0;

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

    // Increment the score based on the player's tag
    public void IncrementScore(string playerTag)
    {
        if (playerTag == "Player1")
        {
            scoreP1++;
        }
        else if (playerTag == "Player2")
        {
            scoreP2++;
        }

        UpdateScoreTexts();
    }
}
