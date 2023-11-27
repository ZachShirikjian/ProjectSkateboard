using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TitleScreen : MonoBehaviour
{
    // public bool altTrackSelected;
    // public bool altTrack2Selected;
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    //Reference to CreditsPanel
    public GameObject creditsPanel;
    public GameObject controlsPanel;
    public GameObject extrasPanel;
    public GameObject musicMenu;
    public GameObject conceptArtMenu;
    public GameObject fullSizePanel;
    public GameObject playButton;
    public GameObject creditsButton;
    public GameObject extrasButton;
    public GameObject controlsButton;
    public GameObject musicButton;
    public GameObject galleryButton;
    public GameObject track1Button;
    public GameObject quitCreditsButton;
    public GameObject img1Button;
    public GameObject keyboardButton;

    [SerializeField, Tooltip("The local high score text.")] private TextMeshProUGUI highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        extrasPanel.SetActive(false);
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        musicMenu.SetActive(false);
        conceptArtMenu.SetActive(false);
        fullSizePanel.SetActive(false);
        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("LocalHighScore").ToString("n0") + " Points";
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public void StartGame(string sceneName)
    {
        GameManager.Instance?.LoadScene(sceneName);
        GameManager.Instance?.AudioManager.Stop(AudioManager.GameSound.Sound.TitlescreenMusic);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(keyboardButton);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(controlsButton);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(quitCreditsButton);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
         EventSystem.current.SetSelectedGameObject(creditsButton);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    //EXTRAS MENU//
    public void OpenExtras()
    {
        extrasPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(musicButton);
    }

    public void CloseExtras()
    {
        extrasPanel.SetActive(false);
        musicMenu.SetActive(false);
        conceptArtMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(extrasButton);
    }
    public void OpenMusicMenu()
    {
        musicMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(track1Button);
    }
    public void CloseMusicMenu()
    {
        musicMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(musicButton);
    }

    public void SwitchMusic()
    {
        Debug.Log("Play PITCH BLACK");
        if (GameManager.Instance?.altTrackSelected == false)
        {
            GameManager.Instance.altTrackSelected = true;
            GameManager.Instance.altTrackSelected2 = false;
            GameManager.Instance.altTrackSelected3 = false;
        }
        else if (GameManager.Instance?.altTrackSelected == true)
        {
            GameManager.Instance.altTrackSelected = false;
        }
    }

    public void SwitchMusic2()
    {
        Debug.Log("Play RIDE THE RAILS");
        if (GameManager.Instance?.altTrackSelected2 == false)
        {
            GameManager.Instance.altTrackSelected2 = true;
            GameManager.Instance.altTrackSelected = false;
            GameManager.Instance.altTrackSelected3 = false;
        }
        else if (GameManager.Instance?.altTrackSelected2 == true)
        {
            GameManager.Instance.altTrackSelected2 = false;
        }
    }


    public void SwitchMusic3()
    {
        Debug.Log("Play HEARTBREAK BLUES");
        if (GameManager.Instance?.altTrackSelected3 == false)
        {
            GameManager.Instance.altTrackSelected3 = true;
            GameManager.Instance.altTrackSelected2 = false;
            GameManager.Instance.altTrackSelected = false;
        }
        else if (GameManager.Instance?.altTrackSelected3 == true)
        {
            GameManager.Instance.altTrackSelected3 = false;
        }
    }

    public void OpenGallery()
    {
        conceptArtMenu.SetActive(true);
        fullSizePanel.SetActive(false);
        extrasPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(img1Button);
    }

    public void CloseGallery()
    {
        conceptArtMenu.SetActive(false);
        extrasPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(galleryButton);
    }


    public void CloseFullScreen()
    {
        fullSizePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(img1Button);
    }
}
