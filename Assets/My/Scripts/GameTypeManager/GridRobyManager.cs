using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridRobyManager : BaseGameTypeManager {

	//[System.NonSerialized]
	//public new BaseGridRobyGameType gameType;

	public Grid grid;

	public GameObject panelLetters;

	Text lettersText;

	public override void InitConfig() {

		((BaseGridRobyGameType)gameType).grid = grid;
		grid.gameTypeManager = this;
		grid.gameTypeConfig = (BaseGridRobyGameType)gameType;

		panelLetters.SetActive(grid.gameTypeConfig.withLetters);
		lettersText = panelLetters.GetComponentInChildren<Text>();
		lettersText.text = "";
	}

	public void AppendLetter(char letter) {
		lettersText.text += letter.ToString();
	}
	
	void Awake() {
	}

}
