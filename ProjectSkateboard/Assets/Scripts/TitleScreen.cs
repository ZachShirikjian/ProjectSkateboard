using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    //Reference to CreditsPanel
    public GameObject creditsPanel;
    public GameObject controlsPanel;
    // Start is called before the first frame update
    void Start()
    {
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void StartGame(string sceneName)
    {
        GameManager.Instance.LoadScene(sceneName);
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
