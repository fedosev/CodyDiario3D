using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void PositionChangeAction(PositionInGrid currentPosition, PositionInGrid previousPosition);
public delegate void DirectionChangeAction(Vector3 currentDirection, Vector3 previousDirection);

[System.Serializable]
public struct PositionInGrid {

	public int row;
	public int col;

	public PositionInGrid(int row, int col) {
		this.row = row;
		this.col = col;
	}

	public void SetNull() {
		row = -1;
		col = -1;
	}

	public bool IsNull() {
		return row == -1 || col == -1;
	}

	public override string ToString() {
		return "[" + row + ", " + col +  "]";
	}

	public bool Equals(PositionInGrid obj) {
		return col == obj.col && row == obj.row;
	}

}


public class PlayerTrackableBehaviour : MonoBehaviour {

    private Grid grid;
    private ConfigScriptableObject config;
    private int quadLength;

 	private Vector3 direction;
	public Vector3 Direction { get {
		return direction;
	} }

	private Vector3 cursorDirection;
	
	private PositionInGrid positionInGrid;
    private PositionInGrid cursorPosInGrid;

	private bool isStartPositionFixes = true;

	private float timeStart = -1;

	[SerializeField]
	private float delay = 1.0f;

	public GameObject playerGameObject;
	
	private RobotController robotController;
	private PlayerBehaviour playerBehaviour;


	// <currentPosition, previousPosition>
	//public event Action<PositionInGrid, PositionInGrid> OnCursorPositionChange;
	
	public event PositionChangeAction OnCursorPositionChange;
	public event DirectionChangeAction OnCursorDirectionChange;

	//public event PositionChangeAction OnPositionChange;
	//public event DirectionChangeAction OnDirectionChange;

	private enum ActionType { Null, MoveForward, TurnLeft, TurnRight };
	private ActionType nextAction = ActionType.Null;

	[SerializeField]
	private float angleTolerance = 5;


    // Use this for initialization
    void OnEnable() {

		grid = GameObject.Find("Grid").GetComponent<Grid>();
		config = grid.config;
		quadLength = config.gridNumberX * config.gridNumberZ;

		// @todo
		playerGameObject = GameObject.Find("Player 2");
		robotController = playerGameObject.GetComponent<RobotController>();
		playerBehaviour = playerGameObject.GetComponent<PlayerBehaviour>();

		positionInGrid = new PositionInGrid();
		cursorPosInGrid = new PositionInGrid();

		// @todo {
		if (isStartPositionFixes) {
			direction = Vector3.forward;
			positionInGrid.row = config.gridNumberX - 1;
			positionInGrid.col = config.gridNumberZ - 1;
		}
		// }

		cursorPosInGrid.SetNull();

		// Events
		OnCursorPositionChange += CursorPositionChangeAction;
		OnCursorDirectionChange += CursorDirectionChangeAction;

	}
	
	// Update is called once per frame
	void Update () {

		if (grid.inPause || !playerBehaviour.IsMyTurn())
			return;

		UpdateCursorPosition();

		UpdateCursorDirection();
		
		if ((Time.time - timeStart) > delay && nextAction != ActionType.Null) {
			if (nextAction == ActionType.MoveForward) {
				robotController.MoveForward();
			}
			else if (nextAction == ActionType.TurnLeft) {
				robotController.TurnLeft();
			}
			else if (nextAction == ActionType.TurnRight) {
				robotController.TurnRight();
			}
			nextAction = ActionType.Null;

		}

		// Debug.Log(transform.localEulerAngles.y + ", " + SelfAngleYTest()); // More or less the same stuff
		// _testPlane.rotation = Quaternion.LookRotation(transform.up, Vector3.back);
		// _testPlane.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z));



	}

	private void UpdateCursorPosition() {

		var pos = this.transform.position;
		int l = quadLength = config.gridNumberX * config.gridNumberZ;

		for (int i = 0; i < l; i++) {
			var quadPos = grid.quadTransforms[i].position;
			var quadSizeHalf = grid.quadTransforms[i].lossyScale.x / 2;
			if (pos.x > quadPos.x - quadSizeHalf && pos.x < quadPos.x + quadSizeHalf &&
				pos.z > quadPos.z - quadSizeHalf && pos.z < quadPos.z + quadSizeHalf)
			{

				// var index = row * config.gridNumberX + col;
				
				int row = (int) (i / config.gridNumberX);
				int col = (int) (i % config.gridNumberX);

				if (cursorPosInGrid.row != row || cursorPosInGrid.col != col) {

					if (OnCursorPositionChange != null) {
						OnCursorPositionChange(new PositionInGrid(row, col), cursorPosInGrid);
					}

					cursorPosInGrid.row = row;
					cursorPosInGrid.col = col;
				}

				return;
			}
		}

	}

	private QuadBehaviour GetQuad(PositionInGrid posInGr) {
		var quadGO = grid.GetQuad(posInGr.row, posInGr.col);
		if (!quadGO)
			throw new Exception("No quad in " + posInGr);

		return quadGO.GetComponent<QuadBehaviour>();
	}

	private void CursorPositionChangeAction(PositionInGrid posInGr, PositionInGrid prevPosInGr) {

		MyDebug.Log("Cursor position changed: " + prevPosInGr + " -> " + posInGr);

		if (!prevPosInGr.IsNull())
			GetQuad(prevPosInGr).Undo();


		var quad = GetQuad(posInGr);
		quad.RecordUndo();
		quad.SetDirection(direction);

		CheckNextAction(posInGr, direction, quad);

	}

	private void CheckNextAction(PositionInGrid pos, Vector3 dir, QuadBehaviour quad) {

		if (pos.Equals(robotController.ForwardPositionInGrid) && direction == robotController.Direction) {
			timeStart = Time.time;
			nextAction = ActionType.MoveForward;
			quad.SetState(QuadStates.CURSOR_ON);
			MyDebug.Log("You are going to move forward");
		}
		else if (pos.Equals(robotController.PositionInGrid) && direction == Quaternion.Euler(0, -90, 0) * robotController.Direction) {
			timeStart = Time.time;
			nextAction = ActionType.TurnLeft;
			quad.SetState(QuadStates.CURSOR_ON);
			MyDebug.Log("You are going to turn left");
		}
		else if (pos.Equals(robotController.PositionInGrid) && direction == Quaternion.Euler(0, 90, 0) * robotController.Direction) {
			timeStart = Time.time;
			nextAction = ActionType.TurnRight;
			quad.SetState(QuadStates.CURSOR_ON);
			MyDebug.Log("You are going to turn right");
		}
		else {
			nextAction = ActionType.Null;
			quad.SetState(QuadStates.CURSOR_WARNING);
			// Debug.Log("You will not move here");
		}
	}

	private float SelfAngleYTest() {
		return Vector3.Angle(
			Vector3.forward,
			Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized
		);
	}

	/*
	private float SelfAngleY() {
		return Quaternion.Angle(
			Quaternion.LookRotation(Vector3.up),
			Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized
		);
	}
	// */

	public void UpdateCursorDirection() {

		var ang = transform.localEulerAngles.y;
		var prevDir = direction;

		if (direction == Vector3.forward) {
			if (ang > 45 + angleTolerance && ang < 135) {
				direction = Vector3.right;
			}
			else if (ang < 315 - angleTolerance && ang > 225) {
				direction = Vector3.left;
			}
		}
		else if (direction == Vector3.right) {
			if (ang > 135 + angleTolerance) {
				direction = Vector3.back;
			}
			else if (ang < 45 - angleTolerance) {
				direction = Vector3.forward;
			}
		}
		else if (direction == Vector3.back) {
			if (ang > 225 + angleTolerance) {
				direction = Vector3.left;
			}
			else if (ang < 135 - angleTolerance) {
				direction = Vector3.right;
			}
		}
		else if (direction == Vector3.left) {
			if (ang > 315 + angleTolerance) {
				direction = Vector3.forward;
			}
			else if (ang < 225 - angleTolerance) {
				direction = Vector3.back;
			}
		}

		if (!prevDir.Equals(direction)) {
			if (OnCursorDirectionChange != null) {
				OnCursorDirectionChange(direction, prevDir);
			}
		}

	}
	
	private void CursorDirectionChangeAction(Vector3 dir, Vector3 prevDir) {

		MyDebug.Log("Cursor direction changed: " + prevDir + " -> " + dir);

		if (!cursorPosInGrid.IsNull()) {
			var quad = GetQuad(cursorPosInGrid);
			if (!quad)
				return;
			quad.SetDirection(dir);
			CheckNextAction(cursorPosInGrid, dir, quad);

		}
		
	}

}
