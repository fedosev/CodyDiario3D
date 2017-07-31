using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchQuadStateBehaviour : MonoBehaviour {


	Grid grid;

	// Use this for initialization
	void Start () {
		grid = GetComponent<Grid>();
	}
	
	// Update is called once per frame
	void Update () {
		
		QuadStates state;
		if (grid.gameType == GameTypes.PATH) {
			state = QuadStates.PATH;
		} else if (grid.gameType == GameTypes.SNAKE) {
			state = QuadStates.OBSTACLE;
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
				if (raycastResults.Count > 0)
					return;

				if (hit.transform.gameObject.tag == "Quad") {
					QuadBehaviour quad = hit.transform.GetComponent<QuadBehaviour>();
					if (quad.mainState == QuadStates.DEFAULT) {
						quad.SetState(state);
					}
					else if (quad.mainState == state) {
						quad.SetState(QuadStates.DEFAULT);
					}
				}
			}
		}
	}
}
