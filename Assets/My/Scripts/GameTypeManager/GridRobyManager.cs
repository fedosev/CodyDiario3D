using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRobyManager : BaseGameTypeManager {

	//[System.NonSerialized]
	//public new BaseGridRobyGameType gameType;

	public Grid grid;


	public override void InitConfig() {

		((BaseGridRobyGameType)gameType).grid = grid;
		grid.gameTypeManager = this;
		grid.gameTypeConfig = (BaseGridRobyGameType)gameType;

	}
	
	void Awake() {
	}

}
