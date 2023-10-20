using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseController : MonoBehaviour
{
    public enum PauseMenu { Main, Controls }

    [SerializeField, Tooltip("The pause menu GameObjects.")] private GameObject[] pauseMenus;
    [SerializeField, Tooltip("The pause menu selectable GameObjects.")] private Selectable[] onMenuSelected;

    private PauseMenu currentMenu;

    private PlayerControls playerControls;
    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.UI.Cancel.performed += _ => Cancel();
    }

    private void Start()
    {
        ClearMenus();
    }

    private void OnEnable()
    {
        playerControls?.Enable();
        PlayerController.OnPaused += Pause;
        EventSystem.current.SetSelectedGameObject(onMenuSelected[(int)PauseMenu.Main].gameObject);
    }

    private void OnDisable()
    {
        playerControls?.Disable();
        PlayerController.OnPaused -= Pause;
        EventSystem.current?.SetSelectedGameObject(null);
    }

    private void Pause()
    {
        if (LevelManager.Instance.IsGameActive())
        {
            if (!LevelManager.Instance.isGamePaused)
            {
                LevelManager.Instance.isGamePaused = true;
                GameManager.Instance?.AudioManager.PauseAllSounds();
                Time.timeScale = 0f;
                OpenMenu(PauseMenu.Main);
            }
            else if(currentMenu == PauseMenu.Main)
                Resume();
        }
    }

    public void Resume()
    {
        if (LevelManager.Instance.isGamePaused && currentMenu == PauseMenu.Main)
        {
            LevelManager.Instance.isGamePaused = false;
            GameManager.Instance?.AudioManager.ResumeAllSounds();
            Time.timeScale = 1f;
            ClearMenus();
        }
    }

    public void Restart()
    {
        Resume();
        GameManager.Instance?.AudioManager.StopAllSounds();
        GameManager.Instance?.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ControlsMenu()
    {
        OpenMenu(PauseMenu.Controls);
    }

    public void Cancel()
    {
        if (currentMenu != PauseMenu.Main)
            OpenMenu(PauseMenu.Main);
        else
            Resume();
    }

    public void BackToMain()
    {
        Resume();
        GameManager.Instance?.AudioManager.StopAllSounds();
        GameManager.Instance?.LoadScene("TitleScreen");
    }

    private void OpenMenu(PauseMenu newMenu)
    {
        if(newMenu != currentMenu)
        {
            pauseMenus[(int)currentMenu].SetActive(false);
            currentMenu = newMenu;
        }

        pauseMenus[(int)currentMenu].SetActive(true);
        EventSystem.current.SetSelectedGameObject(onMenuSelected[(int)currentMenu]?.gameObject);
    }

    private void ClearMenus()
    {
        foreach (GameObject menu in pauseMenus)
            menu.SetActive(false);
    }
}
