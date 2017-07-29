using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotNumberText : MonoBehaviour {

	public string textPrefix = "ROT";
	
	Text text;
	
	public RotCode rotCode;

	public void UpdateText() {

		if (rotCode != null) {
			text.text = textPrefix;
			foreach (var c in rotCode.code) {
				text.text += "-" + c;
			}
		}
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
