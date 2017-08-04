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

	BaseGameTypeManager gameTypeManager;
	BaseGameType gameType;

	int gameTypeIndex = 0;

	CameraDeviceBehaviour cameraDevice;
	EasyARBehaviour easyARBehaviour;
    public ImageTrackerBaseBehaviour imageTracker;

    bool useARchanged = true;

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

			if (!useAR) {
				sceneName += "NoAR";
			}

			SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

			yield return new WaitUntil(() => {
				//gameTypeManager = GameObject.FindObjectOfType<GridRobyManager>();
				gameTypeManager = GameObject.FindObjectOfType<BaseGameTypeManager>();

				return gameTypeManager != null;
			});

			gameTypeManager.gameType = gameType;
			gameTypeManager.useAR = useAR;

			if (useAR) {
				imageTracker.StartTrack();
			}

			//gameTypeManager.ShowGame(false);

			//gameTypeManager.Init();
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

	public void ShowGame(bool show) {

		gameTypeManager.ShowGame(show);
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
		StartCoroutine(Init(allGameTypes.items[index].name));
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
