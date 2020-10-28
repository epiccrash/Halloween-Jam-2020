using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class MonsterSounds : MonoBehaviour {

	public AudioSource staticAudioSource;
	public AudioSource loopAudioSource;

	public List<AudioClip> Static;

	public List<AudioClip> RoamLoops;
	public List<AudioClip> PursueLoops;
	public List<AudioClip> StareLoops;

	public float pitch_min = 0.8f;
	public float pitch_max = 1.2f;

	public float static_pitch_adjust_rate = 0.05f;

	public float roam_dist_per_step = 0.5f;

	private Monster monster;
	private Monster.MonsterState audioState;

	private Vector3 prevPosition = new Vector3();
	private float roamDistCounter = 0.0f;



	// Init
	private void Awake() {
		monster = GetComponent<Monster>();
	}

	// Update is called once per frame
	void Update() {
		staticAudioSource.pitch += Random.Range(-Time.deltaTime * static_pitch_adjust_rate, Time.deltaTime * static_pitch_adjust_rate);
		staticAudioSource.pitch = Mathf.Clamp(staticAudioSource.pitch, pitch_min, pitch_max);

		if (audioState != Monster.MonsterState.ROAM) {
			staticAudioSource.Stop();
		} else if (!staticAudioSource.isPlaying) {
			staticAudioSource.clip = Static[Random.Range(0, Static.Count)];
			staticAudioSource.Play();
		}

		if (audioState != monster.state || !loopAudioSource.isPlaying) {
			audioState = monster.state;

			List<AudioClip> targetLoops = (monster.state == Monster.MonsterState.ROAM) ? RoamLoops :
				(monster.state == Monster.MonsterState.PURSUE) ? PursueLoops :
				(monster.state == Monster.MonsterState.STARE) ? StareLoops :
				throw new System.Exception("No case handled for MonsterState: " + monster.state + " in MonsterSounds.cs");

			if (targetLoops == null) {
				Debug.LogError("No loop provided for MonsterState: " + monster.state + " in MonsterSounds component");
				return;
			}

			// Roaming is now distance-based, not loop based
			if (audioState != Monster.MonsterState.ROAM) {
				loopAudioSource.clip = targetLoops[Random.Range(0, targetLoops.Count)];
				loopAudioSource.pitch = Random.Range(pitch_min, pitch_max);
				loopAudioSource.Play();
			} else {
				loopAudioSource.Stop();
			}
		}

		if (audioState == Monster.MonsterState.ROAM) {
			roamDistCounter += (transform.position - prevPosition).magnitude;
			prevPosition = transform.position;
			if (roamDistCounter > roam_dist_per_step) {
				roamDistCounter = 0;
				loopAudioSource.pitch = Random.Range(pitch_min, pitch_max);
				loopAudioSource.PlayOneShot(RoamLoops[Random.Range(0, RoamLoops.Count)]);
			}
		}
	}
}
