using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTransitionManager : MonoBehaviour
{
    [SerializeField, Tooltip("The day active transform.")] private RectTransform dayActiveTransform;
    [SerializeField, Tooltip("The day inactive transform.")] private RectTransform nightActiveTransform;
    [SerializeField, Tooltip("The night active transform.")] private RectTransform dayInactiveTransform;
    [SerializeField, Tooltip("The night inactive transform.")] private RectTransform nightInactiveTransform;
    [SerializeField, Tooltip("The amount of movement for the text to move in the Y position during the animation.")] private float yMovement = 325f;

    [SerializeField, Tooltip("The color for the text that is inactive.")] private Color inactiveTextColor;

    [SerializeField, Tooltip("The delay before the transition begins.")] private float transitionBeginDelay = 2f;
    [SerializeField, Tooltip("The time it takes for the transition time to be active.")] private float transitionDuration = 3f;
    [SerializeField, Tooltip("The ease type for the time transition.")] private LeanTweenType transitionEaseType;
    [SerializeField, Tooltip("The delay after the transition ends.")] private float transitionEndDelay = 2f;

    [SerializeField, Tooltip("The time it takes for the transition time to fade out.")] private float fadeOutDuration = 3f;
    [SerializeField, Tooltip("The ease type for the fade out transition.")] private LeanTweenType fadeOutEaseType;

    public static Action OnTransitionEnded;

    private Image dayActiveImage;
    private Image dayInactiveImage;
    private Image nightActiveImage;
    private Image nightInactiveImage;
    private CanvasGroup transitionCanvasGroup;
    private TimeOfDay currentTime;

        public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }

    private void Start()
    {
        TransitionDay();
    }

    /// <summary>
    /// Begins the transition after initializing the screen and completing the delay.
    /// </summary>
    private void TransitionDay()
    {
        InitializeTime();
        Invoke("AnimateTransition", transitionBeginDelay);
    }

    /// <summary>
    /// Initializes the transition time to begin at the opposite time of day, so that the animation can transition to the correct time of day.
    /// </summary>
    private void InitializeTime()
    {
        transitionCanvasGroup = GetComponent<CanvasGroup>();
        transitionCanvasGroup.alpha = 1f;
        dayActiveImage = dayActiveTransform.GetComponent<Image>();
        nightActiveImage = nightActiveTransform.GetComponent<Image>();
        dayInactiveImage = dayInactiveTransform.GetComponent<Image>();
        nightInactiveImage = nightInactiveTransform.GetComponent<Image>();
        currentTime = LevelManager.Instance.GetTimeOfDay();

        //If the time is day, show the night time screen
        if (currentTime == TimeOfDay.DAY)
        {
            dayActiveImage.color = inactiveTextColor;
            dayInactiveImage.color = inactiveTextColor;
            nightActiveImage.color = Color.white;
            nightInactiveImage.color = Color.white;

            dayActiveTransform.anchoredPosition = new Vector2(dayActiveTransform.anchoredPosition.x, -yMovement);
            dayInactiveTransform.anchoredPosition = new Vector2(dayInactiveTransform.anchoredPosition.x, 0);

            nightActiveTransform.anchoredPosition = new Vector2(nightActiveTransform.anchoredPosition.x, 0f);
            nightInactiveTransform.anchoredPosition = new Vector2(nightInactiveTransform.anchoredPosition.x, yMovement);
        }

        //If the time is night, show the day time screen
        else
        {
            dayActiveImage.color = Color.white;
            dayInactiveImage.color = Color.white;
            nightActiveImage.color = inactiveTextColor;
            nightInactiveImage.color = inactiveTextColor;

            dayActiveTransform.anchoredPosition = new Vector2(dayActiveTransform.anchoredPosition.x, 0f);
            dayInactiveTransform.anchoredPosition = new Vector2(dayInactiveTransform.anchoredPosition.x, yMovement);

            nightActiveTransform.anchoredPosition = new Vector2(nightActiveTransform.anchoredPosition.x, -yMovement);
            nightInactiveTransform.anchoredPosition = new Vector2(nightInactiveTransform.anchoredPosition.x, 0f);
        }
    }

    /// <summary>
    /// Calls the time transition animation and acts accordingly depending on the time of day.
    /// </summary>
    private void AnimateTransition()
    {
        if (currentTime == TimeOfDay.DAY)
        {
            GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.NightToDayTransition);
            MoveToActivePosition(dayActiveTransform, dayInactiveTransform);
            MoveToInactivePosition(nightActiveTransform, nightInactiveTransform);
        }

        else if(currentTime == TimeOfDay.NIGHT)
        {
            GameManager.Instance?.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.DayToNightTransition);
            MoveToActivePosition(nightActiveTransform, nightInactiveTransform);
            MoveToInactivePosition(dayActiveTransform, dayInactiveTransform);
        }
        FadeOutTimeTransition();
    }

    /// <summary>
    /// Fades out the transition screen after the animation is complete.
    /// </summary>
    private void FadeOutTimeTransition()
    {
        LeanTween.delayedCall(transitionDuration + transitionEndDelay, () =>
            LeanTween.alphaCanvas(transitionCanvasGroup, 0f, fadeOutDuration).setEase(fadeOutEaseType).setOnStart(() => OnTransitionEnded?.Invoke()));
    }

    /// <summary>
    /// Moves the active text into view and the inactive text out of view.
    /// </summary>
    /// <param name="timeActiveTransform">The active text transform.</param>
    /// <param name="timeInactiveTransform">The inactive text transform.</param>
    private void MoveToActivePosition(RectTransform timeActiveTransform, RectTransform timeInactiveTransform)
    {
        LeanTween.moveY(timeActiveTransform, 0f, transitionDuration).setEase(transitionEaseType);
        LeanTween.moveY(timeInactiveTransform, yMovement, transitionDuration).setEase(transitionEaseType);
        LeanTween.color(timeActiveTransform, Color.white, transitionDuration).setEase(transitionEaseType);
        LeanTween.color(timeInactiveTransform, Color.white, transitionDuration).setEase(transitionEaseType);
    }

    /// <summary>
    /// Moves the inactive text into view and the active text into view.
    /// </summary>
    /// <param name="timeActiveTransform">The active text transform.</param>
    /// <param name="timeInactiveTransform">The inactive text transform.</param>
    private void MoveToInactivePosition(RectTransform timeActiveTransform, RectTransform timeInactiveTransform)
    {
        LeanTween.moveY(timeActiveTransform, -yMovement, transitionDuration).setEase(transitionEaseType);
        LeanTween.moveY(timeInactiveTransform, 0f, transitionDuration).setEase(transitionEaseType);
        LeanTween.color(timeActiveTransform, inactiveTextColor, transitionDuration).setEase(transitionEaseType);
        LeanTween.color(timeInactiveTransform, inactiveTextColor, transitionDuration).setEase(transitionEaseType);
    }

}
