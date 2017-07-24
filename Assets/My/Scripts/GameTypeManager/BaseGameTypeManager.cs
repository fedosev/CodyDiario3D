using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameTypeManager : MonoBehaviour {

	//[System.NonSerialized]
	public BaseGameType gameType;

	public GameObject gameObj;

	public void ShowGame(bool show) {

		foreach (var rend in gameObj.GetComponentsInChildren<Renderer>()) {
			rend.enabled = show;
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

		yield return null;
	}

	void Start() {

		StartCoroutine(Init());		
	}

}
