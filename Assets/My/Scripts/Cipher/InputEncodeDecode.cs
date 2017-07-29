using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputEncodeDecode : MonoBehaviour {

	public bool isEncoded;

	public RotCode rotCode;

	public InputField otherInputField;

    public bool isLastFocused = false;

	InputField inputField;

	InputEncodeDecode otherInputEncodeDecode;

    public void UpdateText() {
		if (isLastFocused) {
			 otherInputField.text = rotCode.EncodeDecode(inputField.text, !isEncoded);
		} else {
			inputField.text = rotCode.EncodeDecode(otherInputField.text, isEncoded);
		}
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

		otherInputEncodeDecode = otherInputField.GetComponent<InputEncodeDecode>();
		if (isEncoded) {
			rotCode.onCodeChange.AddListener(UpdateText);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (!isLastFocused && inputField.isFocused) {
			otherInputEncodeDecode.isLastFocused = false;
			isLastFocused = true;
			//rotCode.onCodeChange.RemoveListener(otherInputField.GetComponent<InputEncodeDecode>().UpdateText);
			//rotCode.onCodeChange.AddListener(UpdateText);
		}

	}
}
