using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameTypeManager : MonoBehaviour {

	//[System.NonSerialized]
	public BaseGameType gameType;

	public GameObject gameObj;

	public bool useAR = true;

	protected MainGameManager gameManager;

	public void ShowGame(bool show) {

		if (!useAR)
			return;

		foreach (var rend in gameObj.GetComponentsInChildren<Renderer>()) {
			rend.enabled = show;
		}
		foreach (var canvas in gameObj.GetComponentsInChildren<Canvas>()) {
			canvas.enabled = show;
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

		ShowGame(false);

		yield return null;
	}

	void Awake() {
		gameManager = FindObjectOfType<MainGameManager>();
	}
	void Start() {

		StartCoroutine(Init());
	}

}
