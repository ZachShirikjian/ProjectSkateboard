using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TimeOfDay { DAY, NIGHT }

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [field:SerializeField, Tooltip("The objective for the level.")] public Objective levelObjective { get; private set; }

    [SerializeField, Tooltip("The game score.")] private ScoreManager gameScore;
    [SerializeField, Tooltip("The game timer.")] private GameTimer gameTimer;
    [SerializeField, Tooltip("The starting objective object.")] private StartingObjectiveManager startingObjectiveManager;
    [field: SerializeField, Tooltip("The goal progress bar.")] public GoalLevelProgressController progressBar { get; private set; }

    [Header("Debug Settings")]
    public bool debugAddScore = false;
    public float debugScoreValue = 1000;

    private bool isGameActive = false;
    private bool levelCleared = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameTimer?.InitializeTimer(levelObjective.timeLimit);
    }

    private void OnEnable()
    {
        TimeTransitionManager.OnTransitionEnded += StartObjectiveCountdown;
        StartingObjectiveManager.OnCountdownEnded += StartLevel;
        GameTimer.OnTimerEnded += EndLevel;
        ComboManager.OnComboEnd += AddComboToScore;
        GoalLevelProgressController.OnProgressComplete += ClearLevel;
    }

    private void OnDisable()
    {
        TimeTransitionManager.OnTransitionEnded -= StartObjectiveCountdown;
        StartingObjectiveManager.OnCountdownEnded -= StartLevel;
        GameTimer.OnTimerEnded -= EndLevel;
        ComboManager.OnComboEnd -= AddComboToScore;
        GoalLevelProgressController.OnProgressComplete -= ClearLevel;
    }

    public void StartObjectiveCountdown() => startingObjectiveManager?.InitializeObjective(levelObjective);

    public void StartLevel()
    {
        isGameActive = true;
        levelCleared = false;
        gameTimer?.StartTimer();
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
        gameScore?.AddToScore(comboScore);
    }

    private void ClearLevel() => levelCleared = true;

    private void EndLevel()
    {
        isGameActive = false;
        if (levelCleared)
        {
            Debug.Log("You Win!");
        }

        else
        {
            Debug.Log("You Fail!");
        }
    }

    public TimeOfDay GetTimeOfDay() => levelObjective.objectiveType.TimeOfDay;
    public bool IsGameActive() => isGameActive;
    public bool IsLevelCleared() => levelCleared;
}
