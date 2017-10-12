using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class RotCode : MonoBehaviour {

	public RotCylinder fixedRotCylinder;
	public RotCylinder[] rotCylinders;

	public GameObject codeSizeUI;

	public bool withSpace = false;

	public bool isFixed = false;

	public int[] code;

	public UnityEvent onCodeChange;

	public GameObjectState state;

	bool isPaused = false;

	public string sequence = "";


	public void InitInputEncodeDecode(bool isEditSequenceMode = false) {
		var inputFields = FindObjectsOfType<InputEncodeDecode>();
		foreach (var inputField in inputFields) {
			inputField.SetEditSequenceMode(isEditSequenceMode);
			inputField.Init();
		}
	}

    public void Pause(bool pause) {
		isPaused = pause;
		for (var i = 0; i < code.Length; i++) {
			rotCylinders[i].isPaused = pause;
		}
		fixedRotCylinder.isPaused = pause;
	}
	
	public void SetCodeSizeVariable(bool isVariable) {
		codeSizeUI.SetActive(isVariable);
	}

	public void Init() {

		//rotCylinders = FindObjectsOfType<RotCylinder>();
		if (code.Length < rotCylinders.Length) {
			for (var i = code.Length; i < rotCylinders.Length; i++) {
				rotCylinders[i].gameObject.SetActive(false);
			}
			if (code.Length == 1/* && MainGameManager.Instance != null && MainGameManager.Instance.useAR*/) {
				transform.position = new Vector3(0f, transform.position.y, transform.position.z);
			}
		}
		for (var i = 0; i < code.Length; i++) {
			rotCylinders[i].Init(this);
			if (withSpace || sequence.Length > 0) {
				rotCylinders[i].withSpace = withSpace;
				rotCylinders[i].GenerateChars();
			}
			if (isFixed) {
				rotCylinders[i].isFixed = true;
			}
		}
		if (withSpace) {
			fixedRotCylinder.withSpace = true;
			fixedRotCylinder.Init(this);
			fixedRotCylinder.GenerateChars();
		}
		SetCode(code);
		for (var i = 0; i < code.Length; i++) {
			rotCylinders[i].onRotNumberChange.AddListener(UpdateCodeFromCylinders);
		}
	
		if (false && MainGameManager.Instance != null && !MainGameManager.Instance.useAR) {
			var maskObj = GameObject.Find("QuadMask");
			if (maskObj != null)
				maskObj.SetActive(false);
		}

		state = GameObjectState.Init(this.gameObject, newState => { state = newState; });
	}

	public void IncreaseCodeSize() {
		
		if (code.Length < rotCylinders.Length) {
			var l = code.Length;
			System.Array.Resize<int>(ref code, l + 1);
			code[l] = 0;
			rotCylinders[l].gameObject.SetActive(true);
			rotCylinders[l].SetRotNumber(code[l], false, true);
			if (onCodeChange != null) {
				onCodeChange.Invoke();
			}
		}
	}

	public void DecreaseCodeSize() {

		if (code.Length > 1) {
			var l = code.Length - 1;
			System.Array.Resize<int>(ref code, l);
			rotCylinders[l].gameObject.SetActive(false);
			if (onCodeChange != null) {
				onCodeChange.Invoke();
			}
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
	
	public void UpdateCodeFromCylinders() {
		for (var i = 0; i < code.Length; i++) {
			code[i] = rotCylinders[i].RotNumber;
		}
		if (onCodeChange != null) {
			onCodeChange.Invoke();
		}
	}

	public void TriggerCodeChange() {
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
				var pos = (ascii[i] - 65 + (encode ? 0 : n) + ((encode ? 1 : -1) * code[j])) % n;
				if (sequence.Length > 0) {
					if (pos < 0)
						chars[i] = '?';
					else
						chars[i] = sequence[pos % sequence.Length];
				} else {
					chars[i] = (char)(65 + pos);
				}
				if (chars[i] == '[') {
					chars[i] = ' ';
				}
				j = (j + 1) % code.Length;
			}
		}
		return new string(chars);
	}

	public string Encode(string str) {
		return EncodeDecode(str, true);
	}

	public string Decode(string str) {
		return EncodeDecode(str, false);
	}

}
