using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    //Reference to CreditsPanel
    public GameObject creditsPanel;
    public GameObject controlsPanel;

    [SerializeField, Tooltip("The local high score text.")] private TextMeshProUGUI highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("LocalHighScore").ToString("n0") + " Points";
    }

    public void StartGame(string sceneName)
    {
        GameManager.Instance?.LoadScene(sceneName);
        GameManager.Instance?.AudioManager.Stop(AudioManager.GameSound.Sound.TitlescreenMusic);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
