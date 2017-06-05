﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

	public Transform target;
	public Vector3 rotationOffset;
	private Grid grid;
	private Transform Target { get {
		if (!target && GetGrid && GetGrid.CurrentRobotController)
			target = grid.CurrentRobotController.transform;
		return target;
	} }
	private Grid GetGrid { get {
		if (!grid)
			grid = GameObject.Find("Grid").GetComponent<Grid>();
		return grid;
	} }
	void Start () {
		// grid = GameObject.Find("Grid").GetComponent<Grid>(); // CRASH
	}
	
	// Update is called once per frame
	void Update () {
		//return;
		if (Target) {
			transform.LookAt(Target);
			transform.Rotate(rotationOffset);
		}
	}
}
