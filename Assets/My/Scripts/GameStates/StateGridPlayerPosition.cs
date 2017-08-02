using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateGridPlayerPosition : BaseGameObjectState, IPointerClickHandler {

	Grid grid;

	public override void OnEnter() {
		grid.inPause = true;
	}

	public override BaseGameObjectState NextState() {

		return GoToState<StateGridPlayerDirection>();
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.pointerEnter.tag == "Quad") {
			var quad = eventData.pointerEnter.GetComponent<QuadBehaviour>();
			grid.startPosInGrid = grid.GetQuadPositionInGrid(quad.gameObject);
			//grid.InitRobot1(grid.startPosInGrid.col, grid.startPosInGrid.row, RobyDirection.North);
			quad.SetState(QuadStates.ACTIVE);
			NextState();
		}
	}

	// Use this for initialization
	void Awake() {
		grid = GetComponent<Grid>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
