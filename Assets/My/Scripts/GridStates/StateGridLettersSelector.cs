using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System.Text.RegularExpressions;

public class StateGridLettersSelector : GameObjectState, IPointerClickHandler {

	Grid grid;
	QuadBehaviour quad;
	int n;
	int nRows;
	int nCols;

	public override void OnEnter() {

		grid.inPause = true;
		grid.SetActiveUI(false);
		grid.keyboard.onKeyPressed.AddListener(SetLetter);
		grid.keyboard.onBackspacePressed.AddListener(UnsetLetter);
		grid.keyboard.okButton.GetComponent<Button>().onClick.AddListener(OkButtonHandler);
		grid.keyboard.copyButton.GetComponent<Button>().onClick.AddListener(Copy);
		grid.keyboard.pasteButton.GetComponent<Button>().onClick.AddListener(Paste);
		grid.gameTypeManager.panelLetters.SetActive(false);
		grid.gameTypeManager.codingGrid.HideTemporarily();
		nRows = grid.nRows;
		nCols = grid.nCols;
		n = nRows * nCols;
		var letters = PlayerPrefs.GetString(GetSaveString());
		if (letters.Length == n) {
			for (int i = 0; i < n; i++) {
				grid.quadBhs[i].SetLetter(letters[i]);
			}
			grid.keyboard.okButton.SetActive(true);
			//grid.keyboard.isHiddenOnStart = false;
		}
		grid.gameTypeManager.OnGameTypeStart += ShowKeyboard;
	}

	public override void OnExit() {

		grid.inPause = false;
		grid.keyboard.onKeyPressed.RemoveListener(SetLetter);
		grid.keyboard.onBackspacePressed.RemoveListener(UnsetLetter);
		grid.keyboard.okButton.GetComponent<Button>().onClick.RemoveListener(OkButtonHandler);
		grid.keyboard.copyButton.GetComponent<Button>().onClick.RemoveListener(Copy);
		grid.keyboard.pasteButton.GetComponent<Button>().onClick.RemoveListener(Paste);
		grid.keyboard.Hide();
		grid.gameTypeManager.panelLetters.SetActive(true);
		grid.gameTypeManager.codingGrid.ShowIfWasVisible();
		if (grid.QuadCount(quad => quad.isSetLetter) == n) {
			var sb = new StringBuilder();
			foreach (var qb in grid.quadBhs) {
				sb.Append(qb.letter);
			}
			PlayerPrefs.SetString(GetSaveString(), sb.ToString());
		}
		if (quad != null)
			quad.Undo();
	}
	
	public override GameObjectState NextState() {

		if (grid.CurrentRobotController == null) {
			return GoToState<StateGridPlayerPosition>();
		}

		grid.SetActiveUI(true);
		return GoToState<StateNull>();
	}	

	public void OnPointerClick(PointerEventData eventData) {
		
		if (eventData.pointerEnter.tag == "Quad") {
			if (quad != null) {
				quad.Undo();
				//quad.SetState(QuadStates.DEFAULT);
			}
			quad = eventData.pointerEnter.GetComponent<QuadBehaviour>();
			quad.RecordUndo();
			quad.SetState(QuadStates.INFO);
			grid.keyboard.Show();
		}
	}

	string GetSaveString() {
		return grid.gameTypeConfig.month.ToString() + "-" + grid.gameTypeConfig.name + "/letters";
	}

	QuadBehaviour GetQuad(int index, int offset) {

		var row = index / nCols;
		index = (index + offset) % n;
		if (index < 0) {
			index = n + index;
		}
		var newRow = index / nCols;
		row = (row - (newRow - row)) % nRows;
		if (row < 0) {
			row = nRows + row;
		}
		return grid.GetQuadBh(row, index % nCols);
	}

	void Select(int offset) {

		int index;
		if (quad != null) {
			quad.Undo();
			index = quad.index;
		} else {
			index = grid.GetOtherIndex(0);
		}
		quad = GetQuad(index, offset);
		quad.RecordUndo();
		quad.SetState(QuadStates.INFO);
	}

	void ShowKeyboard() {
		Select(0);
		grid.keyboard.Show();
	}

	void SetLetter(char letter) {
		quad.SetLetter(letter);
		Select(1);
		if (grid.QuadCount(quad => quad.isSetLetter) == n)
			grid.keyboard.okButton.SetActive(true);
	}

	void UnsetLetter() {
		Select(-1);
		if (quad.isSetLetter)
			quad.UnsetLetter();
		grid.keyboard.okButton.SetActive(false);
	}

	void OkButtonHandler() {
		NextState();
	}

	void Copy() {
		var sb = new StringBuilder();
		var qb = grid.GetQuadBh(grid.GetOtherIndex(0));
		for (int i = 0; i < n; i++) {
			sb.Append(qb.letter);
			qb = GetQuad(qb.index, 1);
		}
		UniClipboard.SetText(sb.ToString());
	}

	void Paste() {
		var text = UniClipboard.GetText();
		text = Regex.Replace(text, @"([^A-Za-z ])", "");
		text = text.ToUpper();
		var qb = grid.GetQuadBh(grid.GetOtherIndex(0));
		for (int i = 0; i < n && i < text.Length; i++) {
			qb.SetLetter(text[i]);
			qb = GetQuad(qb.index, 1);
		}
		if (grid.QuadCount(quad => quad.isSetLetter) == n) {
			grid.keyboard.okButton.SetActive(true);
		}
	}


	void Awake() {
		grid = GetComponent<Grid>();
	}

}
