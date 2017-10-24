using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CipherRotGameType : BaseGameType {

	public override string title { get {
		return "Cifrario";
	} }
	
    public override string generalInfo { get {
        return 
			isOrderCipher ? (
				"Oggi puoi cambiare l'ordine del alfabeto per la cifratura. Usa la tastiera per inserire le lettere nell'ordine che preferisci compreso lo spazio.\n" +
				"Quando le hai inserite tutte premi sulla spunta per confermare. \nDopodichè usa il cifrario con nei giorni precedenti."
			) : (
				"Il primo rotore rappresenta le lettere dell’alfabeto su cui è definito il testo in chiaro. Puoi ruotarlo per cercare la lettera che vuoi cifrare."
				+ (!isRotFixed ?
					"\nI rotori a destra del primo rappresentano le corrispondenti lettere cifrate. Possono essere ruotati per cambiare il cifrario." : "")
				+ (!isRotFixed && rotCode.Length == 1 ?
					"\nIn questo cifrario c'è un solo rotore a destra." : "")
				+ (!isDecodedTextFixed ?
					"\nPuoi scrivere e modificare il testo da cifrare facendo il tap sulla casella \"Testo in chiaro\"." : "")
				+ (!isEncodedTextFixed ?
					"\nPuoi scrivere e modificare il testo da decifrare facendo il tap sulla casella \"Testo cifrato\"." : "")
			);
    } }

	public enum EncodingMode { StartEncoding, StartDecoding }

	public EncodingMode encodingMode = EncodingMode.StartEncoding;

	public int[] rotCode = { 0 };
	public bool withSpace = false;
	public string statringText = "";
	public bool isDecodedTextFixed = false;
	public bool isEncodedTextFixed = false;
	public bool isRotFixed = false;
	public bool isCodeSizeVariable = false;

	public bool isOrderCipher = false;
	public string sequence = "";

	RotCode rotObj;
	
    public override string sceneName { get {
        return "CipherRot";
    } }

	/*
    public override string sceneNameNoAR { get {
        return "CipherRotNoAR";
    } }
	// */

	public override void InitBody() {

		if (isOrderCipher || sequence.Length < 0) {
			rotCode = new int[] { 0 };
			isRotFixed = true;
			isCodeSizeVariable = false;
		}

		rotObj = FindObjectOfType<RotCode>();
		rotObj.code = (int[])rotCode.Clone();
		rotObj.withSpace = withSpace;
		rotObj.isFixed = isRotFixed;

		rotObj.SetCodeSizeVariable(!isRotFixed && isCodeSizeVariable);

		var inputFieldDecoded = GameObject.Find("InputFieldDecoded").GetComponent<InputEncodeDecode>();
		var inputFieldEncoded = GameObject.Find("InputFieldEncoded").GetComponent<InputEncodeDecode>();


		if (encodingMode == EncodingMode.StartEncoding) {
			if (statringText != "") {
				inputFieldDecoded.SetText(statringText);
				inputFieldDecoded.InitOtherText();
				inputFieldDecoded.SetLastFocused();
			}

		} else if (encodingMode == EncodingMode.StartDecoding) {
			if (statringText != "") {
				inputFieldEncoded.SetText(statringText);
				inputFieldEncoded.InitOtherText();
				inputFieldEncoded.SetLastFocused();
			}
		}

		inputFieldDecoded.SetFixed(isDecodedTextFixed);
		inputFieldEncoded.SetFixed(isEncodedTextFixed);

		GameObject.FindObjectOfType<RotNumberText>().UpdateText();

		if (sequence.Length > 0) {
			rotObj.sequence = sequence;
		}

		rotObj.Init();

		if (isOrderCipher && sequence != null && sequence.Length == 0) {
			rotObj.state.GoToState<StateCipherOrder>();
			rotObj.InitInputEncodeDecode(true);
		} else {
			rotObj.InitInputEncodeDecode();
		}

	}

    public override void Pause(bool pause) {
        if (rotObj != null) {
            rotObj.Pause(pause);
        }
    }

}
