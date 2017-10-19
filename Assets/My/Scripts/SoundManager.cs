using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

	private static SoundManager instance;
	public static SoundManager Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<SoundManager>();
		return instance;
	} }
	
	public AudioSource audioSource;
	public SoundConfig sounds;

	public void Play(AudioClip sound, float from = 0f) {
		audioSource.clip = sound;
		if (from > 0)
			audioSource.time = from;
		audioSource.Play();
	}

	public void PlayMenu() {
		audioSource.PlayOneShot(sounds.menu);
	}

	public void PlayStar() {
		Play(sounds.star, 0.25f);
	}

	public void PlayWin() {
		return;
		audioSource.PlayOneShot(sounds.win);
	}

	void Awake() {
		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();
	}

}
