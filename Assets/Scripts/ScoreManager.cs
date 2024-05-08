using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public Text scoreP1Text;
    public Text scoreP2Text;

    int scoreP1 = 0;
    int scoreP2 = 0;
    

    void Start()
    {
        scoreP1Text.text = "Player1 Score: " + scoreP1.ToString();
        scoreP2Text.text = "Player2 Score: " + scoreP2.ToString();

    }

    
    void Update()
    {
        
    }
}
