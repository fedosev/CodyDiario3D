using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CipherRotGameType : BaseGameType {

	public override string title { get {
		return "Cifrario";
	} }
    public override string generalInfo { get {
        return "Il primo rotore rappresenta le lettere dell’alfabeto su cui è definito il testo in chiaro. Puoi ruotarlo per cercare la lettera che vuoi cifrare."
            + (!isRotFixed ?
                "\nI rotori a destra del primo rappresentano le corrispondenti lettere cifrate. Possono essere ruotati per cambiare il cifrario." : "")
            + (!isRotFixed && rotCode.Length == 1 ?
                "\nIn questo cifrario c'è un solo rotore a destra." : "")
            + (!isDecodedTextFixed  ?
                "\nPuoi scrivere e modificare il testo da cifrare facendo il tap sulla casella \"Testo in chiaro\"." : "")
            + (!isEncodedTextFixed  ?
                "\nPuoi scrivere e modificare il testo da decifrare facendo il tap sulla casella \"Testo cifrato\"." : "")
        ;
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
		rotObj.Init();

		var inputFields = FindObjectsOfType<InputEncodeDecode>();
		foreach (var inputField in inputFields) {
			inputField.Init();
		}
	}

    public override void Pause(bool pause) {
        if (rotObj != null) {
            rotObj.Pause(pause);
        }
    }

}
