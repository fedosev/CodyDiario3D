using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateGridPlayerDirection : BaseGameObjectState, IPointerClickHandler {

	Grid grid;

	public override BaseGameObjectState NextState() {

		if (grid.gameType == GameTypes.PATH) {
			var gameType = ((PathGameType)(grid.gameTypeConfig));
			if (gameType.path.Length == 0 && !gameType.ignoreCheckPath)
				return GoToState<StateSwitchQuad>();
		}

		return GoToState<StateNull>();
	}

	public override void OnEnter() {
		grid.InitDirectionalQuads();
	}

	public override void OnExit() {
		grid.ClearDirectionalQuads();
		grid.inPause = false;
		grid.SetActiveUI(true);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.pointerEnter.tag == "DirectionSelector") {
			RobyDirection direction = eventData.pointerEnter.GetComponent<QuadBehaviour>().GetDirection();
			grid.InitRobot1(grid.startPosInGrid.col, grid.startPosInGrid.row, direction, true);
			grid.state = NextState();
		} else if (eventData.pointerEnter.tag == "Quad") {
			GoToState<StateGridPlayerPosition>();
		}

	}

	// Use this for initialization
	void Awake () {
		grid = GetComponent<Grid>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
