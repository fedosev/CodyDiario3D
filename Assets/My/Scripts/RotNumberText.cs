using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotNumberText : MonoBehaviour {

	public string textPrefix = "ROT";
	
	Text text;
	
	public RotCylinder[] rotCylinders;

	public void UpdateText() {

		if (rotCylinders != null) {
			text.text = textPrefix;
			foreach (var rc in rotCylinders) {
				text.text += "-" + rc.RotNumber;
			}
		}
	}

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		if (rotCylinders != null) {
			text.text = textPrefix;
			foreach (var rc in rotCylinders) {
				rc.onRotNumberChange.AddListener(UpdateText);
			}
		}
		UpdateText();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
