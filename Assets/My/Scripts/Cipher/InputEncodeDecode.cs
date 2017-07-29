using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputEncodeDecode : MonoBehaviour {

	public bool isEncoded;

	public RotCode rotCode;

	public InputField otherInputField;

	InputField inputField;

	public void UpdateText() {
		inputField.text = rotCode.EncodeDecode(otherInputField.text, isEncoded);
	}

	public void UpdateOtherText(string str) {
		if (inputField.isFocused) {
			otherInputField.text = rotCode.EncodeDecode(str, !isEncoded);

		}
	}

	void Awake() {

		inputField = GetComponent<InputField>();
		inputField.onValueChanged.AddListener(UpdateOtherText);

	}

	// Use this for initialization
	void Start () {
		if (isEncoded) {
			rotCode.onCodeChange.AddListener(UpdateText);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
