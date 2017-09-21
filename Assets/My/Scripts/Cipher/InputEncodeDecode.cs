using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class InputEncodeDecode : MonoBehaviour, IPointerClickHandler {

	public bool isEncoded;

	public RotCode rotCode;

	public InputField otherInputField;

    public bool isLastFocused = false;

	public Keyboard keyboard;

	public bool isFixed = false;

	InputField inputField;


	InputEncodeDecode otherInputEncodeDecode;


	public void OnPointerClick(PointerEventData eventData) {
		keyboard.Show();
		SetLastFocused();
	}

	public void SetText(string text) {
		inputField.text = text;
	}

	public void SetFixed(bool isFixed) {
		this.isFixed = isFixed;
		if (keyboard == null) {
			inputField.readOnly = isFixed;
		}
	}

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

	public void InitOtherText() {
		otherInputField.text = rotCode.EncodeDecode(inputField.text, !isEncoded);
	}

	public void AppendLetter(char letter) {
		if (isLastFocused && !isFixed) {
			inputField.text += letter;
			inputField.MoveTextEnd(true);
			otherInputField.text = rotCode.EncodeDecode(inputField.text, !isEncoded);
		}
	}

	public void RemoveLastLetter() {
		if (isLastFocused && !isFixed && inputField.text.Length > 0) {
			inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
			otherInputField.text = rotCode.EncodeDecode(inputField.text, !isEncoded);
		}
	}

	void Awake() {

		inputField = GetComponent<InputField>();
		inputField.onValueChanged.AddListener(UpdateOtherText);

		if (keyboard != null) {
			keyboard.onKeyPressed.AddListener(AppendLetter);
			keyboard.onBackspacePressed.AddListener(RemoveLastLetter);
			inputField.readOnly = true;
		}

		inputField.keyboardType = (TouchScreenKeyboardType)(-1);
	}

	// Use this for initialization
	void Start () {

		otherInputEncodeDecode = otherInputField.GetComponent<InputEncodeDecode>();
		if (isEncoded) {
			rotCode.onCodeChange.AddListener(UpdateText);
		}

        inputField.keyboardType = (TouchScreenKeyboardType)(-1);
	}
	
	public void SetLastFocused() {
		otherInputEncodeDecode.isLastFocused = false;
		isLastFocused = true;

	}
	// Update is called once per frame
	void Update () {

		/*
		if (!isFixed && !isLastFocused && inputField.isFocused) {
			SetLastFocused();
			//rotCode.onCodeChange.RemoveListener(otherInputField.GetComponent<InputEncodeDecode>().UpdateText);
			//rotCode.onCodeChange.AddListener(UpdateText);
		}
 		*/
	}
	public void Copy() {
		var input = isLastFocused ? this : otherInputEncodeDecode;
		UniClipboard.SetText(input.inputField.text);
	}

	public void Paste() {
		var input = isLastFocused ? this : otherInputEncodeDecode;
		var text = UniClipboard.GetText();
		text = Regex.Replace(text, @"([^A-Za-z ])", "");
		if (text.Length > 0) {
			input.SetText(text.ToUpper());
			UpdateText();
		}
	}
	
}
