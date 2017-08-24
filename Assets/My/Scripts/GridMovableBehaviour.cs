using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyARSample;

public class GridMovableBehaviour : MonoBehaviour {

	public bool isActive;

	private bool isMoving;
	private Vector3 posOffset;
    private TargetOnTheFly ui;

    // Use this for initialization
    void Start () {

		ui = FindObjectOfType<TargetOnTheFly>();

		//isActive = false;
		isMoving = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (!isActive || !ui.isShowUI)
			return;
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				//Debug.Log("Hit something: " + hit.transform.position);
				//if (hit.transform.gameObject.tag == "Quad") {
				if (/*hit.transform.gameObject == gameObject ||*/ hit.transform.parent.gameObject == gameObject
					|| hit.transform.parent.parent.gameObject == gameObject) {
					isMoving = true;
					posOffset = transform.position - hit.point;
				}
			}
		}
		else if (Input.GetMouseButtonUp(0)) {
			isMoving = false;
		}
		else if (isMoving && Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				gameObject.transform.position = new Vector3(
					hit.point.x + posOffset.x,
					0,
					hit.point.z + posOffset.z
				);
			}
		}
	}
}
