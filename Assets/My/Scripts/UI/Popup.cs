using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {

	public Animator animator;
	public Image image;
	public Text title;
	public GameObject buttonRetry;
	public GameObject buttonPlayAgain;

	public Color winColor;
	public Color loseColor;

	public Color overlayColor = new Color(1f, 1f, 1f, 0f);
	public float overlayAlpha = 0.1f;
	public float showDelay = 0.5f;
	public float fadeSpeed = 1f;

	public string[] winTitles = {
		"BRAVO!", "OTTIMO!", "PERFETTO!", "COMPLIMENTI!", "BEN FATTO!"
	};


	public bool isVisible = false;

	static readonly int isActiveHash = Animator.StringToHash("isActive");
	static readonly int popupInHash = Animator.StringToHash("PopupIn");


	public void Init() {
		gameObject.SetActive(false);
		isVisible = false;
	}
	
	public void ShowWin() {
		title.color = winColor;
		title.text = winTitles[Random.Range(0, winTitles.Length)];
		buttonRetry.SetActive(false);
		buttonPlayAgain.SetActive(true);
		Show();
	}

	public void ShowLose() {
		title.color = loseColor;
		title.text = "Ooops...";
		buttonRetry.SetActive(true);
		buttonPlayAgain.SetActive(false);
		Show();
	}

	void Show() {
		//StopAllCoroutines();

		image.color = new Color();

		gameObject.SetActive(true);

		if (!isVisible)
			animator.SetBool(isActiveHash, false);
		StartCoroutine(ShowCoroutine());
	}

	IEnumerator ShowCoroutine() {

		yield return new WaitForSeconds(showDelay);
		StartCoroutine(FadeIn());

		animator.SetBool(isActiveHash, true);

		isVisible = true;
	}

	IEnumerator FadeIn() {
		var color = image.color;
		while(color.a <= overlayAlpha - 0.01) {
			color.a += (overlayAlpha - color.a) * fadeSpeed * Time.deltaTime;
			image.color = color;
			yield return null;
		}
	}

	public void Hide() {
		if (!isVisible)
			return;

		animator.SetBool(isActiveHash, false);
		isVisible = false;
		gameObject.SetActive(false);
	}	

	void OnEnable() {
		if (isVisible) {
			//animator.SetBool(isActiveHash, true);
			animator.Play(popupInHash, 0, 1f);
		} else {
			image.color = overlayColor;
		}
	}

	void OnDisable() {
		StopAllCoroutines();
	}

}
