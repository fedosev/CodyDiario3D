using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Unlock : MonoBehaviour, IPointerClickHandler {

	Color color;
	Text text;

	int taps = 0;
	float interval = 0.35f;
	float t = 0f;

	public void OnPointerClick(PointerEventData eventData) {
		/*
		MyDebug.Log(eventData.clickCount, true);
		if (eventData.clickCount >= 7) {
			text.color = Color.red;
			MainGameManager.Instance.Unlock();
			StartCoroutine(ResetProps());
		}
		// */
		//if (taps++)

		if (taps >= 1 && t + interval > Time.time) {
			taps ++;
		} else {
			taps = 1;
		}
		t = Time.time;

		if (taps >= 7) {
			text.color = Color.red;
			MainGameManager.Instance.Unlock();
			StartCoroutine(ResetProps());
		}
		//MyDebug.Log(taps, true);
	}

	IEnumerator ResetProps() {
		yield return new WaitForSeconds(0.5f);
		text.color = color;
		this.enabled = false;
	}

	void Start () {
		text = GetComponent<Text>();
		color = text.color;
	}
}
