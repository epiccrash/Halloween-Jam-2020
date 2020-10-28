﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//public class SceneTransitionManager : UnitySingletonPersistent<SceneTransitionManager> {
public class SceneTransitionManager : MonoBehaviour {
    public void LoadScene(string name)
    {
        Time.timeScale = 1;
		AudioListener.volume = 1;
		SceneManager.LoadScene(name);
    }
}
