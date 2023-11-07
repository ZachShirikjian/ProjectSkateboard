using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelResultsScreenController : MonoBehaviour
{
    [SerializeField, Tooltip("The win screen.")] private RectTransform winScreen;
    [SerializeField, Tooltip("The fail screen.")] private RectTransform failScreen;

    private CanvasGroup resultsCanvasGroup;

    private void Awake()
    {
        resultsCanvasGroup = GetComponent<CanvasGroup>();
        winScreen.gameObject.SetActive(false);
        failScreen.gameObject.SetActive(false);

        LevelManager.OnLevelWin += WinResults;
        LevelManager.OnLevelFailed += FailResults;
    }

    private void WinResults()
    {
        GameManager.Instance?.AudioManager.StopAllSounds();
        GameManager.Instance?.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.ResultsWin);
        winScreen.gameObject.SetActive(true);
    }

    private void FailResults()
    {
        GameManager.Instance?.AudioManager.StopAllSounds();
        GameManager.Instance?.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.ResultsFail);
        failScreen.gameObject.SetActive(true);
    }

    public void BackToMain()
    {
        GameManager.Instance?.AudioManager.StopAllSounds();
        GameManager.Instance?.LoadScene("TitleScreen");
    }

    public void Continue()
    {
        GameManager.Instance?.AudioManager.StopAllSounds();
        GameManager.Instance?.LoadScene("NightScene");
    }


    private void OnDestroy()
    {
        LevelManager.OnLevelWin -= WinResults;
        LevelManager.OnLevelFailed -= FailResults;
    }
}
