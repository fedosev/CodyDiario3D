using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateGridPlayerPosition : GameObjectState, IPointerClickHandler {

	Grid grid;

	public override void OnEnter() {
		grid.inPause = true;
		grid.SetActiveUI(false);
	}

	public override GameObjectState NextState() {

		return GoToState<StateGridPlayerDirection>();
	}

	public void OnPointerClick(PointerEventData eventData) {

		if (eventData.pointerEnter.tag == "Quad") {
			var quad = eventData.pointerEnter.GetComponent<QuadBehaviour>();
			var startPos = grid.GetQuadPositionInGrid(quad.gameObject);
			if (!grid.startPosInGrid.IsNull() && (startPos.row != grid.startPosInGrid.row || startPos.col != grid.startPosInGrid.col)) {
				grid.GetQuadBh(grid.startPosInGrid.row, grid.startPosInGrid.col).Undo();
			}
			grid.startPosInGrid = startPos;
			//grid.InitRobot1(grid.startPosInGrid.col, grid.startPosInGrid.row, RobyDirection.North);
			quad.SetState(QuadStates.ACTIVE);
			NextState();
		}
	}

	void Awake() {
		grid = GetComponent<Grid>();
	}
	
}
