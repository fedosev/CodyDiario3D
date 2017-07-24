using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour {

	public AllGameTypes allGameTypes;

	BaseGameTypeManager gameTypeManager;

	BaseGameType gameType;

	AsyncOperation asyncOp;

	public IEnumerator InitCover() {

		if (SceneManager.sceneCount > 1) {
			SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
		}

		if (allGameTypes.TryGetValue("Cover", out gameType)) {
			SceneManager.LoadScene(((CoverGameType)gameType).sceneName, LoadSceneMode.Additive);

			yield return new WaitUntil(() => {
				gameTypeManager = GameObject.FindObjectOfType<CoverManager>();
				return gameTypeManager != null;
			});
			 
			gameTypeManager.gameType = (CoverGameType)gameType;
			//gameTypeManager.Init();
		}

		
	}

	public IEnumerator Init(string gameTypeKey) {


		if (SceneManager.sceneCount > 1) {
			asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
			yield return new WaitUntil(() => asyncOp.isDone);
		}

		if (allGameTypes.TryGetValue(gameTypeKey, out gameType)) {


			SceneManager.LoadScene((gameType).sceneName, LoadSceneMode.Additive);

			yield return new WaitUntil(() => {
				gameTypeManager = GameObject.FindObjectOfType<GridRobyManager>();
				/*
				GameObject gtm;
				gtm = GameObject.Find("GameTypeManager");
				if (gtm != null) {
					gameTypeManager = gtm.AddComponent<GridRobyManager>() as GridRobyManager;
				}
				*/
				return gameTypeManager != null;
			});

			gameTypeManager.gameType = gameType;
			//gameTypeManager.Init();
		}

	}

	void Start () {

		//StartCoroutine(InitCover());
		//StartCoroutine(Init("FreeMode01"));
		StartCoroutine(Init("Cover"));
		//SceneManager.LoadScene("SceneSelector", LoadSceneMode.Additive);
	}
	
	void Update () {
		/*
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
			SceneManager.LoadScene("SceneSelector", LoadSceneMode.Additive);
		}
		 */
	}
}
