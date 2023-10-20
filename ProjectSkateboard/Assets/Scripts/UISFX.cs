using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISFX : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        
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
        Debug.Log("TEST");

    }
}
