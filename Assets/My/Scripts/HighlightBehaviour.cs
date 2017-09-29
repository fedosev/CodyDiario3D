using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirection {
	Vector3 Direction { get; }

	event DirectionChangeAction OnDirectionChange;
}

public class HighlightBehaviour : MonoBehaviour, IDirection {

	private ConfigScriptableObject config;
	private GameObject gridGameObj;
	private Grid grid;
	private	Transform quadTransformIn;
	private Transform quadTransformInPrev;
	private int quadLength;

	public bool useDirectionalCursor = true;
	public bool isActiveChecking = false; // @tmp: get value by REAL / VIRTUAL

	private Vector3 direction;

	public Vector3 Direction { get {
		if (robotController != null)
			return robotController.Direction;

		return direction;
	} }

	public event DirectionChangeAction OnDirectionChange;

	private RobotController robotController;


	private bool directionChanged = false;

	private void DirectionChangeAction(Vector3 dir, Vector3 prevDir) {
		directionChanged = true;
	}

	void OnEnable() {
		robotController = GetComponent<RobotController>();
		if (robotController != null) {
			robotController.OnDirectionChange += DirectionChangeAction;
		}
		else {
			OnDirectionChange += DirectionChangeAction;
		}
	}

	// Use this for initialization
	void Start() {
		gridGameObj = GameObject.Find("Grid");
		grid = gridGameObj.GetComponent<Grid>();
		config = grid.config;
		quadLength = config.gridNumberX * config.gridNumberZ;
		
	}
	
	// Update is called once per frame
	void Update() {

		//@tmp (temporarily disabled)
		return;
		
		var pos = this.transform.position;
		int l = quadLength = config.gridNumberX * config.gridNumberZ;
		if (!grid) {
			grid = gridGameObj.GetComponent<Grid>();
			if (!grid)
				return;
		}
		if (grid.inPause)
			return;

		QuadBehaviour quad;
		for (int i = 0; i < l; i++) {
			var quadPos = grid.quadTransforms[i].position;
			var quadSizeHalf = grid.quadTransforms[i].lossyScale.x / 2;
			if (pos.x > quadPos.x - quadSizeHalf && pos.x < quadPos.x + quadSizeHalf &&
				pos.z > quadPos.z - quadSizeHalf && pos.z < quadPos.z + quadSizeHalf)
			{
				if (((quadTransformInPrev != quadTransformIn) && (grid.quadTransforms[i] != quadTransformIn)) || !quadTransformInPrev && !quadTransformIn) {
					quadTransformInPrev = quadTransformIn;
					quadTransformIn = grid.quadTransforms[i];
					if (quadTransformInPrev && (quad = quadTransformInPrev.GetComponent<QuadBehaviour>())) {
						//quad.SetState(QuadStates.OBSTACLE); // @tmp
						quad.SetState(QuadStates.DEFAULT); // @tmp
					}
					quad = quadTransformIn.GetComponent<QuadBehaviour>();
					if (quad) {
						if (quad.IsFreeToGoIn()) { // @tmp
							var player = GetComponent<PlayerBehaviour>();
							if (player.IsMyTurn())
								quad.SetState(QuadStates.ACTIVE);
							else
								quad.SetState(QuadStates.ON);
							if (useDirectionalCursor) {
								quad.SetDirection(Direction);
							}
						}
						else if (isActiveChecking) {
							RobotController rc = GetComponent<RobotController>();
							if (rc) {
								quad.SetState(QuadStates.ERROR);
								rc.DoLose();
							}
							else {
								quad.SetState(QuadStates.WARNING);
							}
						}
					}
					//MyDebug.Log("Color change");
				}
			}
		}
		if (directionChanged) {
			if (useDirectionalCursor) {
				quad = quadTransformIn.GetComponent<QuadBehaviour>();
				if (quad) {
					quad.SetDirection(Direction);
				}
			}
			directionChanged = false;
		}
	}
}
