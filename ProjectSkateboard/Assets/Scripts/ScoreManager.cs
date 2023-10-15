using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField, Tooltip("The minimum duration for the score change.")] private float minScoreAnimationDuration = 0.5f;
    [SerializeField, Tooltip("The maximum duration for the score change.")] private float maxScoreAnimationDuration = 2f;
    [SerializeField, Tooltip("The score animation range (the larger the number, the bigger the amount has to be in order to reach the max score duration).")] private float scoreAnimationDurationRange = 100f;

    private float displayScore, currentScore;
    private float transitionStartTime;

    private TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(displayScore != currentScore)
        {
            //Calculate the progress based on the time elapsed and the time the transition started
            float transitionDuration = CalculateTransitionDuration();
            float progress = Mathf.Clamp01((Time.time - transitionStartTime) / transitionDuration);

            //Lerp between the displayed score and the current score
            displayScore = Mathf.Round(Mathf.Lerp(displayScore, currentScore, progress));

            //Make sure the display score ends up as the current score
            if (progress >= 1.0f)
                displayScore = currentScore;
        }

        UpdateScoreDisplay();
    }

    /// <summary>
    /// Adds to the total score.
    /// </summary>
    /// <param name="amount">The amount to change the score by.</param>
    public void AddToScore(float amount)
    {
        currentScore += amount;
        transitionStartTime = Time.time;
    }

    private float CalculateTransitionDuration() => Mathf.Lerp(minScoreAnimationDuration, maxScoreAnimationDuration, Mathf.Abs(currentScore - displayScore) / scoreAnimationDurationRange);
    private void UpdateScoreDisplay() => scoreText.text = displayScore.ToString();
}
