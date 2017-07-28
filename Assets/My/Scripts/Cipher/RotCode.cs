using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotCode : MonoBehaviour {

	public RotCylinder fixedRotCylinders;
	public RotCylinder[] rotCylinders;

	public bool withSpace = false;

	public bool isFixed = false;

	public int[] code;

	public UnityEvent onCodeChange;
	

	public void Init() {

		//rotCylinders = FindObjectsOfType<RotCylinder>();
		//@tmp
		SetCode(code);
		foreach (var rc in rotCylinders) {
			rc.onRotNumberChange.AddListener(UpdateCodeFromCylinders);
			if (withSpace) {
				rc.withSpace = true;
				rc.GenerateChars();
			}
			if (isFixed) {
				rc.isFixed = true;
			}
		}
		if (withSpace) {
			fixedRotCylinders.withSpace = true;
			fixedRotCylinders.GenerateChars();
		}
	}

	void Awake() {

		if (onCodeChange == null) {
			onCodeChange = new UnityEvent();
		}
	}

	// Use this for initialization
	void Start () {

		Init();
	}

	void OnEnable() {

	}
	
	public void UpdateCodeFromCylinders() {
		var i = 0;
		foreach (var rc in rotCylinders) {
			code[i++] = rc.RotNumber;
		}
		if (onCodeChange != null) {
			onCodeChange.Invoke();
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
