using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboName;
    [SerializeField] private TextMeshProUGUI comboScore;
    [SerializeField] private TextMeshProUGUI totalComboScore;
    [SerializeField] private TextMeshProUGUI comboMultiplier;
    [SerializeField] private TextMeshProUGUI hypeTimeMultiplierText;
    [SerializeField] private float maxMultiplierScale;
    [SerializeField] private float timeUntilComboBreak = 5f;
    [SerializeField] private float hypeTimeMultiplier = 1.5f;

    [SerializeField] private GameObject cooldownBarParent;
    [SerializeField] private Image cooldownBar;

    [Header("Animation Settings")]
    [SerializeField] private float newComboAniDuration;
    [SerializeField] private LeanTweenType newComboEaseType;
    [Space(10)]
    [SerializeField] private float endComboScoreYMovement;
    [SerializeField] private float endComboAniDuration;
    [SerializeField] private float endComboAniStrength;
    [SerializeField] private float endComboDelay;
    [SerializeField] private LeanTweenType endComboScoreEaseType;
    [SerializeField] private AnimationCurve endComboEaseType;

    public static Action<int> OnMultiplierUpdated;
    public static Action<int> OnComboEnd;

    private int currentComboScore;
    private int currentComboMultiplier;
    private float currentComboCooldown;
    private CanvasGroup comboCanvasGroup;
    private RectTransform comboMultiplierTransform;
    private RectTransform totalComboScoreTransform;
    private Vector2 totalComboScoreInitialPosition;

    private bool comboActive, comboCooldownActive, addHypeMultiplier;

    private void Start()
    {
        comboCanvasGroup = GetComponentInChildren<CanvasGroup>();
        comboMultiplierTransform = GetComponent<RectTransform>();
        totalComboScoreTransform = totalComboScore.GetComponent<RectTransform>();
        totalComboScoreInitialPosition = totalComboScoreTransform.anchoredPosition;
        ResetComboUI();
    }

    private void OnEnable()
    {
        PlayerController.OnComboSuccess += AddCombo;
        PlayerController.OnHypeTimeActivated += AddHypeMultiplier;
        PlayerController.OnPlayerGrounded += StartComboCooldown;
        HypeController.HypeTimeEnd += RemoveHypeMultiplier;
    }

    private void OnDisable()
    {
        PlayerController.OnComboSuccess -= AddCombo;
        PlayerController.OnHypeTimeActivated -= AddHypeMultiplier;
        PlayerController.OnPlayerGrounded -= StartComboCooldown;
        HypeController.HypeTimeEnd -= RemoveHypeMultiplier;
    }

    private void ResetComboUI()
    {
        comboCanvasGroup.alpha = 0f;
        cooldownBarParent.SetActive(false);
        cooldownBar.fillAmount = 0f;
        currentComboMultiplier = 0;
        gameObject.transform.localScale = Vector3.zero;
        comboMultiplierTransform.localScale = Vector3.one;
        totalComboScoreTransform.localScale = Vector3.one;
        totalComboScoreTransform.anchoredPosition = totalComboScoreInitialPosition;
        UpdateMultiplier();
    }

    public void AddCombo(Combo comboInfo)
    {
        if (comboCanvasGroup.alpha == 0f)
        {
            comboActive = true;
            StartComboAnimation();
        }

        currentComboScore += Mathf.CeilToInt(comboInfo.score * (addHypeMultiplier ? hypeTimeMultiplier : 1f));
        currentComboMultiplier++;

        comboName.text = comboInfo.name;
        comboScore.text = comboInfo.score.ToString();
        hypeTimeMultiplierText.text = addHypeMultiplier ? "x" + hypeTimeMultiplier.ToString("F1") : "";
        totalComboScore.text = (currentComboScore * currentComboMultiplier).ToString();

        GameManager.Instance.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.TrickSuccess);

        OnMultiplierUpdated?.Invoke(1);

        cooldownBarParent.SetActive(false);
        currentComboCooldown = 0f;
        comboCooldownActive = false;
        UpdateMultiplier();
    }

    public void EndCombo()
    {
        currentComboScore *= currentComboMultiplier;
        OnComboEnd?.Invoke(currentComboScore);
        RemoveHypeMultiplier();
        
        comboName.text = "";
        comboMultiplier.text = "";
        hypeTimeMultiplierText.text = "";

        comboScore.text = currentComboScore.ToString();

        currentComboScore = 0;
        currentComboMultiplier = 0;

        comboCooldownActive = false;
        cooldownBarParent.SetActive(false);
        comboActive = false;
        EndComboAnimation();
        GameManager.Instance.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.ComboEnd);
    }

    private void StartComboAnimation()
    {
        LTDescr startAniCanvas = LeanTween.alphaCanvas(comboCanvasGroup, 1f, newComboAniDuration).setEase(newComboEaseType);
        LTDescr startAniScale = LeanTween.scale(gameObject, Vector3.one, newComboAniDuration).setEase(newComboEaseType);
    }

    private void EndComboAnimation()
    {
        LTDescr endAniScale = LeanTween.scale(comboScore.gameObject, Vector3.one * endComboAniStrength, endComboAniDuration).setEase(endComboEaseType);
        LTDescr endAniScoreMove = LeanTween.moveY(totalComboScoreTransform, totalComboScoreInitialPosition.y + endComboScoreYMovement, endComboAniDuration).setEase(endComboScoreEaseType);
        LTDescr endAniScoreScale = LeanTween.scale(totalComboScoreTransform, Vector3.zero, endComboAniDuration).setEase(endComboScoreEaseType);
        LTDescr endAniDelay = LeanTween.delayedCall(endComboDelay, () => ResetComboUI());
    }

    private void UpdateMultiplier()
    {
        comboMultiplier.text = "x" + currentComboMultiplier.ToString();
        float value = Mathf.Clamp(currentComboMultiplier, 1f, 10f);
        float t = (value - 1f) / (10f - 1f);
        Vector3 newScale = Vector3.Lerp(Vector3.one, Vector3.one * maxMultiplierScale, t);
        comboMultiplier.transform.localScale = newScale;
    }

    private void AddHypeMultiplier(float hypeTime = 0f) => addHypeMultiplier = true;
    private void RemoveHypeMultiplier() => addHypeMultiplier = false;

    private void StartComboCooldown()
    {
        if (!comboActive)
            return;

        comboCooldownActive = true;
        cooldownBarParent.SetActive(true);
    }

    private void Update()
    {
        if (comboCooldownActive)
        {
            if (currentComboCooldown < timeUntilComboBreak)
            {
                currentComboCooldown += Time.deltaTime;
                cooldownBar.fillAmount = Mathf.Clamp01(currentComboCooldown / timeUntilComboBreak);
            }
            else
                EndCombo();
        }
    }
}
