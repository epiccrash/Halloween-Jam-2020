using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// control overall flow of memory game
public class MemoryLogicController : UnitySingleton<MemoryLogicController>
{
    List<TV> goodTVs;
    List<TV> evilTVs;

    public int TVsRemaining;

    private void Start()
    {
        goodTVs = new List<TV>();
        evilTVs = new List<TV>();
        // initialize goodTVs and evilTVs lists
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("TV"))
        {
            Debug.Log("tvloop");
            TV tv = g.GetComponent<TV>();
            if (tv.isEvil)
            {
                Debug.Log("add an evil TV");
                evilTVs.Add(tv);
            }
            else
            {
                Debug.Log("Add a good tv");
                goodTVs.Add(tv);
            }
        }
        TVsRemaining = goodTVs.Count;
        Debug.Log(goodTVs.Count);
    }

    // Executed when the player unplugs an evil TV
    public void UnplugEvil(TV tv)
    {
        // alert the enemy? spawn new enemy?
        evilTVs.Remove(tv);
    }

    // Executed when the player unplugs a good TV
    public void UnplugGood(TV tv)
    {
        goodTVs.Remove(tv);
        TVsRemaining--;
    }

    // Returns true when the player has unplugged all TVs correctly, false otherwise
    public bool TVsUnpluggedCorrectly()
    {
        return TVsRemaining == 0;
    }
}

