using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data / Objective")]
public class Objective : ScriptableObject
{
    public ObjectiveType objectiveType;
    public float timeLimit = 60f;

    public int scoreValue;

    public override string ToString()
    {
        string objectiveGoalText = "Goal: ";

        switch (objectiveType.objectiveGoalType)
        {
            case GoalType.Score:
                objectiveGoalText += scoreValue.ToString("n0") + " Points";
                break;
            case GoalType.Escape:
                // objectiveGoalText += "Survive For " + timeLimit.ToString() + " Seconds";
                objectiveGoalText += "Find The Exit in 90 Seconds!";
                break;
        }

        return objectiveGoalText;
    }
}