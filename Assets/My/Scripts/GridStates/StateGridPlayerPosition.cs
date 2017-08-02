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
			var startPos = grid.GetQuadPositionInGrid(quad.gameObject);
			if (!grid.startPosInGrid.IsNull() && (startPos.row != grid.startPosInGrid.row ||startPos.col != grid.startPosInGrid.col)) {
				grid.GetQuad(grid.startPosInGrid.row, grid.startPosInGrid.col).GetComponent<QuadBehaviour>().Undo();
			}
			grid.startPosInGrid = startPos;
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
