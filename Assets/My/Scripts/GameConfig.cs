using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameConfig : ScriptableObject {

	public Color quadColor;
	public Color borderColor;


	public void Init() {

		// @todo...
		// tmp:
		GameObject.Find("BorderSave").GetComponent<Image>().color = borderColor;
		GameObject.Find("QuadSave").GetComponent<Image>().color = quadColor;

	}

	public void Save() {

		// @ reinit the game with new settings
		Debug.Log("GameConfig - Save");
		
		Init();
	}

}
