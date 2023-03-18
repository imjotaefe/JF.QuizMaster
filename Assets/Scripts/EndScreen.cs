using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private ScoreKeeper scoreKeeper;

    public void ShowFinalScore()
    {
        finalScoreText.text = "You score " + scoreKeeper.CalculateScore() + "%";
    }
}
