using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    // public bool altTrackSelected;
    // public bool altTrack2Selected;
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    //Reference to CreditsPanel
    public GameObject creditsPanel;
    public GameObject controlsPanel;
    public GameObject musicMenu;

    [SerializeField, Tooltip("The local high score text.")] private TextMeshProUGUI highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        musicMenu.SetActive(false);

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

    public void OpenMusicMenu()
    {
        musicMenu.SetActive(true);
    }
    public void CloseMusicMenu()
    {
        musicMenu.SetActive(false);
    }

    public void SwitchMusic()
    {
        Debug.Log("Play PITCH BLACK");
        if( GameManager.Instance?.altTrackSelected == false)
        {
            GameManager.Instance.altTrackSelected = true;
        }
        else if( GameManager.Instance?.altTrackSelected == true)
        {
            GameManager.Instance.altTrackSelected = false;
        }
    }

    public void SwitchMusic2()
    {
        Debug.Log("Play RIDE THE RAILS");
        if(GameManager.Instance?.altTrackSelected2 == false)
        {
            GameManager.Instance.altTrackSelected2 = true;
        }
        else if( GameManager.Instance?.altTrackSelected2 == true)
        {
            GameManager.Instance.altTrackSelected2 = false;
        }
    }
}
