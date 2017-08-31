using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyAR;
using System.Globalization;

public class GridHexaManager : GridRobyManager {

	private static GridHexaManager instance;
	public static new GridHexaManager Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<GridHexaManager>();
		return instance;
	} }

	public HexaText hexaText;

	public Keyboard keyboard;

	public const int hexaCodeLenght = 16;

	public int[] hexaCode = new int[hexaCodeLenght];

	int editIndex = 0;


	public override GameObject GameObj { get {
		return gameObj;
	} }
	public override GameObject GameUI { get {
		return gameUI;
	} }
	public override GameObject TargetCanvas { get {
		return  targetCanvas;
	} }

	public new GridHexaGameType GetGameType() {
		return (GridHexaGameType)gameType;
	}

	public void UpdateHexaCode(int index, int val, bool add) {
		hexaCode[index] += (add ? val : -val);
		hexaText.SetText(index, hexaCode[index]);
	}

	public void UpdateHexaCode(int index, int val) {
		hexaCode[index] = val;
		hexaText.SetText(index, hexaCode[index]);
	}

	public void HandleKeyPress(char key) {
		int val = System.Int16.Parse(key.ToString(), NumberStyles.AllowHexSpecifier);
		UpdateHexaCode(editIndex, val);
		GetGameType().UpdateQuads(editIndex, val);
		NextEditIndex();
	}
	public void NextEditIndex() {
		editIndex = (editIndex + 1) % 16;
		hexaText.Highlight(editIndex);
	}

	public void SelectField(int index) {
		editIndex = index * 2;
		hexaText.Highlight(editIndex);
	}
	
	IEnumerator ShowKeyboardDelayed(float delay) {
		yield return new WaitForSeconds(delay);
		hexaText.Highlight(editIndex);
		keyboard.Show();
	}

	public override void InitConfig() {

		//GetGameType().grid = FindObjectOfType<Grid>();
		GetGameType().grid = grid;
		grid.gameTypeManager = this;
		grid.gameTypeConfig = GetGameType();

		for (var i = 0; i < hexaCodeLenght; i++) {
			hexaCode[i] = 0;
		}
		if (GetGameType().isGridToCode) {
			//keyboard.Hide(true);
		} else {
			StartCoroutine(ShowKeyboardDelayed(2f));
			keyboard.onKeyPressed.AddListener(HandleKeyPress);
			hexaText.OnFieldClick += SelectField;
		}
		hexaText.Init(!GetGameType().isGridToCode);
	}
}
