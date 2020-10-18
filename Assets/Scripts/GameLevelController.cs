using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LevelController is used to manage overall flow of game. Methods defined in
// LevelController are accessible by referencing LevelController.Instance
public class UnitySingleton<GameLevelController> : MonoBehaviour
{
    bool _paused = false;
    [Header("Pause")]
    public KeyCode pauseKey;
    public GameObject pauseScreen;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void BeginPhaseOne()
    {

    }

    void BeginPhaseTwo()
    {

    }

    // pause/unpause the game
    void TogglePause()
    {
        // start/stop time
        Time.timeScale = _paused ? 0 : 1;

        // display pause screen

    }

    // 
    void Lose()
    {
        // display lose screen
    }

    // executed when the player successfully completes level
    void Win()
    {
       // display win screen
    }

    
}
