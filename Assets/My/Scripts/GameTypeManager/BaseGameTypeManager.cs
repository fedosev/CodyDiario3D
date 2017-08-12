using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameTypeManager : MonoBehaviour {

	//[System.NonSerialized]
	public BaseGameType gameType;

	public GameObject gameObj;

    public GameObject NoARObj;

	public bool useAR = true;

	public bool isGameInit = false;

	protected MainGameManager gameManager;

	protected bool gameCanBeShown = true;


    public void ShowGame(bool show) {

		if (!useAR && !show)
			return;

		if (!gameCanBeShown && show)
			return;
			
		foreach (var rend in gameObj.GetComponentsInChildren<Renderer>()) {
			rend.enabled = show;
		}
		foreach (var canvas in gameObj.GetComponentsInChildren<Canvas>()) {
			canvas.enabled = show;
		}
		Debug.Log(show);
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

		if (gameManager == null) {
			SetUseAR(false);
		}

		if (!gameManager.wasTargetFound || !gameCanBeShown) {
			ShowGame(false);
		}

		isGameInit = true;

		yield return null;
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

}
