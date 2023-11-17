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
    public GameObject galleryImage1;

    //Reference to PreviewImage GameObject
    public Image previewImage;
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

    //EXTRAS MENU//
    public void OpenExtras()
    {
        extrasPanel.SetActive(true);
    }

    public void CloseExtras()
    {
        extrasPanel.SetActive(false);
        musicMenu.SetActive(false);
        conceptArtMenu.SetActive(false);
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
        if (GameManager.Instance?.altTrackSelected == false)
        {
            GameManager.Instance.altTrackSelected = true;
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
        }
        else if (GameManager.Instance?.altTrackSelected2 == true)
        {
            GameManager.Instance.altTrackSelected2 = false;
        }
    }


    public void SwitchMusic3()
    {
        Debug.Log("Play AMEN TYPE BEAT");
        if (GameManager.Instance?.altTrackSelected3 == false)
        {
            GameManager.Instance.altTrackSelected3 = true;
        }
        else if (GameManager.Instance?.altTrackSelected3 == true)
        {
            GameManager.Instance.altTrackSelected3 = false;
        }
    }

    public void OpenGallery()
    {
        conceptArtMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(galleryImage1);
    }

    public void CloseGallery()
    {
        conceptArtMenu.SetActive(false);
    }

    public void ChangePreviewImage()
    {
        // EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject);
        previewImage.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
    }

    public void CloseFullScreen()
    {
        fullSizePanel.SetActive(false);
        // EventSystem.current.SetSelectedGameObject(galleryImage1);
    }
}
