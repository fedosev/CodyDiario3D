using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {

	public Animator animator;
	public Text title;
	public GameObject buttonRetry;
	public GameObject buttonPlayAgain;

	public bool isVisible;

	readonly int isActiveHash = Animator.StringToHash("isActive");


	public void Init() {
		gameObject.SetActive(false);
		isVisible = false;
	}
	
	public void ShowWin() {
		title.text = "OTTIMO!";
		buttonRetry.SetActive(false);
		buttonPlayAgain.SetActive(true);
		Show();
	}

	public void ShowLose() {
		title.text = "Ooops...";
		buttonRetry.SetActive(true);
		buttonPlayAgain.SetActive(false);
		Show();
	}

	void Show() {
		if (!isVisible)
			animator.SetBool(isActiveHash, false);

		gameObject.SetActive(true);
		animator.SetBool(isActiveHash, true);

		isVisible = true;
	}

	public void Hide() {
		if (!isVisible)
			return;

		animator.SetBool(isActiveHash, false);
		gameObject.SetActive(false);

		isVisible = false;
	}	

}
