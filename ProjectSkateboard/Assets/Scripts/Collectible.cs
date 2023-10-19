using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    //REFERENCE THE SCORE//
    private ScoreManager scoreManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            // scoreManager.currentScore += 1000;
            // scoreManager.displayScore += 1000;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
