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

	bool isPaused = false;

	public void Pause(bool pause) {
		isPaused = pause;
		for (var i = 0; i < code.Length; i++) {
			rotCylinders[i].isPaused = pause;
		}
		fixedRotCylinder.isPaused = pause;
	}
	

	public void Init() {

		//rotCylinders = FindObjectsOfType<RotCylinder>();
		if (code.Length < rotCylinders.Length) {
			for (var i = code.Length; i < rotCylinders.Length; i++) {
				rotCylinders[i].gameObject.SetActive(false);
			}
			if (code.Length == 1 && MainGameManager.Instance != null && MainGameManager.Instance.useAR) {
				transform.position = new Vector3(0f, transform.position.y, transform.position.z);
			}
		}
		for (var i = 0; i < code.Length; i++) {
			if (withSpace) {
				rotCylinders[i].withSpace = true;
				rotCylinders[i].GenerateChars();
			}
			if (isFixed) {
				rotCylinders[i].isFixed = true;
			}
		}
		if (withSpace) {
			fixedRotCylinder.withSpace = true;
			fixedRotCylinder.GenerateChars();
		}
		SetCode(code);
		for (var i = 0; i < code.Length; i++) {
			rotCylinders[i].onRotNumberChange.AddListener(UpdateCodeFromCylinders);
		}
	}

	void Awake() {

		if (code.Length > rotCylinders.Length) {
			Debug.LogError("code.Length > rotCylinders.Length");
		}

		if (onCodeChange == null) {
			onCodeChange = new UnityEvent();
		}
	}

	// Use this for initialization
	void Start () {

		//Init();
	}

	void OnEnable() {

	}
	
	public void UpdateCodeFromCylinders() {
		for (var i = 0; i < code.Length; i++) {
			code[i] = rotCylinders[i].RotNumber;
		}
		if (onCodeChange != null) {
			onCodeChange.Invoke();
		}
	}

	public void SetCode(int[] code) {

		this.code = code;
		for (var i = 0; i < code.Length; i++) {
			rotCylinders[i].SetRotNumber(code[i]);
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
