using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LevelController is used to manage overall flow of game. Methods defined in
// LevelController are accessible by referencing LevelController.Instance
public class GameLogicController : UnitySingleton<GameLogicController>
{
    bool _paused = false;
    [Header("Pause")]
    public KeyCode pauseKey = KeyCode.Tab;
	public KeyCode pauseKey2 = KeyCode.Escape;
	public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject winScreen;

    [Header("Lighting")]
    public GameObject phase1Lights;

    [Header("Player")]
    public GameObject player;
    public AnimationClip introAnimation;
    public GameObject bossNote;

    [Header("Monster")]
    public GameObject monsterPrefab;
    public Transform monsterSpawn;
    public List<Monster> monsters;

	[Header("Sound")]
	public GameObject musicPlayers;

    
    public enum GamePhase
    {
        PHASE_ONE,
        PHASE_TWO
    };
    [HideInInspector]
    public GamePhase phase;


	private bool introAnimPlaying = false;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        BeginPhaseOne();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey) || Input.GetKeyDown(pauseKey2))
        {
            TogglePause();
        }
    }

    public void BeginPhaseOne()
    {
        phase = GamePhase.PHASE_ONE;
        player.GetComponent<FPController>().RemoveControl();

        foreach (TV tv in MemoryLogicController.Instance.allTVs)
        {
            tv.DisplayState();
        }

        // play opening animation sequence
        player.transform.Find("Camera").GetComponent<Animator>().SetTrigger("Intro");

        // wait for the animation sequence to finish
        // give the player control of character
        StartCoroutine("WaitForIntroAnimationFinish");

		// Enable the 80's music
		musicPlayers.SetActive(true);
	}

    // waits for the intro animation to finish and then gives control to the player
    // displays boss note and plays sound
    IEnumerator WaitForIntroAnimationFinish()
    {
		introAnimPlaying = true;
		yield return new WaitForSeconds(introAnimation.length);
		introAnimPlaying = false;

        // CONTROL IS GIVEN BACK TO PLAYER FROM BossNote.cs
        bossNote.SetActive(true);

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

		// Disable the 80's music
		musicPlayers.SetActive(false);
	}

    // pause/unpause the games
    public void TogglePause()
    {
		if (!_paused && winScreen.activeInHierarchy) return;
		if (bossNote.activeInHierarchy) return;
		if (introAnimPlaying) return;

        _paused = !_paused;

        // lock/unlock cursor
        Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;

        // start/stop time
        Time.timeScale = _paused ? 0 : 1;

		// Mute the game if you pause
		AudioListener.volume = _paused ? 0 : 1;

		// display pause screen
		if (pauseScreen != null) pauseScreen.SetActive(_paused);
        
    }

    public void Lose()
    {
        Animator loseScreenAnimator = loseScreen.GetComponent<Animator>();
        FPController f = player.GetComponent<FPController>();
        // activate cursor
        Cursor.lockState = CursorLockMode.None;
        // lock player in place
        f.forwMovementSpeed = 0;
        f.horzMovementSpeed = 0;
        f.lookspeed = 0;
        // display lose screen
        loseScreen.SetActive(true);
        loseScreenAnimator.SetTrigger("Fade in");
        
    }

    // executed when the player successfully completes level
    // this function is called by ExitDoor.cs when the player tries to exit through
    // front of the store
    public void Win()
    {
		// Pause the game
        Time.timeScale = 0;
		AudioListener.volume = 0;
		// activate cursor
		Cursor.lockState = CursorLockMode.None;
		// Display win screen
		winScreen.SetActive(true);
		Animator winScreenAnimator = winScreen.GetComponent<Animator>();
		winScreenAnimator.SetTrigger("Fade in");
	}   
}
