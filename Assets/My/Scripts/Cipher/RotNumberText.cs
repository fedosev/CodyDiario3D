using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotNumberText : MonoBehaviour {

	public string textPrefix = "ROT";
	public string textPrefixVar = "VAR";
	
	public RotCode rotCode;

	Text text;

	public void UpdateText() {

		if (rotCode != null) {
			bool useSeparator = false;
			if (rotCode.sequence != null && rotCode.sequence.Length > 0) {
				text.text = rotCode.sequence.Substring(0, 10) + "...";
				return;
			}
			if (rotCode.code.Length > 1) {
				text.text = textPrefixVar;
				foreach (var c in rotCode.code) {
					if (c > 9) {
						useSeparator = true;
						break;
					}
				}
			} else {
				text.text = textPrefix;
			}
			foreach (var c in rotCode.code) {
				if (useSeparator)
					text.text += '-';
				text.text += c;
			}
			if (rotCode.withSpace) {
				text.text += "s";
			}
				
		}
	}

	public void SetText(string text) {
		this.text.text = text;
	}

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		if (rotCode != null) {
			rotCode.onCodeChange.AddListener(UpdateText);
		}
		UpdateText();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
