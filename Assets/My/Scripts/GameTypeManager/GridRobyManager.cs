using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyAR;

public class GridRobyManager : BaseGameTypeManager {

	private static GridRobyManager instance;
	public static GridRobyManager Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<GridRobyManager>();
		return instance;
	} }

	//[System.NonSerialized]
	//public new BaseGridRobyGameType gameType;

	public Grid grid;

	public GameObject panelLetters;

    public CodingGrid codingGrid;

	public GameObject[] objectsToHideOnDevBoard;

	public EasyImageTargetBehaviour imageTargetDevBoard;

	Text lettersText;

	public override void InitConfig() {

		((BaseGridRobyGameType)gameType).grid = grid;
		grid.gameTypeManager = this;
		grid.gameTypeConfig = (BaseGridRobyGameType)gameType;

		if (panelLetters != null) {
			panelLetters.SetActive(grid.gameTypeConfig.withLetters);
			lettersText = panelLetters.GetComponentInChildren<Text>();
			lettersText.text = "";
		}

		if (codingGrid == null) {
			codingGrid = GameObject.FindObjectOfType<CodingGrid>();
		}

		if (((BaseGridRobyGameType)gameType).useDevBoard && useAR && imageTargetDevBoard != null) {
			SetDevBoardActive(true);
		}		
	}

	public void AppendLetter(char letter) {
		if (!panelLetters.gameObject.activeSelf) {
			panelLetters.gameObject.SetActive(true);
		}
		lettersText.text += letter.ToString();
	}

	public void SetDevBoardActive(bool activate) {

		gameManager.imageTracker.StopTrack();
		if (activate) {
			//imageTargetDevBoard.gameObject.SetActive(true);
			imageTargetDevBoard.ActiveTargetOnStart = true;
			imageTargetDevBoard.Bind(gameManager.imageTracker);
			gameManager.imageTracker.LoadImageTargetBehaviour(imageTargetDevBoard);
		} else {
			gameManager.imageTracker.UnloadImageTargetBehaviour(imageTargetDevBoard);
			//imageTargetDevBoard.gameObject.SetActive(false);
		}
		foreach (var obj in objectsToHideOnDevBoard) {
			obj.SetActive(!activate);
		}
		gameManager.imageTracker.StartTrack();
	}
	
}
