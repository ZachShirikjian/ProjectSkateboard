using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Officer : MonoBehaviour
{
        //Reference to the GameTimer script
    public GameTimer timerScript;

    //Reference to LevelManager script
    public LevelManager levelManager;
    // Start is called before the first frame update

    void OnTriggerEnter2D(Collider2D other)
    {           
        if(other.gameObject.tag =="Player")
        {
            Debug.Log("PLAYER GOT CAUGHT!");
            Debug.Log("Set Lvl2 Objective to Fail Automatically");
            timerScript.EndTimer();
            levelManager.levelCleared = false;
            levelManager.EndLevel();

        }
    }
}
