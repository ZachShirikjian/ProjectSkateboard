using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalType
{
    Score,
    Escape
}

[CreateAssetMenu(menuName = "Data / Objective Type")]
public class ObjectiveType : ScriptableObject
{
    public TimeOfDay TimeOfDay;
    public GoalType objectiveGoalType;
    [TextArea] public string description;
}
