using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISFX : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }


    //Tab References
    public GameObject gamepadControls;
    public GameObject keyboardControls;
    // Start is called before the first frame update
    void Start()
    {
        keyboardControls.SetActive(true);
        gamepadControls.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayHoverSFX()
    {
        GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.MenuButtonOver);
    }

    public void PlayClickSFX()
    {
        GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.MenuButtonClick);
    }

    public void OpenGamepadTab()
    {
        keyboardControls.SetActive(false);
        gamepadControls.SetActive(true);
    }

    public void OpenKeyboardTab()
    {
        keyboardControls.SetActive(true);
        gamepadControls.SetActive(false);
    }
}
