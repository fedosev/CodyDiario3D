using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyAR;

public class MainGameManager : MonoBehaviour {

	private static MainGameManager instance;
	public static MainGameManager Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<MainGameManager>();
		return instance;
	} }

	public AllGameTypes allGameTypes;

	public MainMenu mainMenu;

	public Text gameTitle; //@todo

	public bool useAR = true;

	public GameObject aRGameObject;

	public Canvas targetsCanvas;

    public ImageTrackerBaseBehaviour imageTracker;
	public MyImageTargetBehaviour[] mainImageTargets;

	public bool wasTargetFound = false;

	bool isMainImageTargetsActive = true;

	BaseGameTypeManager gameTypeManager;
	BaseGameType gameType;

	int gameTypeIndex = 0;

	CameraDeviceBehaviour cameraDevice;
	EasyARBehaviour easyARBehaviour;

    bool useARchanged = true;

    bool isGameVisible;
    bool isARPaused;
    IEnumerator coroutine;

	public bool IsGameInit() {
		if (gameTypeManager != null && gameTypeManager.isGameInit)
			return true;

		return false;
	}

    public IEnumerator Init(string gameTypeKey) {


		if (SceneManager.sceneCount > 1) {
			imageTracker.StopTrack();
			var asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
			yield return new WaitUntil(() => asyncOp.isDone);
		}

		SetUseAR();

		// Scene unloaded here

		if (allGameTypes.TryGetValue(gameTypeKey, out gameType)) {

			var sceneName = gameType.sceneName;

			if (!useAR && sceneName != gameType.sceneNameNoAR) {
				sceneName = gameType.sceneNameNoAR;
			}

			SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

			yield return new WaitUntil(() => {
				//gameTypeManager = GameObject.FindObjectOfType<GridRobyManager>();
				gameTypeManager = GameObject.FindObjectOfType<BaseGameTypeManager>();

				return gameTypeManager != null;
			});

			gameTypeManager.gameType = gameType;
			gameTypeManager.SetUseAR(useAR);

			StartCoroutine(gameTypeManager.Init());

			yield return new WaitUntil(() => gameTypeManager.isGameInit);

			if (useAR) {
				//imageTracker.StopTrack();
				imageTracker.StartTrack();
			}

			//gameTypeManager.ShowGame(false);

			//gameTypeManager.Init();
		}

	}

	public void SetMainImgTargetsActive(bool activate) {

		if (activate && !isMainImageTargetsActive) {
			targetsCanvas.gameObject.SetActive(true);
			foreach (var target in mainImageTargets) {
				imageTracker.LoadImageTargetBehaviour(target);
			}
			isMainImageTargetsActive = true;
		}
		else if (!activate && isMainImageTargetsActive) {
			targetsCanvas.gameObject.SetActive(false);
			foreach (var target in mainImageTargets) {
				imageTracker.UnloadImageTargetBehaviour(target);
			}
			isMainImageTargetsActive = false;
		}

	}

	public void SetUseAR() {

		if (!useARchanged)
			return;

		if (useAR) {
			//easyARBehaviour.gameObject.SetActive(true);
			aRGameObject.gameObject.SetActive(true);
			EasyAR.Engine.Resume();
			cameraDevice.OpenAndStart();
		} else {
			EasyAR.Engine.Pause();
			cameraDevice.Close();
			//easyARBehaviour.gameObject.SetActive(false);
			aRGameObject.gameObject.SetActive(false);
		}
		useARchanged = false;
	}
	
	public void RestartWithAR(bool useAR) {
		this.useAR = useAR;
		useARchanged = true;
		LoadGameType(gameTypeIndex);
	}

	public void ChangeWithAR(bool useAR) {
		if (gameType.sceneName != gameType.sceneNameNoAR) {
			RestartWithAR(useAR);
		} else {
			this.useAR = useAR;
			useARchanged = true;
			SetUseAR();
			gameTypeManager.SetUseAR(useAR);
			/*
			if (!useAR) {
				ShowGame(true);
			}
			*/
			ShowGame(!useAR);
		}
	}

	public void ShowTargetCanvas(bool show) {
		targetsCanvas.gameObject.SetActive(show);
	}

	IEnumerator ShowTargetCanvasDelayed(bool show) {
		
		if (show) {
			yield return new WaitForSeconds(1f);
			if (!isGameVisible && !isARPaused && isMainImageTargetsActive) {
				targetsCanvas.gameObject.SetActive(true);
			}
		} else {
			targetsCanvas.gameObject.SetActive(false);
		}
		yield return null;
	}

	public void PauseAR(bool pause) {
		if (pause) {
			EasyAR.Engine.Pause();
		} else {
			EasyAR.Engine.Resume();
		}
		isARPaused = pause;
		ShowTargetCanvas(!pause);
	}
	public void PauseGame(bool pause) {
		gameType.Pause(pause);
	}

	public void ShowGame(bool show) {
		if (coroutine != null)
			StopCoroutine(coroutine);
		coroutine = ShowTargetCanvasDelayed(!show);
		StartCoroutine(coroutine);
		gameTypeManager.ShowGame(show);
		isGameVisible = show;
	}

	void Awake() {

		easyARBehaviour = FindObjectOfType<EasyARBehaviour>();
		cameraDevice = FindObjectOfType<CameraDeviceBehaviour>();
		if (imageTracker == null) {
			imageTracker = FindObjectOfType<ImageTrackerBaseBehaviour>();
		}
	}

	void Start () {

		//StartCoroutine(Init("FreeMode01"));
		//StartCoroutine(Init("Cover"));
		LoadGameType(0);
	}
	
	public void LoadGameType(int index) {
		gameTypeIndex = index;
		if (index >= 0 && index < allGameTypes.items.Capacity)
			StartCoroutine(Init(allGameTypes.items[index].name));
	}

	public void Quit() {
		Debug.Log("Application Quit");
		Application.Quit();
	}

	void LoadNextGameType() {
		// @tmp
		StartCoroutine(Init(allGameTypes.items[(++gameTypeIndex) % allGameTypes.items.Count].name));
	}

	void Update () {
	
		/*
		if (Input.GetKeyDown(KeyCode.Escape)) {
			LoadNextGameType();
		}
		*/
	}
}
