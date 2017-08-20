﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyAR;
using System;

public class MainGameManager : MonoBehaviour {

	private static MainGameManager instance;
	public static MainGameManager Instance { get {
		if (instance == null)
			instance = GameObject.FindObjectOfType<MainGameManager>();
		return instance;
	} }

	public AllGameTypes allGameTypes;

	public MainMenu mainMenu;

	public static MainMenu Menu { get {
		return MainGameManager.Instance.mainMenu;
	}}

    public ARCameraBaseBehaviour aRCamera;

    public Text gameTitle; //@todo

	public bool useAR = true;

	public GameObject aRGameObject;

	public GameObject targetsCanvas;

    public ImageTrackerBaseBehaviour imageTracker;
	public MyImageTargetBehaviour[] mainImageTargets;

	[HideInInspector] public bool wasTargetFound = false;

	public UnityEngine.UI.Image fadeOverlay;
	public float fadeDuration = 0.5f;

	IEnumerator fadeOverlayCoroutine;

	bool isMainImageTargetsActive = true;

	public BaseGameTypeManager gameTypeManager;

	public Camera noARCamera;

	[HideInInspector] public bool isLoading;
	[HideInInspector] public bool isBackground;


	BaseGameType gameType;

	int gameTypeIndex = 0;

	CameraDeviceBehaviour cameraDevice;
	EasyARBehaviour easyARBehaviour;

    bool useARchanged = true;
    bool isGameVisible;
    public bool IsARTracked { get {
		if (currentARTarget != null && currentARTarget.Status == TargetInstance.TrackStatus.Tracked) {
			return true;
		}
		return false;
	} }

    [HideInInspector] public bool isARPaused;

    IEnumerator coroutine;
    private TargetInstance currentARTarget;

    public bool IsGameInit() {
		if (gameTypeManager != null && gameTypeManager.isGameInit)
			return true;

		return false;
	}

	public IEnumerator Init(string gameTypeKey) {
		if (allGameTypes.TryGetValue(gameTypeKey, out gameType)) {
			StartCoroutine(Init(gameType));
		}
		yield return null;
	}

    public void UpdateVisibility() {

		if (gameTypeManager)
        	gameTypeManager.UpdateVisibility();
    }

    public void LoadGameType(BaseGameType gameType) {
		StartCoroutine(Init(gameType));
	}

    public IEnumerator Init(BaseGameType gameType) {

		AsyncOperation asyncOp;
		this.gameType = gameType;
		var t = Time.time;
		isLoading = true;
		isBackground = false;

		if (SceneManager.sceneCount > 1) {
			imageTracker.StopTrack();
			yield return StartCoroutine(FadeOverlay(true, fadeDuration));
			asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
			if (!useAR) {
				noARCamera.gameObject.SetActive(true);
			}
			yield return new WaitUntil(() => asyncOp.isDone && (Time.time - t) > fadeDuration);
		}
		SetUseAR();

		// Scene unloaded here

		var sceneName = gameType.sceneName;

		if (!useAR && sceneName != gameType.sceneNameNoAR) {
			sceneName = gameType.sceneNameNoAR;
		}

		asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		yield return new WaitUntil(() => {
			if (!asyncOp.isDone)
				return false;
			gameTypeManager = GameObject.FindObjectOfType<BaseGameTypeManager>();
			return gameTypeManager != null;
		});

		if (!useAR) {
			noARCamera.gameObject.SetActive(false);
		}

		gameTypeManager.gameType = gameType;
		gameTypeManager.SetUseAR(useAR);

		gameTypeManager.targetCanvas = targetsCanvas;

		StartCoroutine(gameTypeManager.Init());

		yield return new WaitUntil(() => gameTypeManager.isGameInit && (Time.time - t) > fadeDuration);

		isLoading = false;

		Menu.InfoPanel.Setup(gameType.title, gameType.GetInfo());

		Menu.coverButton.SetActive(false);
		Menu.resumeButton.SetActive(true);
		Menu.helpButton.SetActive(true);

		if (gameType.showInfoOnStart) {
			Menu.ShowInfoOnStart();
		} else {
			Menu.Hide(false);
			StartCoroutine(FadeOverlay(false, fadeDuration * 3));
		}

		if (useAR) {
			//imageTracker.StopTrack();
			imageTracker.StartTrack();
		}

		//gameTypeManager.ShowGame(false);

		//gameTypeManager.Init();
	}

	public IEnumerator InitBackground() {

		AsyncOperation asyncOp;

		isLoading = true;
		isBackground = true;

		useAR = false;
		useARchanged = true;

		Menu.Hide(false);
		fadeOverlay.gameObject.SetActive(true);
		fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, 1f);

		SetUseAR();

		noARCamera.gameObject.SetActive(true);

		if (SceneManager.sceneCount > 1) {
			//yield return StartCoroutine(FadeOverlay(true, fadeDuration));
			asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
			yield return new WaitUntil(() => asyncOp.isDone);
		}

		SceneManager.LoadScene("Background", LoadSceneMode.Additive);
		noARCamera.gameObject.SetActive(false);

		Menu.ShowMainOnStart();

		useAR = true;
		useARchanged = true;
	}

	public IEnumerator FadeOverlay(bool fadeIn, float duration) {
		if (fadeOverlayCoroutine != null)
			StopCoroutine(fadeOverlayCoroutine);
		fadeOverlayCoroutine = FadeOverlayAnimation(fadeIn, duration);
		yield return StartCoroutine(fadeOverlayCoroutine);
		//return fadeOverlayCoroutine;
	}

	IEnumerator FadeOverlayAnimation(bool fadeIn, float duration) {

		var t = Time.time;
		var dt = 0f;
		if (fadeIn) {
			fadeOverlay.gameObject.SetActive(true);
			//fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, 0f);
		}
		float val = 0f;
		while (dt < duration + Time.deltaTime) {
			val = (dt / duration);
			val = Mathf.Clamp01(val);
			if (!fadeIn) {
				//val = val < 0.5f ? 2 * val * val : -1 + (4 - 2 * val) * val; // easeInOutQuad
				val = val * (2 - val); // EaseOutQuad
				val = 1f - val;
			} else {
				val *= val; // EaseInQuad
			}
			fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, val);
			fadeOverlay.raycastTarget = val < 0.5f ? false : true;
			dt = Time.time - t;
			yield return null;
		}
		print(val);
		if (!fadeIn) {
			fadeOverlay.gameObject.SetActive(false);
			//fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, 1f);
		}
		
		//yield return null;
	}

    public void SetMainImgTargetsActive(bool activate, bool force = false) {

		if (activate && (force || !isMainImageTargetsActive)) {
			if (targetsCanvas)
				targetsCanvas.SetActive(true);
			foreach (var target in mainImageTargets) {
				imageTracker.LoadImageTargetBehaviour(target);
			}
			isMainImageTargetsActive = true;
		}
		else if (!activate && (force || isMainImageTargetsActive)) {
			if (targetsCanvas)
				targetsCanvas.SetActive(false);
			foreach (var target in mainImageTargets) {
				imageTracker.UnloadImageTargetBehaviour(target);
			}
			isMainImageTargetsActive = false;
		}

		if (force) { // @tmp
			StartCoroutine(ForceTargetsCanvasActive(activate, 1f));
		}
	}

	// @tmp
	public IEnumerator ForceTargetsCanvasActive(bool active, float duration) {
		var t = Time.time;
		while (Time.time < t + duration) {
			if (targetsCanvas)
				targetsCanvas.SetActive(active);
			yield return null;
		}
	}


	public void SetUseAR() {

		if (!useARchanged)
			return;

		if (useAR) {
			//easyARBehaviour.gameObject.SetActive(true);
			aRGameObject.gameObject.SetActive(true);
			aRCamera.enabled = true;
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
		LoadGameType(gameType);
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
			UpdateVisibility();
		}
	}

	public void ShowTargetCanvas(bool show) {
		targetsCanvas.SetActive(show);
	}

	IEnumerator ShowTargetCanvasDelayed(bool show) {
		
		if (show) {
			yield return new WaitForSeconds(1f);
			if (!isGameVisible && !isARPaused && isMainImageTargetsActive) {
				targetsCanvas.SetActive(true);
			}
		} else {
			targetsCanvas.SetActive(false);
		}
		yield return null;
	}

	public void PauseAR(bool pause) {
		if (!useAR)
			return;

		if (pause) {
			//EasyAR.Engine.Pause();
			aRCamera.enabled = false;
			imageTracker.StopTrack();

		} else {
			//EasyAR.Engine.Resume();
			aRCamera.enabled = true;
			imageTracker.StartTrack();
		}
		isARPaused = pause;
		//ShowTargetCanvas(!pause);
	}

	public void PauseGame(bool pause) {
		if (gameType)
			gameType.Pause(pause);
	}

	/*
	public void ShowGame(bool show) {
		if (coroutine != null)
			StopCoroutine(coroutine);
		coroutine = ShowTargetCanvasDelayed(!show);
		StartCoroutine(coroutine);
		//...
		gameTypeManager.UpdateVisibility();
		isGameVisible = show;
	}
	*/

	void Awake() {

		easyARBehaviour = FindObjectOfType<EasyARBehaviour>();
		cameraDevice = FindObjectOfType<CameraDeviceBehaviour>();
		if (imageTracker == null) {
			imageTracker = FindObjectOfType<ImageTrackerBaseBehaviour>();
		}
		aRCamera = FindObjectOfType<ARCameraBaseBehaviour>();
		aRCamera.FrameUpdate += (ARCameraBaseBehaviour aRCam, Frame frame) => {
			if (frame.Targets.Count > 0) {
				currentARTarget = frame.Targets[0];
			} else {
				currentARTarget = null;
			}
		};
	}

	void Start () {

		//StartCoroutine(Init("FreeMode01"));
		//StartCoroutine(Init("Cover"));

		fadeOverlay.gameObject.SetActive(true);
		fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, 1f);

		StartCoroutine(InitBackground());
	}
	
	public void LoadGameType(int index) {
		gameTypeIndex = index;
		if (index >= 0 && index < allGameTypes.testItems.Capacity)
			StartCoroutine(Init(allGameTypes.testItems[index].name));
	}

	public void LoadCover() {
		StartCoroutine(Init("Cover"));
	}

	public void Quit() {
		Debug.Log("Application Quit");
		Application.Quit();
	}

	void LoadNextGameType() {
		// @tmp
		StartCoroutine(Init(allGameTypes.testItems[(++gameTypeIndex) % allGameTypes.testItems.Count].name));
	}

	void Update () {
	
		/*
		if (Input.GetKeyDown(KeyCode.Escape)) {
			LoadNextGameType();
		}
		*/
	}
}
