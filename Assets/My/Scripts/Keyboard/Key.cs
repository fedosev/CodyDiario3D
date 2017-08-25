using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[ExecuteInEditMode]
public class Key : MonoBehaviour, IPointerClickHandler {

	public Keyboard keyboard;
	public char letter;

	public void OnPointerClick(PointerEventData eventData) {
		keyboard.HandleClick(letter);
	}

	public void Init() {
		//GetComponentInChildren<Text>().text = letter.ToString();
		keyboard = GetComponentInParent<Keyboard>();
	}

	void Awake () {
		//Init();
	}
	
	void Update () {
		//Init();
	}
}
