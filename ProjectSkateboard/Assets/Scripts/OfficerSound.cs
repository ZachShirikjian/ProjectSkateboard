using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficerSound : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; private set; }
    void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance?.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.PoliceHey);
    }
}
