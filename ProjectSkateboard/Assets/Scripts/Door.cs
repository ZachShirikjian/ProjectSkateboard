using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
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
            Debug.Log("PLAYER REACHED EXIT!");
            Debug.Log("Set Lvl2 Objective to Win Automatically");
            timerScript.EndTimer();
            levelManager.levelCleared = true;
            levelManager.EndLevel();

        }
    }
}
