using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateGridPlayerDirection : BaseGameObjectState, IPointerClickHandler {

	Grid grid;

	public override void OnEnter() {
		grid.InitDirectionalQuads();
	}

	public override void OnExit() {
		grid.ClearDirectionalQuads();
		grid.inPause = false;
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.pointerEnter.tag == "DirectionSelector") {
			RobyDirection direction = eventData.pointerEnter.GetComponent<QuadBehaviour>().GetDirection();
			grid.InitRobot1(grid.startPosInGrid.col, grid.startPosInGrid.row, direction);
			NextState();
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
