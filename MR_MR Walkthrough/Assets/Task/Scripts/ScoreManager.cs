using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public int CurrentScore = 0;
    public int FixedScore = 100;

    public TMP_Text scoreText;  

    public void AddScore(int score)
    {
        if (score < FixedScore)
        {
            CurrentScore += score;
        }
    }

    public void ShowScore()
    {
        scoreText.text = "Your Score Is : "+ CurrentScore.ToString();
    }
}
