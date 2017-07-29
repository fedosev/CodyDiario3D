using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class RotCode : MonoBehaviour {

	public RotCylinder fixedRotCylinder;
	public RotCylinder[] rotCylinders;

	public bool withSpace = false;

	public bool isFixed = false;

	public int[] code;

	public UnityEvent onCodeChange;
	

	public void Init() {

		//rotCylinders = FindObjectsOfType<RotCylinder>();
		foreach (var rc in rotCylinders) {
			if (withSpace) {
				rc.withSpace = true;
				rc.GenerateChars();
			}
			if (isFixed) {
				rc.isFixed = true;
			}
		}
		if (withSpace) {
			fixedRotCylinder.withSpace = true;
			fixedRotCylinder.GenerateChars();
		}
		SetCode(code);
		foreach (var rc in rotCylinders) {
			rc.onRotNumberChange.AddListener(UpdateCodeFromCylinders);
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

	public string EncodeDecode(string str, bool encode) {

		var n = 26 + (withSpace ? 1 : 0);
		var chars =  str.ToCharArray();
		byte[] ascii = Encoding.ASCII.GetBytes(str);
		var j = 0; // code index
		for (var i = 0; i < str.Length; i++) {
			if (ascii[i] >= 97) {
				ascii[i] -= 32; // Uppercase
			}
			if (chars[i] != ' ' || withSpace) {
				if (chars[i] == ' ') {
					ascii[i] = 91;
				}
				chars[i] = (char)(65 + (ascii[i] - 65 + (encode ? 0 : n) + ((encode ? 1 : -1) * code[j])) % n);
				if (chars[i] == '[') {
					chars[i] = ' ';
				}
				j = (j + 1) % code.Length;
			}
			//chars[i] = (encode ? 'E' : 'D');
		}
		return new string(chars);
	}

	public string Encode(string str) {
		return EncodeDecode(str, true);
	}

	public string Decode(string str) {
		return EncodeDecode(str, false);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
