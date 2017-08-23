using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCheck : MonoBehaviour, ISavable {

	public int index;
	public bool isBorder = false;

	public Color val;

	GameConfig gameConfig;
	CheckDistinct check;


	public void Setup(int index, Color color) {

		this.index = index;
		val = color;
		GetComponentInChildren<Image>().color = color;
	}

	public void Init() {

		gameConfig = MainGameManager.Instance.gameConfig;
		check = GetComponent<CheckDistinct>();
		if (isBorder && gameConfig.borderColor == val) {
			check.SetOn(true);
		}
		else if (!isBorder && gameConfig.quadColor == val) {
			check.SetOn(true);
		} else {
			check.SetOn(false);
		}		
	}

	void Start() {

		//Init();
	}

	public void Save() {

		var gameConfig = MainGameManager.Instance.gameConfig;
		if (isBorder) {
			gameConfig.borderColor = val;
		}
		else {
			gameConfig.quadColor = val;
		}
		gameConfig.Save();

		var grid = FindObjectOfType<Grid>();
		if (grid != null) {
			grid.UpdateColors();
		}		
	}	

}
