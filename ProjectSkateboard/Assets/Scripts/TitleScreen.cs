using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    //Reference to CreditsPanel
    public GameObject creditsPanel;
    // Start is called before the first frame update
    void Start()
    {
        creditsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
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
    }
}
