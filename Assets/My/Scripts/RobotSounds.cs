using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSounds : MonoBehaviour {

	public AudioClip soundWin;
	public AudioClip soundLose;
	public AudioClip soundStep;
	public AudioClip soundMoving;

	private AudioSource aSource;

	// Use this for initialization
	void Start() {
		aSource = GetComponent<AudioSource>();
	}

	public void PlaySound(AudioClip sound) {
		if (aSource)
		 	aSource.PlayOneShot(sound, 1);
	}

	public void PlaySound(AudioClip sound, float volume) {
		if (aSource)
		 	aSource.PlayOneShot(sound, volume);
	}

	public void PlayStep() {
		//playSound(soundStep, 0.9f);
		aSource.clip = soundMoving;
		aSource.Play();
	}

	public void PlayMoving() {
		//playSound(soundStep, 0.9f);
		aSource.clip = soundMoving;
		aSource.Play();
	}

	public void StopPlaying() {
		aSource.Stop();
	}

	public void playWin() {
		PlaySound(soundWin, 0.9f);
	}

	public void playLose() {
		PlaySound(soundLose, 0.9f);
	}
	
	// Update is called once per frame
	void Update() {
		
	}
}
