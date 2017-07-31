using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CipherRotGameType : BaseGameType {

	public enum EncodingMode { StartEncoding, StartDecoding }

	public EncodingMode encodingMode = EncodingMode.StartEncoding;

	public int[] rotCode = { 0 };
	public bool withSpace = false;
	public string statringText = "";
	//public bool isStatringTextEncoded;
	public bool isDecodedTextFixed = false;
	public bool isEncodedTextFixed = false;
	public bool isRotFixed = false;

	RotCode rotObj;
	
    public override string sceneName { get {
        return "CipherRot";
    } }

	public override void InitBody() {

		rotObj = FindObjectOfType<RotCode>();
		rotObj.code = (int[])rotCode.Clone();
		rotObj.withSpace = withSpace;
		rotObj.isFixed = isRotFixed;

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
		//rotObj.SetCode(rotObj.code);
	}

}
