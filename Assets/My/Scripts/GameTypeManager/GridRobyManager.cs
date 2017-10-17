using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyAR;
using System.Diagnostics;

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
    public HandCards handCards;
    public CardsSelection cardsSelection;

	public ImageTargetBehaviour imageTargetDevBoard;
	public GameObject devBoardTargetCanvas;

	public BaseGridRobyGameType GetGameType() {
		return (BaseGridRobyGameType)gameType;
	}


	public GameObject gameObjDevBoard;
	public GameObject gameUIDevBoard;

	public Button execButton;

	public override GameObject GameObj { get {
		return isDevBoardMode ? gameObjDevBoard : gameObj;
	} }
	public override GameObject GameUI { get {
		return isDevBoardMode ? gameUIDevBoard : gameUI;
	} }
	public override GameObject TargetCanvas { get {
		return isDevBoardMode ? devBoardTargetCanvas : targetCanvas;
	} }

	string[] text = new string[2];

	Text lettersText;

	bool isDevBoardMode = false;

    [HideInInspector] public Deck deck;


    public override void InitConfig() {

		//GetGameType().grid = FindObjectOfType<Grid>();
		GetGameType().grid = grid;
		grid.gameTypeManager = this;
		grid.gameTypeConfig = GetGameType();

		if (execButton != null) {
			execButton.gameObject.SetActive(false);
		}

		if (panelLetters != null) {
			panelLetters.SetActive(grid.gameTypeConfig.withLetters);
			lettersText = panelLetters.GetComponentInChildren<Text>();
			text[0] = "";
			text[1] = null;
			lettersText.text = "";
		}

		if (codingGrid == null) {
			codingGrid = GameObject.FindObjectOfType<CodingGrid>();
		}
		if (codingGrid != null)
			codingGrid.Init();

		if (handCards == null) {
			handCards = GameObject.FindObjectOfType<HandCards>();
		}
		if (cardsSelection == null) {
			cardsSelection = GameObject.FindObjectOfType<CardsSelection>();
		}

		if (GetGameType().useDevBoard && useAR) {
			SetDevBoardActive(true);
		}		
	}

	public void InitLettersTextLine(int line) {
		text[line] = "";
	}

	public string GetLettersText(int line = 0) {
		return text[line];
	}

	public void AppendLetter(char letter, bool skipText = false, int line = 0) {
		if (!skipText) {
			AddLetter(letter, line);
		}
		if (!panelLetters.gameObject.activeSelf) {
			panelLetters.gameObject.SetActive(true);
		}
		lettersText.text = text[0];
		if (text[1] != null) {
			lettersText.text += "\n" + text[1];
		}
	}

	public IEnumerator AppendLetterDelayed(char letter, float delay, int line = 0) {
		AddLetter(letter, line);
		yield return new WaitForSeconds(delay);
		AppendLetter(letter, true, line);
	}

	void AddLetter(char letter, int line) {
		if (text[line] == null) {
			text[line] = "";
		} else if (text[line].Length > 40) {
			text[line] = text[line].Substring(1);
		}
		text[line] += letter;
	}

	public void RemoveLastLetter() {
		lettersText.text = lettersText.text.Substring(0, lettersText.text.Length - 1);
	}

	public void EnableAllGameObjects(bool enable) {
		TargetCanvas.SetActive(enable);
		GameObj.SetActive(enable);
		GameUI.SetActive(enable);
	}

	[Conditional("F_AR_ENABLED")]
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
