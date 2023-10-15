using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextShaker : MonoBehaviour
{
    [SerializeField, Tooltip("The speed of the text shake (how often it shakes).")] private float shakeSpeed = 5.0f;
    [SerializeField, Tooltip("The magnitude of the shake (how strong the shake is).")] float shakeMagnitude = 0.1f;
    [SerializeField, Tooltip("The seed for the perlin noise.")] private Vector2 perlinNoiseSeed = new Vector2(0, 1);

    private Vector3 initialPosition;
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        initialPosition = textMesh.rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        textMesh.rectTransform.anchoredPosition = initialPosition;
    }

    void Update()
    {
        //Calculate the offsets using Perlin noise
        float xNoise = Mathf.PerlinNoise(perlinNoiseSeed.x, Time.time * shakeSpeed) * 2.0f - 1.0f;
        float yNoise = Mathf.PerlinNoise(perlinNoiseSeed.y, Time.time * shakeSpeed) * 2.0f - 1.0f;

        //Apply the perlin noise to the text's anchored position
        Vector2 newPosition = initialPosition + new Vector3(xNoise, yNoise) * shakeMagnitude;
        textMesh.rectTransform.anchoredPosition = newPosition;
    }
}
