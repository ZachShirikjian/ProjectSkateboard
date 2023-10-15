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
    [SerializeField] private float timeUntilComboBreak = 5f;

    [SerializeField] private GameObject cooldownBarParent;
    [SerializeField] private Image cooldownBar;

    public static Action<int> OnComboEnd;

    private int currentComboScore;
    private float currentComboCooldown;
    private CanvasGroup comboCanvasGroup;

    private bool comboActive, comboCooldownActive;

    private void Start()
    {
        comboCanvasGroup = GetComponentInChildren<CanvasGroup>();
        comboCanvasGroup.alpha = 0f;
        cooldownBarParent.SetActive(false);
        cooldownBar.fillAmount = 0f;
    }

    private void OnEnable()
    {
        PlayerController.OnComboSuccess += AddCombo;
        PlayerController.OnPlayerGrounded += StartComboCooldown;
    }

    private void OnDisable()
    {
        PlayerController.OnComboSuccess -= AddCombo;
        PlayerController.OnPlayerGrounded -= StartComboCooldown;
    }

    public void AddCombo(Combo comboInfo)
    {
        if (comboCanvasGroup.alpha == 0f)
        {
            comboActive = true;
            StartComboAnimation();
        }
        else
            NewComboAnimation();

        comboName.text = comboInfo.name;
        comboScore.text = comboInfo.score.ToString();
        currentComboScore += comboInfo.score;

        cooldownBarParent.SetActive(false);
        currentComboCooldown = 0f;
        comboCooldownActive = false;
    }

    public void EndCombo()
    {
        OnComboEnd?.Invoke(currentComboScore);
        currentComboScore = 0;
        comboCooldownActive = false;
        cooldownBarParent.SetActive(false);
        comboActive = false;
        EndComboAnimation();
    }

    private void StartComboAnimation()
    {
        comboCanvasGroup.alpha = 1f;
    }

    private void NewComboAnimation()
    {

    }

    private void EndComboAnimation()
    {
        comboCanvasGroup.alpha = 0f;
    }

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
