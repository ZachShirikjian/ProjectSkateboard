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
        winScreen.gameObject.SetActive(true);
    }

    private void FailResults()
    {
        failScreen.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelWin -= WinResults;
        LevelManager.OnLevelFailed -= FailResults;
    }
}
