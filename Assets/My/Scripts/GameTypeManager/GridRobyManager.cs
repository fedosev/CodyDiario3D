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

	public GameObject[] toHideOnDevBoard;
	public GameObject[] toHideOnMenu;

	public ImageTargetBehaviour imageTargetDevBoard;
	public GameObject devBoardTargetCanvas;

	public BaseGridRobyGameType GetGameType() {
		return (BaseGridRobyGameType)gameType;
	}

	Text lettersText;

	bool isDevBoardMode = false;

	public override void InitConfig() {

		//GetGameType().grid = FindObjectOfType<Grid>();
		GetGameType().grid = grid;
		grid.gameTypeManager = this;
		grid.gameTypeConfig = GetGameType();

		if (panelLetters != null) {
			panelLetters.SetActive(grid.gameTypeConfig.withLetters);
			lettersText = panelLetters.GetComponentInChildren<Text>();
			lettersText.text = "";
		}

		if (codingGrid == null) {
			codingGrid = GameObject.FindObjectOfType<CodingGrid>();
		}

		if (GetGameType().useDevBoard && useAR) {
			SetDevBoardActive(true);
		}		
	}

	public void AppendLetter(char letter) {
		if (!panelLetters.gameObject.activeSelf) {
			panelLetters.gameObject.SetActive(true);
		}
		lettersText.text += letter.ToString();
	}

	public void RemoveLastLetter() {
		lettersText.text = lettersText.text.Substring(0, lettersText.text.Length - 1);
	}

	public void SetDevBoardActive(bool activate) {

		//gameManager.imageTracker.StopTrack();

		devBoardTargetCanvas.SetActive(activate);
		gameManager.SetMainImgTargetsActive(!activate, true);

		if (activate) {
			/*
			if (imageTargetDevBoard == null) {
				imageTargetDevBoard = Instantiate(imageTargetDevBoardPrefab, Vector3.back, Quaternion.identity).GetComponent<ImageTargetBehaviour>();
			}
			*/
			isDevBoardMode = true;
			gameCanBeShown = false;
			imageTargetDevBoard.gameObject.SetActive(true);

			if (!imageTargetDevBoard.ActiveTargetOnStart) {
				imageTargetDevBoard.SetupWithImage(imageTargetDevBoard.Path, imageTargetDevBoard.Storage, imageTargetDevBoard.Name, imageTargetDevBoard.Size);
				imageTargetDevBoard.TargetFound += (TargetAbstractBehaviour behaviour) => {
					devBoardTargetCanvas.SetActive(false);
				};
				imageTargetDevBoard.TargetLost += (TargetAbstractBehaviour behaviour) => {
					if (isDevBoardMode)
						devBoardTargetCanvas.SetActive(true);
				};
				imageTargetDevBoard.ActiveTargetOnStart = true;
			}
			imageTargetDevBoard.Bind(gameManager.imageTracker);
			//gameManager.imageTracker.LoadImageTargetBehaviour(imageTargetDevBoard);
			ShowGame(false);
			imageTargetDevBoard.gameObject.SetActive(false);
		}
		else { // Deactivate
			if (imageTargetDevBoard) {
				isDevBoardMode = false;
				gameCanBeShown = true;
				gameManager.imageTracker.UnloadImageTargetBehaviour(imageTargetDevBoard);
				devBoardTargetCanvas.SetActive(false);
				//Destroy(imageTargetDevBoard);
			}
			//imageTargetDevBoard.gameObject.SetActive(false);
		}

		foreach (var obj in toHideOnDevBoard) {
			obj.SetActive(!activate);
		}

		//gameManager.imageTracker.StartTrack();
	}

	void OnDestroy() {
		if (gameManager != null)
			gameManager.SetMainImgTargetsActive(true);
		instance = null;
	}
	
}
