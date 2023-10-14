using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data / Player Settings")]
public class PlayerSettings : ScriptableObject
{
    public float moveSpeed = 5f;
    public float airSpeed = 3f;
    public float railSpeed = 5f;
    public float rotationSpeed = 0.2f;
    public float deceleration = 5f;
    public float maxSpeed = 10f;
    public float jumpForce = 10f;
}
