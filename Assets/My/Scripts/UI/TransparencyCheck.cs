using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransparencyCheck : MonoBehaviour, ISavable {

	public int index;
	public bool isBorder = false;

	public float val;

	GameConfig gameConfig;

	CheckDistinct check;


	public void Setup(int index, float val) {

		this.index = index;
		this.val = val;
	}

	public void Init() {

		gameConfig = MainGameManager.Instance.gameConfig;
		check = GetComponent<CheckDistinct>();
		if (isBorder && gameConfig.borderColorAlpha == val) {
			check.SetOn(true);
		}
		else if (!isBorder && gameConfig.quadColorAlpha == val) {
			check.SetOn(true);
		} else {
			check.SetOn(false);
		}		
	}

	void Start() {

		//Init();
	}

	public void Save() {

		if (isBorder) {
			gameConfig.borderColorAlpha = val;
		}
		else {
			gameConfig.quadColorAlpha = val;
		}
		gameConfig.Save();

		var grid = FindObjectOfType<Grid>();
		if (grid != null) {
			grid.UpdateColors();
		}
	}

}
