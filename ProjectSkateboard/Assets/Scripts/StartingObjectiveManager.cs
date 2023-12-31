using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartingObjectiveManager : MonoBehaviour
{
    public static GameManager ManagerInstance { get; private set; }
    public AudioManager AudioManager { get; private set; }

    [SerializeField, Tooltip("The objective description text.")] private TextMeshProUGUI objectiveText;
    [SerializeField, Tooltip("The objective goal text.")] private TextMeshProUGUI goalText;
    [SerializeField, Tooltip("The objective countdown text.")] private TextMeshProUGUI countdownText;

    [SerializeField, Tooltip("The delay before the level countdown.")] private float countdownDelay = 3f;
    [SerializeField, Tooltip("The number of seconds to start counting down from.")] private int countdownTime = 3;
    [SerializeField, Tooltip("The speed of which to countdown from.")] private float countdownRate = 1f;

    private bool countdownActive;
    private float currentCountdownTime;
    private int totalCountdownTime;
    private CanvasGroup objectiveScreenCanvasGroup;

    public static Action OnCountdownEnded;

    private void Awake()
    {
        objectiveScreenCanvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Initialize the objective and goal text.
    /// </summary>
    /// <param name="currentObjective">The current objective data.</param>
    public void InitializeObjective(Objective currentObjective)
    {
        totalCountdownTime = countdownTime;

        objectiveText.text = currentObjective.objectiveType.description;
        goalText.text = currentObjective.ToString();
        countdownText.text = "";
        Invoke("StartCountdown", countdownDelay);
        objectiveScreenCanvasGroup.alpha = 1f;
    }

    private void StartCountdown()
    {
        countdownActive = true;
        countdownText.text = totalCountdownTime.ToString();
    }

    private void EndCountdownAnimation()
    {
        gameObject.SetActive(false);
        GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.DayMusic);
    }

    private void Update()
    {
        if (countdownActive)
        {
            if (currentCountdownTime < countdownRate)
                currentCountdownTime += Time.deltaTime;
            else
            {
                totalCountdownTime--;
                currentCountdownTime = 0f;
                if (totalCountdownTime == 0)
                    countdownText.text = "Go!";

                else if(totalCountdownTime == -1)
                {
                    countdownActive = false;
                    OnCountdownEnded?.Invoke();
                    EndCountdownAnimation();
                }

                else
                    countdownText.text = totalCountdownTime.ToString();
            }
        }
    }
}
