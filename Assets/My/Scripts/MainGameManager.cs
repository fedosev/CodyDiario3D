using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyAR;

public class MainGameManager : MonoBehaviour {

	public AllGameTypes allGameTypes;

	public MainMenu mainMenu;

	public Text gameTitle; //@todo

	BaseGameTypeManager gameTypeManager;
	BaseGameType gameType;

	int gameTypeIndex = 0;
    private ImageTrackerBaseBehaviour imageTracker;

    public IEnumerator Init(string gameTypeKey) {


		if (SceneManager.sceneCount > 1) {
			imageTracker.StopTrack();
			var asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
			yield return new WaitUntil(() => asyncOp.isDone);
		}

		// Scene unloaded here

		if (allGameTypes.TryGetValue(gameTypeKey, out gameType)) {

			SceneManager.LoadScene((gameType).sceneName, LoadSceneMode.Additive);

			yield return new WaitUntil(() => {
				//gameTypeManager = GameObject.FindObjectOfType<GridRobyManager>();
				gameTypeManager = GameObject.FindObjectOfType<BaseGameTypeManager>();

				return gameTypeManager != null;
			});

			gameTypeManager.gameType = gameType;

			imageTracker.StartTrack();

			//gameTypeManager.ShowGame(false);

			//gameTypeManager.Init();
		}

	}

	public void ShowGame(bool show) {

		gameTypeManager.ShowGame(show);
	}

	void Awake() {

		imageTracker = FindObjectOfType<ImageTrackerBaseBehaviour>();
	}

	void Start () {

		//StartCoroutine(Init("FreeMode01"));
		//StartCoroutine(Init("Cover"));
		LoadGameType(0);
	}
	
	public void LoadGameType(int index) {
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
