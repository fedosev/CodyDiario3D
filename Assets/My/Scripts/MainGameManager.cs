
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyAR;
using System;
using System.Diagnostics;

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

	public bool shoudUseAR;
	public bool useAR;

	public GameObject aRGameObject;

	public GameObject targetsCanvas;

    public ImageTrackerBaseBehaviour imageTracker;
	public MyImageTargetBehaviour[] mainImageTargets;

	[HideInInspector] public bool wasTargetFound = false;

	public UnityEngine.UI.Image fadeOverlay;
	public float fadeDuration = 0.5f;

	bool isMainImageTargetsActive = true;

	public BaseGameTypeManager gameTypeManager;

	public Camera noARCamera;

	public ConfigScriptableObject config;
	public GameConfig gameConfig;

	public AudioSource music;

	public Text logText;

	public bool isUnlocked = false;

	public MyDate today;

	public DebugPanel debugPanel;

	[HideInInspector] public bool isLoading;
	[HideInInspector] public bool isBackground;

	IEnumerator fadeOverlayCoroutine;

	BaseGameType gameType;

	int gameTypeIndex = 0;

	CameraDeviceBehaviour cameraDevice;
	EasyARBehaviour easyARBehaviour;

    bool useARchanged = true;
    bool isGameVisible;

    [HideInInspector] public bool isARPaused;

    //IEnumerator coroutine;
    TargetInstance currentARTarget;

    bool isDebug;
    bool isDebugOnScreen;

	#if F_AR_ENABLED
		const int gameTypeSceneIndex = 1;
	#else
		const int gameTypeSceneIndex = 2;
	#endif


    public void SetDebugMode(bool isOn) {
		isDebug = isOn;
		isDebugOnScreen = isOn;
		debugPanel.gameObject.SetActive(isOn);
		logText.gameObject.SetActive(isOn);
		Menu.aRCheck.gameObject.SetActive(isOn);
		if (isOn) {
			Menu.aRCheck.onValueChanged.AddListener(ChangeWithAR);
		} else {
			Menu.aRCheck.onValueChanged.RemoveListener(ChangeWithAR);
		}
		Menu.debugOptions.SetActive(isOn);
	}

	public void ToggleDebugMode() {
		SetDebugMode(!isDebug);
	}

    public bool IsARTracked { get {
		if (currentARTarget != null && currentARTarget.Status == TargetInstance.TrackStatus.Tracked) {
			return true;
		}
		return false;
	} }

	public void Unlock() {
		isUnlocked = true;
		today = allGameTypes.endDate;
		DaySelector.Instance.RefreshNextTime();
	}

	public void SetLogText(string str) {
		if (isDebugOnScreen)
			logText.text = str;
	}

	public virtual void TurnMusicOn(bool isOn) {
		if (!isLoading || isBackground) {
			gameConfig.isMusicOn = isOn;
			gameConfig.Save();
		}
		TurnMusicOnAction(isOn);
	}

	public virtual void TurnMusicOnAction(bool isOn) {
		if (isOn) {
			music.Play();
		} else {
			music.Stop();
		}
	}

    public void SetSoundOn(bool isOn) {
		if (!isLoading || isBackground) {
			gameConfig.isSoundOn = isOn;
			gameConfig.Save();
		}
    }

	public virtual void TurnSoundOn(bool isOn) {
		SoundManager.Instance.audioSource.mute = !isOn;
		if (gameTypeManager) {
			gameTypeManager.TurnSoundOn(isOn);
		} else {
			SetSoundOn(isOn);
		}
	}
	

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

		if (gameTypeManager) {
        	gameTypeManager.UpdateVisibility();
		} else if (isBackground) {
			#if F_AR_ENABLED
				targetsCanvas.SetActive(false);
			#endif
		}

		mainMenu.popup.gameObject.SetActive(!mainMenu.isVisible && mainMenu.popup.isVisible);
    }

    public void LoadGameType(BaseGameType gameType) {
		StartCoroutine(Init(gameType));
	}

    public void LoadTodayGameType() {
		if (!today.IsGTE(allGameTypes.startDate) || today.IsGTE(allGameTypes.endDate)) {
			LoadCover();
			return;
		}
		foreach (var month in allGameTypes.months) {
			if (month.month == today.month) {
				int dayIndex = (today.day - 1) / 2;
				if (dayIndex < month.days.Count) {
					StartCoroutine(Init(month.days[dayIndex]));
				}
				return;
			}
		}
	}

    public IEnumerator Init(BaseGameType gameType) {

		AsyncOperation asyncOp;
		this.gameType = gameType;
		var t = Time.time;
		isLoading = true;

		#if !F_AR_ENABLED
			if (isBackground) {
				var light = GameObject.Find("RealityDirectionalLight").GetComponent<Light>();
				light.enabled = true;
				light.intensity = 1f;
			}
		#endif

		isBackground = false;

		if (SceneManager.sceneCount > gameTypeSceneIndex) {
			#if F_AR_ENABLED
				aRCamera.enabled = false;
				imageTracker.StopTrack();
			#endif
			yield return StartCoroutine(FadeOverlay(true, fadeDuration));
			asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(gameTypeSceneIndex));
			if (!useAR) {
				noARCamera.gameObject.SetActive(true);
			}
            yield return new WaitUntil(() => asyncOp.isDone && (Time.time - t) > fadeDuration);
			System.GC.Collect();
		}
		#if F_AR_ENABLED
			else if (!useAR) {
				noARCamera.gameObject.SetActive(true);
			}
		#endif
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

		gameTypeManager.WinAction = mainMenu.popup.ShowWin;
		gameTypeManager.WinTextAction = mainMenu.popup.ShowWinText;
		gameTypeManager.LoseAction = mainMenu.popup.ShowLose;
		gameTypeManager.LoseTextAction = mainMenu.popup.ShowLoseText;

		StartCoroutine(gameTypeManager.Init());

		yield return new WaitUntil(() => gameTypeManager.isGameInit && (Time.time - t) > fadeDuration);

		TurnSoundOn(gameConfig.isSoundOn);

		isLoading = false;

		if (gameType.name == "Cover") {
			if (!today.IsGTE(allGameTypes.startDate))
				((CoverGameType)gameType).SetOtherSubTitle(false);
			else if (today.IsGTE(allGameTypes.endDate))
				((CoverGameType)gameType).SetOtherSubTitle(true);

		}
		
		Menu.InfoPanelObj.Setup(gameType.title, gameType.subTitle, gameType.GetInfo());

		if (gameType.name != "ScanOptions")
			Menu.AfterFirstGameLoad();

		if (useAR) {
			//imageTracker.StopTrack();
			//imageTracker.StartTrack();
			PauseAR(false);
			yield return new WaitForSeconds(0.2f);
			PauseAR(true);
		}
		
		Menu.popup.Init();

		var showInfoOnStart = gameType.showInfoOnStart;
		#if UNITY_EDITOR && F_DEBUG
			showInfoOnStart = false;
		#endif
		if (showInfoOnStart) {
			Menu.ShowInfoOnStart();
			Menu.InfoPanelObj.OnFirstClose += GameTypeStart;
		} else {
			Menu.Hide(false);
			StartCoroutine(FadeOverlay(false, fadeDuration * 3));
			StartCoroutine(GameTypeStartDelayed(fadeDuration * 3));
		}

		//gameTypeManager.ShowGame(false);

		//gameTypeManager.Init();
	}

	public IEnumerator InitBackground() {

		TurnSoundOn(gameConfig.isSoundOn);

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

		if (SceneManager.sceneCount > gameTypeSceneIndex) {
			//yield return StartCoroutine(FadeOverlay(true, fadeDuration));
			asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(gameTypeSceneIndex));
			yield return new WaitUntil(() => asyncOp.isDone);
		}

		SceneManager.LoadScene("Background", LoadSceneMode.Additive);
		noARCamera.gameObject.SetActive(false);

		Menu.ShowMainOnStart();

		#if F_AR_ENABLED
			useAR = !isDebug || shoudUseAR;
			useARchanged = true;
		#endif
	}

	void GameTypeStart() {
		StartCoroutine(GameTypeStartDelayed(fadeDuration * 1.5f));
	}

	public IEnumerator GameTypeStartDelayed(float delay) {
		yield return new WaitForSeconds(delay);
		gameTypeManager.StartGameType();
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
		//MyDebug.Log(val);
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


	[Conditional("F_AR_ENABLED")]
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

	public void RestartGameType() {
		LoadGameType(gameType);
	}

	public void ChangeWithAR(bool useAR) {
		if (gameType && (gameType.sceneName != gameType.sceneNameNoAR)) {
			RestartWithAR(useAR);
		} else {
			this.useAR = useAR;
			useARchanged = true;
			SetUseAR();
			if (gameTypeManager)
				gameTypeManager.SetUseAR(useAR);
			if (mainMenu.isVisible && useAR) {
				isARPaused = false;
				PauseAR(true);
			}
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

	[Conditional("F_AR_ENABLED")]
	public void PauseAR(bool pause) {
		if (!useAR)
			return;

		if (pause && !isARPaused) {
			//EasyAR.Engine.Pause();
			aRCamera.enabled = false;
			if (gameTypeManager)
				gameTypeManager.wasShowBeforeMenu = IsARTracked;
			imageTracker.StopTrack();

		} else if (!pause && isARPaused) {
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

		#if !UNITY_EDITOR && F_AR_ENABLED
			shoudUseAR = true;
		#endif

		#if F_ALLOW_DEBUG
			MyDebug.ShowOnScreen = SetLogText;
		#endif

		#if F_DEBUG
			isDebug = true;
			Menu.aRCheck.gameObject.SetActive(true);
			Menu.aRCheck.isOn = shoudUseAR;
			Menu.aRCheck.onValueChanged.AddListener(ChangeWithAR);
			Menu.debugOptions.SetActive(true);
		#else
			isDebug = false;
			Menu.aRCheck.gameObject.SetActive(false);
			Menu.debugOptions.SetActive(false);
		#endif

		#if F_DEBUG_ON_SCREEN
			isDebugOnScreen = true;
			debugPanel.gameObject.SetActive(true);
		#else
			isDebugOnScreen = false;
			debugPanel.gameObject.SetActive(false);
		#endif

		useAR = shoudUseAR;

		today = new MyDate();

		#if F_UNLOCK
			Unlock();
		#endif

		gameConfig.Init();

		#if F_AR_ENABLED
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
		#endif

		TurnMusicOnAction(gameConfig.isMusicOn);
	}

	void Start () {

		//StartCoroutine(Init("FreeMode01"));
		//StartCoroutine(Init("Cover"));

		fadeOverlay.gameObject.SetActive(true);
		fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, 1f);

		#if !F_AR_ENABLED
			if (SceneManager.sceneCount == 1) {
				SceneManager.LoadScene("Reality", LoadSceneMode.Additive);
			}
		#endif

		StartCoroutine(InitBackground());
	}
	
	public void LoadGameType(int index) {
		gameTypeIndex = index;
		if (index >= 0 && index < allGameTypes.items.Capacity)
			StartCoroutine(Init(allGameTypes.items[index].name));
	}

	public void LoadScanOptions() {
		#if F_AR_ENABLED
			if (!useAR) {
				useAR = true;
				useARchanged = true;
				SetUseAR();
			}
			StartCoroutine(Init("ScanOptions"));
		#endif
	}

	public void LoadCover() {
		StartCoroutine(Init("Cover"));
	}

	public void Quit() {
		MyDebug.Log("Application Quit");
		Application.Quit();
	}

	public void ChangeQuality(bool increase) {
		if (increase) {
			QualitySettings.IncreaseLevel();
			//QualitySettings.resolutionScalingFixedDPIFactor = 1.5f;
		}
		else {
			QualitySettings.DecreaseLevel();
			//QualitySettings.resolutionScalingFixedDPIFactor = 1f;
		}
		MyDebug.Log(QualitySettings.GetQualityLevel(), true);
	}

	public void SetTimeScale(float timeScale) {
		Time.timeScale = timeScale;
	}

	void LoadNextGameType(int offset = 1) {
		//StartCoroutine(Init(allGameTypes.items[(++gameTypeIndex) % allGameTypes.items.Count].name));

		if (gameType == null)
			return;
		AllGameTypes.Month month;
		var monthNumber = gameType.month;
		if (allGameTypes.TryGetMonth(monthNumber, out month)) {
			var i = -1;
			var dayFound = false;
			foreach (var day in month.days) {
				i++;
				if (gameType == day) {
					dayFound = true;
					break;
				}
			}
			if (dayFound) {
				i += offset;
				if (i < 0) {
					monthNumber--;
					if (monthNumber < 1)
						monthNumber = 12;
				} else if (i >= month.days.Count) {
					monthNumber++;
					if (monthNumber > 12)
						monthNumber = 1;
					i = 0;
				}
				if (allGameTypes.TryGetMonth(monthNumber, out month)) {
					StartCoroutine(Init(month.days[i < 0 ? month.days.Count - 1 : i]));
				}
			}
		}
	}

	void Update () {
		/*
		if (Input.GetKeyDown(KeyCode.Escape)) {
			LoadNextGameType();
		}
		*/
		#if UNITY_EDITOR
			var inputStr = Input.inputString;
			if (Input.GetKeyDown(KeyCode.W) && Input.inputString == "w") {
				Time.timeScale *= 2;
				MyDebug.Log(Time.timeScale + "x");
			} else if (Input.GetKeyDown(KeyCode.Q) && Input.inputString == "q") {
				Time.timeScale *= 0.5f;
				MyDebug.Log(Time.timeScale + "x");
			} else if (Input.GetKeyDown(KeyCode.Alpha1)) {
				LoadNextGameType(-1);
			} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
				LoadNextGameType();
			} else if (Input.GetKeyDown("`")) {
				RestartGameType();
			}
		#endif
	}
}
