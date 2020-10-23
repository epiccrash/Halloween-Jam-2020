using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LevelController is used to manage overall flow of game. Methods defined in
// LevelController are accessible by referencing LevelController.Instance
public class GameLogicController : UnitySingleton<GameLogicController>
{
    bool _paused = false;
    [Header("Pause")]
    public KeyCode pauseKey;
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject winScreen;

    [Header("Lighting")]
    public GameObject phase1Lights;

    [Header("Player")]
    public GameObject player;

    [Header("Monster")]
    public GameObject monsterPrefab;
    public Transform monsterSpawn;
    public List<Monster> monsters;
    
    public enum GamePhase
    {
        PHASE_ONE,
        PHASE_TWO
    };
    [HideInInspector]
    public GamePhase phase;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        BeginPhaseOne();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void BeginPhaseOne()
    {
        phase = GamePhase.PHASE_ONE;

        foreach (TV tv in MemoryLogicController.Instance.allTVs)
        {
            tv.DisplayState();
        }

        // play opening animation sequence

        // wait for the animation sequence to finish

        // give the player control of character

    }

    public void BeginPhaseTwo()
    {
        phase = GamePhase.PHASE_TWO;

        foreach(TV tv in MemoryLogicController.Instance.allTVs)
        {
            tv.DisplayState();
        }

        // turn off the lights
        phase1Lights.SetActive(false);

        // spawn the enemy
        GameObject m = Instantiate(monsterPrefab);
        m.transform.position = monsterSpawn.transform.position;
        m.SetActive(true);
    }

    // pause/unpause the games
    void TogglePause()
    {
        _paused = !_paused;

        // lock/unlock cursor
        Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;

        // start/stop time
        Time.timeScale = _paused ? 0 : 1;

        // display pause screen
        if (pauseScreen != null) pauseScreen.SetActive(_paused);
        
    }

    // 
    public void Lose()
    {
        // display lose screen
        Time.timeScale = 0;
        if (loseScreen != null) loseScreen.SetActive(true);
        
    }

    // executed when the player successfully completes level
    // this function is called by ExitDoor.cs when the player tries to exit through
    // front of the store
    public void Win()
    {
        Debug.Log("You win!");
        // display the win screen
        Time.timeScale = 0;
        if (winScreen != null) winScreen.SetActive(true);
    }   
}
