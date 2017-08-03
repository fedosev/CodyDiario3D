using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateSwitchQuad : BaseGameObjectState {

	Grid grid;

	void Awake() {
		grid = GetComponent<Grid>();
	}
	
	// Update is called once per frame
	void Update() {
		
		QuadStates quadState;
		if (grid.gameType == GameTypes.PATH) {
			quadState = QuadStates.PATH;
		} else if (true || grid.gameType == GameTypes.SNAKE) {
			quadState = QuadStates.OBSTACLE;
		} else
			return;

		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				Debug.Log("Hit something: " + hit.transform.position);

				// Check for not UI
             	var pointerData = new PointerEventData(EventSystem.current);
             	pointerData.position = Input.mousePosition;
				var raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerData, raycastResults);
				//Debug.Log(raycastResults.Count);
				if (raycastResults.Count > 1) {
					foreach (var res in raycastResults) {
						if (res.gameObject.layer == 5) { // UI
							return;
						}
					}
				}
				/*
				*/

				if (hit.transform.gameObject.tag == "Quad") {
					QuadBehaviour quad = hit.transform.GetComponent<QuadBehaviour>();
					if (quad.mainState == QuadStates.DEFAULT) {
						quad.SetState(quadState);
					}
					else if (quad.mainState == quadState) {
						quad.SetState(QuadStates.DEFAULT);
					}
				}
			}
		}
	}
}
