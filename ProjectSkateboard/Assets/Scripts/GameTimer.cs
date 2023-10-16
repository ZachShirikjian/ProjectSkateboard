using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static Action OnTimerEnded;

    private float currentTimeLeft;
    private TextMeshProUGUI timerText;
    private bool timerActive = false;

    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Initializes a starting time.
    /// </summary>
    /// <param name="timerSeconds">The time to start the countdown at (in seconds).</param>
    public void InitializeTimer(float timerSeconds)
    {
        currentTimeLeft = timerSeconds;
        timerText.text = TimeToString(currentTimeLeft - 1);
    }

    public void StartTimer() => timerActive = true;

    public void EndTimer()
    {
        OnTimerEnded?.Invoke();
    }

    private void Update()
    {
        if (timerActive)
        {
            if(currentTimeLeft > 0)
            {
                currentTimeLeft -= Time.deltaTime;
                timerText.text = TimeToString(currentTimeLeft);
            }
            else
            {
                currentTimeLeft = 0;
                EndTimer();
            }
        }
    }

    /// <summary>
    /// Converts the time in seconds to displayable text.
    /// </summary>
    /// <param name="currentTime">The time to display.</param>
    /// <returns>The time as a displayable formatted string.</returns>
    private string TimeToString(float currentTime)
    {
        //Adds one to the time so that the timer doesn't initially skip the first value
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        //Formats the minutes and seconds so that the minutes have at least one digit and the seconds have at least two digits
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
