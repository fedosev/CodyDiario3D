using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotCode : MonoBehaviour {

	public RotCylinder fixedRotCylinders;
	public RotCylinder[] rotCylinders;

	public bool withSpace = false;

	public int[] code;

	// Use this for initialization
	void Start () {

		//rotCylinders = FindObjectsOfType<RotCylinder>();
		//@tmp
		SetCode(code);
		foreach (var rc in rotCylinders) {
			rc.onRotNumberChange.AddListener(UpdateCodeFromCylinders);
			if (withSpace) {
				rc.withSpace = true;
				rc.GenerateChars();
			}
		}
		if (withSpace) {
			fixedRotCylinders.withSpace = true;
			fixedRotCylinders.GenerateChars();
		}
	}

	void OnEnable() {

	}
	
	public void UpdateCodeFromCylinders() {
		var i = 0;
		foreach (var rc in rotCylinders) {
			code[i++] = rc.RotNumber;
		}
	
	}

	public void SetCode(int[] code) {
		this.code = code;
		var i = 0;
		foreach (var rc in rotCylinders) {
			rc.SetRotNumber(code[i++]);
		}
	}

	public int[] GetCode() {
		return code;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
