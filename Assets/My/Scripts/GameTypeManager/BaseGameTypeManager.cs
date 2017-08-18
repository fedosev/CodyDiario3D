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

	protected MainGameManager gameManager;

	protected bool gameCanBeShown = true;
	public bool wasShowBeforeMenu = false;



	bool isGameVisible = true;
	bool isGameUIVisible = true;


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
		if (gameManager && gameManager.mainMenu.isVisible) {
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

		if (gameManager && useAR && !gameManager.isARPaused && !gameManager.IsARTracked) {
			show = true;
		}
		/*
		if (gameManager && gameManager.isLoading) {
			show = false;
		}
		*/

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

		Debug.Log("BaseGameTypeManager - init");

		yield return new WaitUntil(() => {
			return gameType != null;
		});

		InitConfig();
		gameType.Init();

		wasShowBeforeMenu = false;

		if (gameManager == null) {
			SetUseAR(false);
		}

		UpdateVisibility();

		isGameInit = true;

		//yield return null;
	}

	public void SetUseAR(bool useAR) {
		this.useAR = useAR;
		if (NoARObj != null) {
			NoARObj.SetActive(!useAR);
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
		
		if (gameManager == null) {
			gameManager.wasTargetFound = false;
		}
	}

}
