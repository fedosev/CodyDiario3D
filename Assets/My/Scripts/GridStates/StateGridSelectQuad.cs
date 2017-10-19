using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateGridSelectQuad : GameObjectState, IPointerClickHandler {

	public event Action<GameObjectState, QuadBehaviour> OnSelect;

	Grid grid;

	public override void OnEnter() {
		//grid.inPause = true;
		grid.SetActiveUI(false);
	}

	public override void OnExit() {
		//grid.inPause = false;
		//grid.SetActiveUI(true);
	}

	public void OnPointerClick(PointerEventData eventData) {
		
		if (eventData.pointerEnter != null && eventData.pointerEnter.tag == "Quad" && OnSelect != null) {
			var quad = eventData.pointerEnter.GetComponent<QuadBehaviour>();
			if (quad.isFake)
				return;
			OnSelect(this, quad);
		}
	}

	void Awake() {
		grid = GetComponent<Grid>();
	}
}
