using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameTypeManager : MonoBehaviour {

	//[System.NonSerialized]
	public BaseGameType gameType;

	public GameObject gameObj;
	public GameObject gameUI;
	public GameObject targetCanvas;

	public virtual GameObject TargetCanvas { get {
		return targetCanvas;
	} }

	public virtual GameObject GameObj { get {
		return gameObj;
	} }
	public virtual GameObject GameUI { get {
		return gameUI;
	} }

    public GameObject NoARObj;

	public bool useAR = true;

	public bool isGameInit = false;

	public Action WinAction;
	public Action<string> WinTextAction;
	public Action LoseAction;
	public Action<string> LoseTextAction;

	protected MainGameManager gameManager;

	protected bool gameCanBeShown = true;
	public bool wasShowBeforeMenu = false;

	bool isGameVisible = true;
	bool isGameUIVisible = true;

    Canvas uICanvas;
    float canvasScaleFactor;


    public void UpdateVisibility(bool force = false) {

		UpdateGameVisibility();
		UpdateUIVisibility(force);
		UpdateTargetCanvasVisibility();
	}

	void UpdateGameVisibility() {

		bool showGame = true;
		
		if (gameManager && useAR && (gameManager.isARPaused || !gameManager.IsARTracked)) {
			showGame = false;
		}
		if (gameManager && gameManager.mainMenu.isVisible && wasShowBeforeMenu) {
			showGame = true;
		}
		/*
		if (gameManager && gameManager.isLoading) {
			showGame = false;
		}
		*/

		if (showGame != isGameVisible) {
			SetVisible(GameObj, showGame);
			isGameVisible = showGame;
		}
	}

	void UpdateUIVisibility(bool force = false) {

		bool showUI = true;
		
		if (gameManager && gameManager.mainMenu.isVisible) {
			showUI = false;
		}
		/*
		if (gameManager && gameManager.isLoading) {
			showUI = false;
		}
		*/

		if (showUI != isGameUIVisible || force) {
			SetVisible(GameUI, showUI);
			isGameUIVisible = showUI;
		}		
	}

	void UpdateTargetCanvasVisibility() {

		bool show = false;

		if (gameManager && useAR && !gameManager.isARPaused && !gameManager.IsARTracked && !gameManager.mainMenu.popup.isVisible) {
			show = true;
		}
		/*
		if (gameManager && gameManager.isLoading) {
			show = false;
		}
		*/
		if (TargetCanvas)
			TargetCanvas.SetActive(show);
	}

	public void SetVisible(GameObject obj, bool visible) {

		if (obj == null)
			return;

		foreach (var rend in  obj.GetComponentsInChildren<Renderer>()) {
			rend.enabled = visible;
		}
		foreach (var canvas in obj.GetComponentsInChildren<Canvas>()) {
			canvas.enabled = visible;
		}
	}

	public virtual void InitConfig() {
		//@todo remove me
	}

	public virtual IEnumerator Init() {

		MyDebug.Log("BaseGameTypeManager - init");

		yield return new WaitUntil(() => {
			return gameType != null;
		});

		InitConfig();
		gameType.Init();

		//wasShowBeforeMenu = false;

		if (gameManager == null) {
			SetUseAR(false);
		}

		UpdateVisibility();

		isGameInit = true;

		// Fix font quality {
		uICanvas = gameUI.GetComponent<Canvas>();
		if (uICanvas != null) {
			yield return new WaitUntil(() => {
				return uICanvas != null && uICanvas.isActiveAndEnabled;
			});
			uICanvas = gameUI.GetComponent<Canvas>();
			canvasScaleFactor = uICanvas.scaleFactor;
			uICanvas.scaleFactor = 1f;
			yield return new WaitForEndOfFrame();
			/*
			if (gameManager) {
				if (useAR)
					yield return new WaitForSeconds(0.5f);
				yield return new WaitUntil(() => {
					return !gameManager.mainMenu.isVisible && uICanvas.isActiveAndEnabled;
				});
			}
			*/
			uICanvas.scaleFactor = canvasScaleFactor;
		}
		// }

	}

	public void SetUseAR(bool useAR) {
		this.useAR = useAR;
		if (NoARObj != null) {
			NoARObj.SetActive(!useAR);
		}
	}

	public virtual void TurnMusicOn(bool isOn) {
		if (gameManager)
			gameManager.TurnMusicOn(isOn);
	}

	public virtual void TurnSoundOn(bool isOn) {
			
		var audioSources = FindObjectsOfType<AudioSource>();
		foreach (var audio in audioSources) {
			audio.mute = !isOn;
		}
		if (gameManager) {
			gameManager.SetSoundOn(isOn);
			gameManager.music.mute = false;
		}
	}

	void Awake() {
		gameManager = FindObjectOfType<MainGameManager>();
		

    }

    void Start() {

		if (gameManager == null) {
			StartCoroutine(Init());
		}
    }

	void OnDestroy() {
		
		if (gameManager != null) {
			gameManager.wasTargetFound = false;
		}
	}

}
