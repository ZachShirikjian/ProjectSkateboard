using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Objective))]
public class ObjectiveDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Get property values for the ScriptableObject
        SerializedProperty objectiveTypeProperty = serializedObject.FindProperty("objectiveType");
        SerializedProperty timeLimitProperty = serializedObject.FindProperty("timeLimit");
        SerializedProperty scoreValueProperty = serializedObject.FindProperty("scoreValue");

        //Label
        EditorGUILayout.LabelField("Objective Information", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(objectiveTypeProperty, new GUIContent("Objective Type"));
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        ObjectiveType selectedObjectiveType = null;

        if (objectiveTypeProperty.objectReferenceValue != null)
        {
            selectedObjectiveType = objectiveTypeProperty.objectReferenceValue as ObjectiveType;

            if (selectedObjectiveType != null)
            {
                EditorGUILayout.LabelField("Time of Day", selectedObjectiveType.TimeOfDay.ToString());
                EditorGUILayout.LabelField("Goal Type", selectedObjectiveType.objectiveGoalType.ToString());
                EditorGUILayout.LabelField("Description", selectedObjectiveType.description);
                EditorGUILayout.Space(20);
            }
        }

        timeLimitProperty.floatValue = EditorGUILayout.FloatField(new GUIContent("Time Limit"), timeLimitProperty.floatValue);

        if (selectedObjectiveType != null && selectedObjectiveType.objectiveGoalType == GoalType.Score)
            scoreValueProperty.intValue = EditorGUILayout.IntField(new GUIContent("Goal Score"), scoreValueProperty.intValue);

        serializedObject.ApplyModifiedProperties();
    }
}
