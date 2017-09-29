using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateHexaSwitchQuad : BaseGameObjectState, IPointerDownHandler, IPointerUpHandler {


	public bool isDrawMode = false;

	public float drawDelay = 0.2f;

	Grid grid;
	QuadStates quadState = QuadStates.OBSTACLE;

	float pointerDown = float.MaxValue;


	public void OnPointerDown(PointerEventData eventData) {

		pointerDown = Time.time;

		//if (!isDrawMode || pointerDown == Time.time) {
			var gameObj = eventData.pointerCurrentRaycast.gameObject;
			if (gameObj.tag != "Quad")
				return;
			QuadBehaviour quad = gameObj.GetComponent<QuadBehaviour>();
			SwitchQuad(quad);
		//}

	}

	public void OnPointerUp(PointerEventData eventData) {
		pointerDown = float.MaxValue;
	}

	void SwitchQuad(QuadBehaviour quad) {
		if (quad.mainState == QuadStates.DEFAULT) {
			quad.SetState(quadState);
			grid.QuadStateChanged(quad);
		}
		else if (quad.mainState == quadState) {
			quad.SetState(QuadStates.DEFAULT);
			grid.QuadStateChanged(quad);
		}
	}

	void SetQuad(QuadBehaviour quad, bool on) {
		var state = on ? quadState : QuadStates.DEFAULT;
		if (quad.mainState != state) {
			quad.SetState(state);
			grid.QuadStateChanged(quad);
		}
	}

	void Update() {
		if (isDrawMode && pointerDown + drawDelay < Time.time && Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {

				// Check for not UI
				/*
             	var pointerData = new PointerEventData(EventSystem.current);
             	pointerData.position = Input.mousePosition;
				var raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerData, raycastResults);
				//MyDebug.Log(raycastResults.Count);
				if (raycastResults.Count > 1) {
					foreach (var res in raycastResults) {
						if (res.gameObject.layer == 5) { // UI
							return;
						}
					}
				}
				// */

				if (hit.transform.gameObject.tag == "Quad") {
					QuadBehaviour quad = hit.transform.GetComponent<QuadBehaviour>();
					SetQuad(quad, true);
					//SwitchQuad(quad);
				}
			}		
		}
	}

	void Awake() {
		grid = GetComponent<Grid>();
	}
	
}
