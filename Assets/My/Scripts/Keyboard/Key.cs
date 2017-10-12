using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[ExecuteInEditMode]
public class Key : MonoBehaviour, IPointerClickHandler {

	public Keyboard keyboard;
	public char letter;

	public Button button;

	public void OnPointerClick(PointerEventData eventData) {
		if (button.interactable) {
			keyboard.HandleClick(letter);
		}
	}

	public void Init() {
		//GetComponentInChildren<Text>().text = letter.ToString();
		keyboard = GetComponentInParent<Keyboard>();
	}

	void Awake () {
		button = GetComponent<Button>();
	}
	
	void Update () {
		//Init();
	}
}
