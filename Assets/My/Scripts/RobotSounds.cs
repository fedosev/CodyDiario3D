using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSounds : MonoBehaviour {

	public AudioClip soundWin;
	public AudioClip soundLose;
	public AudioClip soundStep;

	private AudioSource aSource;

	// Use this for initialization
	void Start() {
		aSource = GetComponent<AudioSource>();
	}

	public void playSound(AudioClip sound) {
		if (aSource)
		 	aSource.PlayOneShot(sound, 1);
	}

	public void playSound(AudioClip sound, float volume) {
		if (aSource)
		 	aSource.PlayOneShot(sound, volume);
	}

	public void playStep() {
		playSound(soundStep, 0.9f);
	}
	public void playWin() {
		playSound(soundWin, 0.9f);
	}
	public void playLose() {
		playSound(soundLose, 0.9f);
	}
	
	// Update is called once per frame
	void Update() {
		
	}
}
