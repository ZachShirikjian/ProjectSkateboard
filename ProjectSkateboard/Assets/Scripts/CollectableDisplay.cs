using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableDisplay : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }

    [SerializeField, Tooltip("The UI collectable image prefab.")] private Image collectablePrefab;
    [SerializeField, Tooltip("The color of the image when inactive.")] private Color inactiveColor;

    [SerializeField, Tooltip("Strength of the pulse for the collectable animation.")] private float animationStrength = 1.2f;
    [SerializeField, Tooltip("Speed of the pulse for the collectable animation.")] private float animationSpeed = 0.2f;
    [SerializeField, Tooltip("The collectable animation curve for when a collectable is collected.")] private AnimationCurve collectableAnimationCurve;

    private Color activeColor = Color.white;
    private int collectableCount;
    private int currentCount;

    private List<Image> collectableImages = new List<Image>();

    private void Start()
    {
        InitializeDisplay();
    }

    private void OnEnable()
    {
        Collectible.OnCollectable += UpdateCollectableDisplay;
    }

    private void OnDisable()
    {
        Collectible.OnCollectable -= UpdateCollectableDisplay;
    }

    /// <summary>
    /// Creates a display of collectables based on the number of collectables in the scene.
    /// </summary>
    private void InitializeDisplay()
    {
        collectableCount = FindObjectsOfType<Collectible>().Length;

        //Clear display
        foreach (Transform trans in transform)
            Destroy(trans.gameObject);

        //Create a display accurate to the number of collectables in the level
        for(int i = 0; i < collectableCount; i++)
        {
            Image newCollectable = Instantiate(collectablePrefab, transform);
            newCollectable.color = inactiveColor;
            collectableImages.Add(newCollectable);
        }
    }

    /// <summary>
    /// Updates the collectable display when a new collectable is retrieved.
    /// </summary>
    /// <param name="score"></param>
    private void UpdateCollectableDisplay(int score)
    {
        collectableImages[currentCount].color = activeColor;
        AnimateCollectableImage(collectableImages[currentCount]);
        currentCount++;
        GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.CollectCD);
        if(currentCount >= 3)
        {
                    GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.CollectCD);
        }

    }

    /// <summary>
    /// Animates the collectable image being used to show that a collectable is retrieved.
    /// </summary>
    /// <param name="currentCollectable"></param>
    private void AnimateCollectableImage(Image currentCollectable)
    {
        LeanTween.scale(currentCollectable.gameObject, Vector3.one * animationStrength, animationSpeed).setEase(collectableAnimationCurve);
    }
}
