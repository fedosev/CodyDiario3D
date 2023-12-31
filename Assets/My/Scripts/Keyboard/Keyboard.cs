﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventChar : UnityEvent<char> { }

public class Keyboard : MonoBehaviour {

	public const char space = ' ';
	public const char backspace = '<';

	public UnityEventChar onKeyPressed = new UnityEventChar();
	public UnityEvent onBackspacePressed = new UnityEvent();
    public float posY = 18f;
    public float posYHidden;
	public float animDuration = 0.5f;

	public bool couldBeHidden = true;
	public bool isHiddenOnStart = true;

	public GameObject copyButton;
	public GameObject pasteButton;
	public GameObject okButton;

	bool isAnimating = false;
    public bool useRT = false;

	float height;
    float rectRatio;

	bool isVisible = true;

    RectTransform rt;

	Key[] keys;
	Dictionary<char, Key> keyChar;

	public Key[] GetKeys() {
		if (keys == null)
			keys = GetComponentsInChildren<Key>();
		return keys;		
	}

	public bool TryGetKey(char letter, out Key key) {
		if (keyChar == null) {
			keyChar = new Dictionary<char, Key>();
			foreach(var k in GetKeys()) {
				keyChar.Add(k.letter, k);
			}
		}
		if (keyChar.TryGetValue(letter, out key)) {
			return true;
		}
		return false;
	}

	public void SetInteractable(bool b) {
		foreach (var key in GetKeys()) {
			key.button.interactable = b;
		}
	}

    public void HandleClick(char letter) {

		//MyDebug.Log(letter, true);

		if (letter == space) {
			letter = ' ';
		}
		if (letter == backspace) {
			onBackspacePressed.Invoke();
		} else {
			onKeyPressed.Invoke(letter);
		}
	}

	public void Show() {
		if (gameObject == null)
			return;
		if (!isAnimating && !isVisible) {
			gameObject.SetActive(true);
			StartCoroutine(ShowAnimated(true));
			isVisible = true;
		}
	}

	public void Hide(bool immediate = false) {
		if (couldBeHidden && !isAnimating && isVisible) {
			if (immediate)
				StartCoroutine(ShowAnimated(false, 0));
			else
				StartCoroutine(ShowAnimated(false));
		}
	}

	IEnumerator ShowAnimated(bool show, float duration = -1f) {
		
		if (duration < 0f)
			duration = animDuration;
		isAnimating = true;
		var t0 = Time.time;
		var t = 0f;
		float v = 0f;
		float animFreq = 0f;
		if (duration > 0)
			animFreq = 1 / duration;
		var rectRatioInv = 1f / rectRatio;
		while (t <= duration + Time.deltaTime) {
			v = (t * animFreq);
			v = Mathf.Clamp01(v);

			if (duration == 0f)
				v = 1f;

			//v = v < 0.5f ? 2 * v * v : -1 + (4 - 2 * v) * v; // easeInOutQuad
			//v *= v; // EaseInQuad
			//v = v * (2 - v); // EaseOutQuad
			//v = (--v) * v * v + 1; // easeOutCubic
			v = 1 - (--v) * v * v * v; //easeOutQuart
			if (!show) {
				v = 1f - v;
			}
			if (useRT) {
				rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, (posYHidden + (posY - posYHidden) * v) * rectRatioInv);
			} else {
				transform.position = new Vector3(
					transform.position.x,
					posYHidden + (posY - posYHidden) * v,
					transform.position.z
				);
			}
			t = Time.time - t0;
			yield return null;
		}
		if (!show)
			gameObject.SetActive(false);
		isVisible = show;
		isAnimating = false;
	}

	void Start() {
		
		rt = GetComponent<RectTransform>();
		posY = transform.position.y;
		rectRatio = posY / rt.anchoredPosition.y * transform.localScale.y;
		posYHidden = - rectRatio * rt.rect.height - posY;

		if (okButton)
			okButton.SetActive(false);
		if (isHiddenOnStart)
			Hide(true);
	}

	void Update() {

		#if UNITY_EDITOR
			foreach (char c in Input.inputString) {
				if (c == '\b') {
					onBackspacePressed.Invoke();
				} else {
					Key key;
					if (TryGetKey(c, out key)) {
						if (!key.button.interactable)
							return;
						onKeyPressed.Invoke(c);
					}
				}
			}
		#endif
	}
	

}
