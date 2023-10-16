using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TitleScreen : MonoBehaviour
{
    [Tooltip("Reference to Credits Panel")] public GameObject creditsPanel;

    // Start is called before the first frame update
    void Start()
    {
        creditsPanel.SetActive(false);
    }

    public void StartGame(string sceneName)
    {
        GameManager.Instance.LoadScene(sceneName);
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
