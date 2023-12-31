using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public enum TimeOfDay { DAY, NIGHT }

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [field:SerializeField, Tooltip("The objective for the level.")] public Objective levelObjective { get; private set; }

    [SerializeField, Tooltip("The game score.")] private ScoreManager gameScore;
    [SerializeField, Tooltip("The game timer.")] private GameTimer gameTimer;
    [SerializeField, Tooltip("The starting objective object.")] private StartingObjectiveManager startingObjectiveManager;
    [field: SerializeField, Tooltip("The goal progress bar.")] public GoalLevelProgressController progressBar { get; private set; }
    [SerializeField, Tooltip("The level results screen.")] private LevelResultsScreenController levelResultsScreenController;

    public static Action OnLevelWin;
    public static Action OnLevelFailed;

    [Header("Debug Settings")]
    public bool debugAddScore = false;
    public float debugScoreValue = 1000;

    internal bool isGamePaused = false;
    private bool isGameActive = false;
    private bool levelEnded = false;
    private bool levelCleared = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameTimer?.InitializeTimer(levelObjective.timeLimit);
        levelResultsScreenController.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        TimeTransitionManager.OnTransitionEnded += StartObjectiveCountdown;
        StartingObjectiveManager.OnCountdownEnded += StartLevel;
        GameTimer.OnTimerEnded += EndLevel;
        ComboManager.OnComboEnd += AddToTotalScore;
        Collectible.OnCollectable += AddToTotalScore;
        GoalLevelProgressController.OnProgressComplete += ClearLevel;
    }

    private void OnDisable()
    {
        TimeTransitionManager.OnTransitionEnded -= StartObjectiveCountdown;
        StartingObjectiveManager.OnCountdownEnded -= StartLevel;
        GameTimer.OnTimerEnded -= EndLevel;
        ComboManager.OnComboEnd -= AddToTotalScore;
        Collectible.OnCollectable -= AddToTotalScore;
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
            AddToTotalScore((int)debugScoreValue);
            debugAddScore = false;
        }
    }

    /// <summary>
    /// Adds the current score to the total score.
    /// </summary>
    /// <param name="addedScore">The current score to add.</param>
    private void AddToTotalScore(int addedScore)
    {
        gameScore?.AddToScore(addedScore);
    }

    private void ClearLevel() => levelCleared = true;

    private async void EndLevel()
    {
        isGameActive = false;
        levelEnded = true;

        await Task.Delay(100);

        levelResultsScreenController.gameObject.SetActive(true);

        if (levelCleared)
        {
            if (PlayerPrefs.GetInt("LocalHighScore") < gameScore.GetScore())
                PlayerPrefs.SetInt("LocalHighScore", gameScore.GetScore());
            OnLevelWin?.Invoke();
        }
        else
            OnLevelFailed?.Invoke();
    }

    public TimeOfDay GetTimeOfDay() => levelObjective.objectiveType.TimeOfDay;
    public bool IsGameActive() => isGameActive;
    public bool IsLevelCleared() => levelCleared;
    public bool HasLevelEnded() => levelEnded;
}
