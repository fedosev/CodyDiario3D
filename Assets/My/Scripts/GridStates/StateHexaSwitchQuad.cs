using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateHexaSwitchQuad : BaseGameObjectState, IPointerClickHandler {

	Grid grid;

	public void OnPointerClick(PointerEventData eventData) {

		var gameObj = eventData.pointerCurrentRaycast.gameObject;
		if (gameObj.tag != "Quad")
			return;


		QuadStates quadState = QuadStates.OBSTACLE;
		QuadBehaviour quad = gameObj.GetComponent<QuadBehaviour>();
		if (quad.mainState == QuadStates.DEFAULT) {
			quad.SetState(quadState);
		}
		else if (quad.mainState == quadState) {
			quad.SetState(QuadStates.DEFAULT);
		}
		grid.QuadStateChanged(quad);
	}

	void Awake() {
		grid = GetComponent<Grid>();
	}
	
}
