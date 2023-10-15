using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField, Tooltip("The amount of time allotted for the level (in seconds).")] private float levelTime = 60f;

    [SerializeField, Tooltip("The game score.")] private ScoreManager gameScore;
    [SerializeField, Tooltip("The game timer.")] private GameTimer gameTimer;

    [Header("Debug Settings")]
    public bool debugAddScore = false;
    public float debugScoreValue = 1000;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameTimer.InitializeTimer(levelTime);
    }

    private void OnEnable()
    {
        GameTimer.OnTimerEnded += EndLevel;
        ComboManager.OnComboEnd += AddComboToScore;
    }

    private void OnDisable()
    {
        GameTimer.OnTimerEnded -= EndLevel;
        ComboManager.OnComboEnd -= AddComboToScore;
    }

    private void Update()
    {
        if (debugAddScore)
        {
            AddComboToScore((int)debugScoreValue);
            debugAddScore = false;
        }
    }

    /// <summary>
    /// Adds the current combo score to the total score.
    /// </summary>
    /// <param name="comboScore">The current combo score.</param>
    private void AddComboToScore(int comboScore)
    {
        gameScore.AddToScore(comboScore);
    }

    private void EndLevel()
    {
        Debug.Log("Level Ended!");
    }
}
