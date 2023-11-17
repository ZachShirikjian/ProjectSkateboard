using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelResultsScreenController : MonoBehaviour
{
    [SerializeField, Tooltip("The win screen.")] private RectTransform winScreen;
    [SerializeField, Tooltip("The fail screen.")] private RectTransform failScreen;

    private CanvasGroup resultsCanvasGroup;
    public GameObject continueButton;

    private void Awake()
    {
        resultsCanvasGroup = GetComponent<CanvasGroup>();
        winScreen.gameObject.SetActive(false);
        failScreen.gameObject.SetActive(false);

        LevelManager.OnLevelWin += WinResults;
        LevelManager.OnLevelFailed += FailResults;
        continueButton.SetActive(false);
    }

    private void WinResults()
    {
        GameManager.Instance?.AudioManager.StopAllSounds();
        GameManager.Instance?.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.ResultsWin);
        winScreen.gameObject.SetActive(true);
        continueButton.SetActive(true);
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
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameManager.Instance?.LoadScene("NightScene");
        }
        else if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            GameManager.Instance?.LoadScene("TitleScreen");
        }
    }


    private void OnDestroy()
    {
        LevelManager.OnLevelWin -= WinResults;
        LevelManager.OnLevelFailed -= FailResults;
    }
}
