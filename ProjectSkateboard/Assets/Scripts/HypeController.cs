using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HypeController : MonoBehaviour
{
    [SerializeField, Tooltip("The target multiplier to reach to fill the hype meter.")] private int goalMultiplier = 10;
    [SerializeField, Tooltip("The Image for the hype bar.")] private Image hypeBar;
    [SerializeField, Tooltip("The speed that the hype bar takes to fill.")] private float hypeBarAnimationSpeed = 0.3f;
    [SerializeField, Tooltip("The indicator for when the hype meter is full.")] private GameObject hypeReadyIndicator;

    public static Action HypeMeterFull;
    public static Action HypeTimeEnd;

    private float displayedMultiplier, currentMultiplier;
    private float currentAnimationTime;
    private float currentHypeBarSpeed;
    private bool hypeMeterReady, hypeTimeActive;

    private void OnEnable()
    {
        ComboManager.OnMultiplierUpdated += AddToHypeMeter;
        ComboManager.OnComboEnd += ClearHypeTime;
        PlayerController.OnHypeTimeActivated += StartHypeTime;
        ResetMeter();
    }

    private void OnDisable()
    {
        ComboManager.OnMultiplierUpdated -= AddToHypeMeter;
        ComboManager.OnComboEnd -= ClearHypeTime;
        PlayerController.OnHypeTimeActivated -= StartHypeTime;
    }

    /// <summary>
    /// Resets the meter to its default settings.
    /// </summary>
    private void ResetMeter()
    {
        hypeBar.fillAmount = 0f;
        displayedMultiplier = 0f;
        currentMultiplier = 0f;
        currentHypeBarSpeed = hypeBarAnimationSpeed;
        hypeReadyIndicator?.SetActive(false);
    }
    
    /// <summary>
    /// Adds a multiplier to the hype meter.
    /// </summary>
    /// <param name="multiplier">The amount to increment the hype meter by.</param>
    private void AddToHypeMeter(int multiplier)
    {
        if (hypeTimeActive)
            return;

        currentMultiplier += multiplier;
        currentMultiplier = Mathf.Clamp(currentMultiplier, 0, goalMultiplier);
        currentAnimationTime = 0f;

        //If the meter has reached the goal, make it ready for hype time
        if(currentMultiplier == goalMultiplier && !hypeMeterReady)
        {
            hypeMeterReady = true;
            hypeReadyIndicator?.SetActive(true);
            HypeMeterFull?.Invoke();
        }
    }

    /// <summary>
    /// Starts the hype time event.
    /// </summary>
    /// <param name="hypeTimeDuration">The duration of the hype time event.</param>
    private void StartHypeTime(float hypeTimeDuration)
    {
        currentHypeBarSpeed = hypeTimeDuration;
        currentMultiplier = 0;
        displayedMultiplier = 0f;
        currentAnimationTime = 0f;
        hypeTimeActive = true;
        hypeReadyIndicator?.SetActive(false);
    }

    /// <summary>
    /// Clears the hype meter.
    /// </summary>
    /// <param name="scoreInfo"></param>
    private void ClearHypeTime(int scoreInfo = 0)
    {
        //If hype time is active, stop hype time and deplete the bar to 0 if there's any of it left
        if (hypeTimeActive)
        {
            hypeTimeActive = false;
            hypeMeterReady = false;
            currentHypeBarSpeed = hypeBarAnimationSpeed;

            float fillAmount = Mathf.Clamp01(hypeBar.fillAmount);
            displayedMultiplier = fillAmount <= 0.8f ? fillAmount * (goalMultiplier - 1) : goalMultiplier;

            HypeTimeEnd?.Invoke();
        }
        else
            currentMultiplier = 0f;

        currentAnimationTime = 0f;
    }

    private void Update()
    {
        if(displayedMultiplier != currentMultiplier && !hypeTimeActive)
        {
            currentAnimationTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentAnimationTime / currentHypeBarSpeed);

            // Lerp between the displayed multiplier and the current multiplier
            displayedMultiplier = Mathf.Lerp(displayedMultiplier, currentMultiplier, progress);

            // Ensure the display ends up as the current multiplier
            if (progress >= 1.0f)
                displayedMultiplier = currentMultiplier;

            float fillProgress;

            //If the multiplier is less than the goal multiplier, only fill between 0 and 0.8 so that it doesn't look full before the meter is ready
            if (displayedMultiplier <= goalMultiplier - 1)
                fillProgress = Mathf.Lerp(0f, 0.8f, displayedMultiplier / (goalMultiplier - 1));
            else
            {
                float transitionProgress = (displayedMultiplier - (goalMultiplier - 1));
                fillProgress = Mathf.Lerp(0.8f, 1f, transitionProgress);
            }
            hypeBar.fillAmount = fillProgress;
        }

        //If hype time is active, constantly deplete the hype meter
        else if (hypeTimeActive)
        {
            currentAnimationTime += Time.deltaTime;
            float fillProgress = Mathf.Lerp(1.0f, 0.0f, currentAnimationTime / currentHypeBarSpeed);
            hypeBar.fillAmount = Mathf.Clamp01(fillProgress);

            if (fillProgress <= 0.0f)
                ClearHypeTime();
        }
    }

}
