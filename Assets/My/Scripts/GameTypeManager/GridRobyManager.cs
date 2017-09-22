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

	public ImageTargetBehaviour imageTargetDevBoard;
	public GameObject devBoardTargetCanvas;

	public BaseGridRobyGameType GetGameType() {
		return (BaseGridRobyGameType)gameType;
	}


	public GameObject gameObjDevBoard;
	public GameObject gameUIDevBoard;

	public override GameObject GameObj { get {
		return isDevBoardMode ? gameObjDevBoard : gameObj;
	} }
	public override GameObject GameUI { get {
		return isDevBoardMode ? gameUIDevBoard : gameUI;
	} }
	public override GameObject TargetCanvas { get {
		return isDevBoardMode ? devBoardTargetCanvas : targetCanvas;
	} }

	string text;

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
			text = "";
			lettersText.text = "";
		}

		if (codingGrid == null) {
			codingGrid = GameObject.FindObjectOfType<CodingGrid>();
		}

		if (GetGameType().useDevBoard && useAR) {
			SetDevBoardActive(true);
		}		
	}

	public string GetLettersText() {
		return text;
	}

	public void AppendLetter(char letter, bool skipText = false) {
		if (!skipText)
			text += letter.ToString();
		if (!panelLetters.gameObject.activeSelf) {
			panelLetters.gameObject.SetActive(true);
		}
		lettersText.text = text;
	}

	public IEnumerator AppendLetterDelayed(char letter, float delay) {
		text += letter;
		yield return new WaitForSeconds(delay);
		AppendLetter(letter, true);
	}

	public void RemoveLastLetter() {
		lettersText.text = lettersText.text.Substring(0, lettersText.text.Length - 1);
	}

	public void EnableAllGameObjects(bool enable) {
		TargetCanvas.SetActive(enable);
		GameObj.SetActive(enable);
		GameUI.SetActive(enable);
	}

	public void SetDevBoardActive(bool activate) {

		//gameManager.imageTracker.StopTrack();

		//devBoardTargetCanvas.SetActive(activate);
		gameManager.SetMainImgTargetsActive(!activate, true);

		if (activate) {
			/*
			if (imageTargetDevBoard == null) {
				imageTargetDevBoard = Instantiate(imageTargetDevBoardPrefab, Vector3.back, Quaternion.identity).GetComponent<ImageTargetBehaviour>();
			}
			*/
			EnableAllGameObjects(false);
			isDevBoardMode = true;
			EnableAllGameObjects(true);
			gameCanBeShown = false;
			//imageTargetDevBoard.gameObject.SetActive(true);

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
			imageTargetDevBoard.gameObject.SetActive(false);
			imageTargetDevBoard.GetComponentInChildren<ARFormOptions.DevBoardFormElement>().OnSendToCodingGrid += (string code) => {
				MyDebug.Log(code);
				codingGrid.SetCards(code);
				GridRobyManager.Instance.SetDevBoardActive(false);
			};
		}
		else { // Deactivate
			if (imageTargetDevBoard) {
				EnableAllGameObjects(false);
				isDevBoardMode = false;
				EnableAllGameObjects(true);
				gameCanBeShown = true;
				gameManager.imageTracker.UnloadImageTargetBehaviour(imageTargetDevBoard);
				devBoardTargetCanvas.SetActive(false);
			//Destroy(imageTargetDevBoard);
			}
			//imageTargetDevBoard.gameObject.SetActive(false);
		}
		UpdateVisibility(true);

		//gameManager.imageTracker.StartTrack();
	}

	public override void TurnMusicOn(bool isOn) {
		base.TurnMusicOn(isOn);
		//@todo
	}

	public override void TurnSoundOn(bool isOn) {
		base.TurnSoundOn(isOn);
		//@todo
	}

	void OnDestroy() {
		if (gameManager != null)
			gameManager.SetMainImgTargetsActive(true);
		instance = null;
	}
	
}
