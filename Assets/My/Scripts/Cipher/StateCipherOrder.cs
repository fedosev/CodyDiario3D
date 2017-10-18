using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateCipherOrder : GameObjectState {

	RotCode rotCode;
	InputEncodeDecode[] inputsEncodeDecode;
	string sequence;
	InputEncodeDecode inputSequence;
	Button okButton;

	Vector3 originalPosition;

	public override void OnEnter() {

		foreach (var rotCyl in rotCode.rotCylinders) {
			rotCyl.gameObject.SetActive(false);
		}
		BaseGameTypeManager.Instance.shouldBeVisibleTargetCanvas = false;
		BaseGameTypeManager.Instance.UpdateVisibility();
		//rotCode.fixedRotCylinder.gameObject.SetActive(false);
		originalPosition = rotCode.transform.position;
		rotCode.transform.position = new Vector3(0f, 0f, -1000f);
		inputSequence = inputsEncodeDecode[0].GetField(true);
		okButton = inputSequence.keyboard.okButton.GetComponent<Button>();
		inputSequence.OnAddLetter += AddLetterHandler;
		inputSequence.OnRemoveLetter += RemoveLetterHandler;
		okButton.onClick.AddListener(OkButtonHandler);
		
		var rotText = GameObject.FindObjectOfType<RotNumberText>();
		var sequence = PlayerPrefs.GetString("CipherSequence");
		if (sequence.Length == 26 || sequence.Length == 27) {
			rotCode.sequence = sequence;
			inputsEncodeDecode[0].SetEditSequenceMode(true);
			inputsEncodeDecode[1].SetEditSequenceMode(true);
			inputSequence.SetText(sequence);
			inputSequence.InitOtherText();
			inputSequence.keyboard.SetInteractable(false);
			Key key;
			if (sequence.Length == 26) {
				if (inputSequence.keyboard.TryGetKey(' ', out key)) {
					key.button.interactable = true;
				}
			}
			if (inputSequence.keyboard.TryGetKey('<', out key)) {
				key.button.interactable = true;
			}
			okButton.gameObject.SetActive(true);
			rotText.SetText("MODIFICA...");
		} else {
			inputsEncodeDecode[0].GetField(false).SetPlaceholder("Lettere in ordine alfabetico");
			inputSequence.SetPlaceholder("Lettere nel tuo ordine");
			rotText.SetText("SELEZIONA...");
		}
		CipherRotManager.Instance.OnGameTypeStart += ShowKeyboard;
		//StartCoroutine(ShowKeyboard(0f));
	}

	void ShowKeyboard(/*float delay = 1f*/) {
		inputSequence.keyboard.couldBeHidden = false;
		//yield return new WaitForSeconds(delay);
		inputSequence.keyboard.Show();
	}

	public override void OnExit() {

		foreach (var rotCyl in rotCode.rotCylinders) {
			rotCyl.gameObject.SetActive(true);
		}
		//rotCode.fixedRotCylinder.gameObject.SetActive(true);
		rotCode.transform.position = originalPosition;
		rotCode.sequence = inputSequence.GetText();
		rotCode.Init();
		foreach (var inputED in inputsEncodeDecode) {
			inputED.SetText("");
			inputED.SetEditSequenceMode(false);
			inputED.ResetPlaceholder();
		}
		okButton.onClick.RemoveListener(OkButtonHandler);
		okButton.gameObject.SetActive(false);
		inputSequence.OnAddLetter -= AddLetterHandler;
		inputSequence.OnRemoveLetter -= RemoveLetterHandler;
		inputSequence.keyboard.SetInteractable(true);
		inputSequence.keyboard.couldBeHidden = true;
		rotCode.TriggerCodeChange();

		PlayerPrefs.SetString("CipherSequence", rotCode.sequence);
		BaseGameTypeManager.Instance.shouldBeVisibleTargetCanvas = true;
		BaseGameTypeManager.Instance.UpdateVisibility(true);
		//StartCoroutine(UpdateVisibility());
	}

	IEnumerator UpdateVisibility() {
		yield return null;
		BaseGameTypeManager.Instance.UpdateVisibility(true);
	}

	void OkButtonHandler() {
		NextState();
	}

	void AddLetterHandler(char letter) {
		Key key;
		if (inputSequence.keyboard.TryGetKey(letter, out key)) {
			key.button.interactable = false;
		}
	}

	void RemoveLetterHandler(char letter) {
		Key key;
		if (inputSequence.keyboard.TryGetKey(letter, out key)) {
			key.button.interactable = true;
		}
	}

	void Awake() {
		rotCode = GetComponent<RotCode>();
		inputsEncodeDecode = FindObjectsOfType<InputEncodeDecode>();
	}
}
