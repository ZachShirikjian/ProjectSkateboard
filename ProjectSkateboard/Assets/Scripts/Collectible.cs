using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField, Tooltip("The points for collecting this collectable.")] private int collectableScore = 1000;
    public static Action <int> OnCollectable;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            OnCollectable?.Invoke(collectableScore);
            Destroy(gameObject);
        }
    }
}
