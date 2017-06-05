using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchQuadStateBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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
						quad.SetState(QuadStates.OBSTACLE);
					}
					else if (quad.mainState == QuadStates.OBSTACLE) {
						quad.SetState(QuadStates.DEFAULT);
					}
				}
			}
		}
	}
}
