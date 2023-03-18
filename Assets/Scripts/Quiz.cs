using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private QuestionSO currentQuestion;
    [SerializeField] private List<QuestionSO> questions = new List<QuestionSO>();

    [Header("Answers")]
    [SerializeField] private GameObject[] answerButtons;
    private int _correctAnswerIndex;
    private bool _hasAnsweredEarly = true;
    
    [Header("Button Colors")]
    [SerializeField] private Sprite defaultAnswerSprite;
    [SerializeField] private Sprite correctAnswerSprite;
    
    [Header("Timer")]
    [SerializeField] private Image timerImage;
    private Timer _timer;
        
    [Header("Scoring")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private ScoreKeeper _scoreKeeper;
    
    [Header("ProgressBar")]
    [SerializeField] private Slider progressBar;

    public bool isComplete;


    private void Start()
    {
        _timer = FindObjectOfType<Timer>();
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    private void Update()
    {
        timerImage.fillAmount = _timer.fillFraction;
        if (_timer.loadNextQuestion)
        {
            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;
            }
            _hasAnsweredEarly = false ;
            GetNextQuestion();
            _timer.loadNextQuestion = false;
        }
        else if(!_hasAnsweredEarly && !_timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

    public void OnAnswerSelected(int index)
    {
        _hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        _timer.CancelTimer();
        scoreText.text = "Score: " + _scoreKeeper.CalculateScore() + "%";
    }

    private void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];
        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
    }

    public void DisplayAnswer(int index)
    {
        _correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
        if (index == _correctAnswerIndex)
        {
            questionText.text = "Correct";
            Image buttonImage = answerButtons[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
            _scoreKeeper.IncrementCorrectAnswer();
        }
        else
        {
            questionText.text = "Sorry, te correct answer was: " + currentQuestion.GetAnswer(_correctAnswerIndex);
            Image buttonImage = answerButtons[_correctAnswerIndex].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
        }
    }

    void GetNextQuestion()
    {
        if (questions.Count <= 0) return;
        SetButtonState(true);
        SetDefaultButtonSprite();
        GetRandomQuestion();
        DisplayQuestion();
        progressBar.value++;
        _scoreKeeper.IncrementQuestionsSeen();
    }

    private void SetDefaultButtonSprite()
    {
        foreach (var button in answerButtons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }

    private void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            var buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.GetAnswer(i);
        }
    }

    private void SetButtonState(bool state)
    {
        foreach (var answerButton in answerButtons)
        {
            Button button = answerButton.GetComponent<Button>();
            button.interactable = state;
        }
    }
}
