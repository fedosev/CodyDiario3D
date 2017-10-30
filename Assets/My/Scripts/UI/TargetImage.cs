using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetImage : MonoBehaviour {

	public float fadeSpeed = 2f;
	public float maxOpacity = 1f;
	public AnimationCurve curve;

	float tEnabled = 0f;
	CanvasGroup canvasGroup;
    float alpha;

    void OnEnable() {
		tEnabled = Time.time;
		alpha = 0f;
		canvasGroup.alpha = 0f;
	}

	void Awake() {
		canvasGroup = GetComponent<CanvasGroup>();
	}

	void Update () {
		if (alpha < maxOpacity) {
			canvasGroup.alpha = curve.Evaluate((Time.time - tEnabled) * fadeSpeed) * maxOpacity;
			alpha = canvasGroup.alpha;
		}
	}
}
