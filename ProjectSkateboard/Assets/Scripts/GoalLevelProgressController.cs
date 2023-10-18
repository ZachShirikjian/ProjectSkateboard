using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalLevelProgressController : MonoBehaviour
{
    [SerializeField, Tooltip("The text for the objective description.")] private TextMeshProUGUI objectiveText;
    [SerializeField, Tooltip("The text for the goal description.")] private TextMeshProUGUI goalText;
    [SerializeField, Tooltip("The progress bar fill image.")] private Image progressBarImage;
    public static Action OnProgressComplete;

    private Objective currentObjective;

    private void Start()
    {
        progressBarImage.fillAmount = 0f;
        currentObjective = LevelManager.Instance?.levelObjective;

        objectiveText.text = currentObjective.objectiveType.description;
        goalText.text = currentObjective.ToString();
    }

    /// <summary>
    /// Updates the progress bar to show how complete the current objective is.
    /// </summary>
    /// <param name="progress">The progress of the objective (from 0 to 1).</param>
    public void UpdateProgressBar(float progress)
    {
        progressBarImage.fillAmount = Mathf.Clamp01(progress);

        //Tell the LevelManager that the goal has been achieved
        if (progressBarImage.fillAmount == 1)
            OnProgressComplete?.Invoke();
    }
}
