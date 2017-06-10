using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : ScriptableObject {

	public Color quadColor;
	public Color borderColor;


	public void Init() {

		// @todo...
	}

	public void Save() {

		// @ reinit the game with new settings
		Debug.Log("GameConfig - Save");
		
		Init();
	}

}
