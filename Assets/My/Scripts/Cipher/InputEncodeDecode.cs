using System;
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

	public event Action<char> OnRemoveLetter;
	public event Action<char> OnAddLetter;

	bool isEditSequenceMode = false;

	InputField inputField;
	InputEncodeDecode otherInputEncodeDecode;

    float tKeyPressed = float.MaxValue;
    string placeholderText;


    public void OnPointerClick(PointerEventData eventData) {
		if (isEditSequenceMode) {
			keyboard.Show();
			isLastFocused = isEncoded;
		} else if (!isFixed) {
			keyboard.Show();
			SetLastFocused();
		}
	}

	public InputEncodeDecode GetField(bool encodedField) {
		if (isEncoded == encodedField)
			return this;
		return otherInputEncodeDecode;
	}

	public void SetEditSequenceMode(bool isOn) {
		isLastFocused = isEncoded;
		isEditSequenceMode = isOn;
	}

	public void SetText(string text) {
		inputField.text = text;
	}

	public string GetText() {
		return inputField.text;
	}

	public void SetFixed(bool isFixed) {
		this.isFixed = isFixed;
		if (keyboard == null) {
			inputField.readOnly = isFixed;
		}
	}

	string EncodeDecode(string str, bool encode) {

		if (!isEditSequenceMode) {
			return rotCode.EncodeDecode(str, encode);
		} else {
			if (encode) {
				return str;
			} else {
				var chars = new char[str.Length];
				for (int i = 0; i < str.Length && i < 26; i++) {
					chars[i] = (char)(65 + i);
				}
				for (int i = 26; i < str.Length; i++) {
					chars[i] = ' ';
				}
				return new string(chars);
			}
		}
	}

    public void UpdateText() {
		if (isLastFocused) {
			 otherInputField.text = EncodeDecode(inputField.text, !isEncoded);
		} else {
			inputField.text = EncodeDecode(otherInputField.text, isEncoded);
		}
	}

	public void UpdateOtherText(string str) {
		if (inputField.isFocused) {
			otherInputField.text = EncodeDecode(str, !isEncoded);
		}
	}

	public void InitOtherText() {
		otherInputField.text = EncodeDecode(inputField.text, !isEncoded);
	}

	public void AppendLetter(char letter) {
		if (isEditSequenceMode) {
			isLastFocused = isEncoded;
			if (inputField.text.Length >= 26 + (rotCode.withSpace ? 1 : 0))
				return;
		}
		if (isLastFocused && !isFixed) {
			inputField.text += letter;
			inputField.MoveTextEnd(true);
			otherInputField.text = EncodeDecode(inputField.text, !isEncoded);
			tKeyPressed = Time.time;
			if (OnAddLetter != null) {
				OnAddLetter(letter);
			}
		}
		UpdateOkButton();
	}

	public void RemoveLastLetter() {
		if (isLastFocused && !isFixed && inputField.text.Length > 0) {
			var lastIndex = inputField.text.Length - 1;
			if (OnRemoveLetter != null) {
				OnRemoveLetter(inputField.text[lastIndex]);
			}
			inputField.text = inputField.text.Substring(0, lastIndex);
			otherInputField.text = EncodeDecode(inputField.text, !isEncoded);
		}
		UpdateOkButton();
	}

	void UpdateOkButton() {
		if (isEditSequenceMode) {
			keyboard.okButton.SetActive(inputField.text.Length == 26 + (rotCode.withSpace ? 1 : 0));
		}
	}

	void Awake() {

		inputField = GetComponent<InputField>();
		otherInputEncodeDecode = otherInputField.GetComponent<InputEncodeDecode>();
	}

	public void Init() {

		//inputField.onValueChanged.AddListener(UpdateOtherText);

		if (keyboard != null) {
			keyboard.onKeyPressed.AddListener(AppendLetter);
			keyboard.onBackspacePressed.AddListener(RemoveLastLetter);
			inputField.readOnly = true;
		}

		inputField.keyboardType = (TouchScreenKeyboardType)(-1);

		if (isEncoded) {
			rotCode.onCodeChange.AddListener(UpdateText);
		}

        inputField.keyboardType = (TouchScreenKeyboardType)(-1);
	}

	public void SetPlaceholder(string text) {
		if (placeholderText == null)
			placeholderText = ((Text)inputField.placeholder).text;
		((Text)inputField.placeholder).text = text;
	}

	public void ResetPlaceholder() {
		if (placeholderText != null)
			((Text)inputField.placeholder).text = placeholderText;
	}
	
	public void SetLastFocused() {
		otherInputEncodeDecode.isLastFocused = false;
		isLastFocused = true;

	}

	void SetDebugMode(bool isOn) {
		if (MainGameManager.Instance)
			MainGameManager.Instance.SetDebugMode(isOn);
		inputField.text = "DEBUG";
		otherInputField.text = rotCode.EncodeDecode(inputField.text, !isEncoded);
	}

	// Update is called once per frame
	void Update () {

		#if F_ALLOW_DEBUG
			if (!isEncoded && tKeyPressed + 1f < Time.time) {
				if (inputField.text == "DEBUGFFF") {
					SetDebugMode(true);
				} else if (inputField.text == "DEBUGOFF") {
					SetDebugMode(false);
				}
				tKeyPressed = float.MaxValue;
			}
		#endif
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
		text = text.ToUpper();
		// @todo...
		if (isEditSequenceMode) {
			var i = 0;
			foreach (var chr in text) {
				Key key;
				if (keyboard.TryGetKey(chr, out key)) {
					if (key.button.interactable) {
						input.AppendLetter(chr);
						key.button.interactable = false;
					}
					i++;
				}
				if (i > 27)
					break;
			}
		} else {
			if (text.Length > 0) {
				input.SetText(text);
				input.UpdateText();
			}
		}
	}
	
}
