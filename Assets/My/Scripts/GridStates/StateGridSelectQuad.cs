using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateGridSelectQuad : BaseGameObjectState, IPointerClickHandler {

	public event Action<BaseGameObjectState, QuadBehaviour> OnSelect;

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
		
		if (eventData.pointerEnter.tag == "Quad" && OnSelect != null) {
			var quad = eventData.pointerEnter.GetComponent<QuadBehaviour>();
			OnSelect(this, quad);
		}
	}

	void Awake() {
		grid = GetComponent<Grid>();
	}
}
