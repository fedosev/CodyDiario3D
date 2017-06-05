using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RobotStates { Idle, MovingForward, TurningLeft, TurningRight, WaitingForAnimation };

public class RobotController : MonoBehaviour, IDirection {

	protected ConfigScriptableObject config;
    protected Animator animator;
	protected GameObject gridGameObj;
	protected Grid grid;
	//Quaternion rotation;
	public float gridWidth;
	public float gridHeight;

	public RobotStates currentState;

	private GameObject currentQuad;
	public GameObject CurrentQuad { get {
		if (currentQuad == null) {
			try {
				currentQuad = grid.GetQuad(currentQuadRow, currentQuadCol);
			}
			catch (Exception e) { }

		}
		return currentQuad;
	} set {
		currentQuad = value;
	} }

	public GameObject prevQuad;

	protected Vector3 direction;
	protected Vector3 nextDirection;
	protected float nextAngle;

	protected int currentQuadRow;
	protected int currentQuadCol;

	protected RobotSounds sounds;

    public bool isFreeMove = true;

    public Vector3 Direction { get {
		Debug.Log(direction);
		return direction;
	} }

	public PositionInGrid PositionInGrid { get {
		return new PositionInGrid(
			currentQuadRow,
			currentQuadCol
		);
	} }

	public PositionInGrid ForwardPositionInGrid { get {
		return new PositionInGrid(
			currentQuadRow + (int)direction.z,
			currentQuadCol + (int)direction.x
		);
	} }

	public event DirectionChangeAction OnDirectionChange;

	public void UpdateCurrentQuad() {
		prevQuad = grid.GetQuad(currentQuadRow, currentQuadCol);

		currentQuadCol += (int)direction.x;
		currentQuadRow += (int)direction.z;

		currentQuad = grid.GetQuad(currentQuadRow, currentQuadCol);

		// @tmp
		// if (true || GetComponent<HighlightBehaviour>().isActiveAndEnabled || isFreeMove)
			return;

		var quadBh = currentQuad.GetComponent<QuadBehaviour>();
		var prevQuadBh = prevQuad.GetComponent<QuadBehaviour>();
		prevQuadBh.SetState(QuadStates.OBSTACLE);
		if (quadBh.IsFreeToGoIn()) {
			quadBh.SetState(QuadStates.ACTIVE);
			//sounds.playSound(sounds.soundStep);
		} else {
			quadBh.SetState(QuadStates.ERROR);
			DoLose();
		}
	}

	public void DoLose() {
			animator.SetTrigger("Lose");
			sounds.playLose();
			Debug.Log("You lose!");
			grid.inPause = true;
	}

	public bool isMoving() {
		return currentState != RobotStates.Idle;
	}

	IEnumerator WaitAndFixTransform(Vector3 directionV3, string triggerName) {
		direction = directionV3;

		yield return new WaitForSeconds(1f);

		/*
		if (direction == Vector3.forward)
			transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f));
		else if (direction == Vector3.left)
			transform.rotation = Quaternion.LookRotation(new Vector3(-1f, 0f, 0f));
		else if (direction == Vector3.back)
			transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, -1f));
		else if (direction == Vector3.right)
			transform.rotation = Quaternion.LookRotation(new Vector3(1f, 0f, 0f));
		*/

		transform.rotation = Quaternion.LookRotation(direction);

		if (triggerName != "") {
			animator.SetTrigger(triggerName);
		}
		else {
			currentState = RobotStates.Idle;
		}

		StopCoroutine("WaitAndFixTransform");
	}

	IEnumerator WaitAndFixTransform() {

		if (OnDirectionChange != null && direction != nextDirection) {
			OnDirectionChange(nextDirection, direction);
		}

		direction = nextDirection;

		yield return new WaitForSeconds(0.25f);

		transform.rotation = Quaternion.LookRotation(direction);
		
		var quadPos = grid.getQuadPosition(currentQuadRow, currentQuadCol);
		transform.position = new Vector3(quadPos.x, transform.position.y, quadPos.z);

		currentState = RobotStates.Idle;
		
		StopCoroutine("WaitAndFixTransform");

		if (!grid.inPause)
			grid.NextTurn();
	}

	private void MoveArroundUpdate() {
		if (!animator.IsInTransition(0)) {
			var pos = transform.position;
			var gridScale = gridGameObj.transform.lossyScale;
			var quadSize = gridGameObj.GetComponent<Grid>().quadTransforms[0].lossyScale.x;
			var borderSize = quadSize * config.borderSize / config.size;
			var gridPos = grid.transform.position;
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Forward") && (
				direction == Vector3.left && pos.x - gridPos.x <= -(gridScale.x - 1.3 * quadSize)  / 2 + borderSize ||
				direction == Vector3.right && pos.x - gridPos.x >= (gridScale.x - 1.3 * quadSize) / 2 - borderSize ||
				direction == Vector3.forward && pos.z - gridPos.z >= (gridScale.y - 1.3 * quadSize) / 2 - borderSize ||
				direction == Vector3.back && pos.z - gridPos.z <= -(gridScale.y - 1.3 * quadSize) / 2 + borderSize)
			) {
				animator.SetTrigger("Turn");
			}
			else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Turn")) {
				if (direction == Vector3.right && ((int)(transform.rotation.eulerAngles.y / 10)) == 0) {
					animator.SetTrigger("Stop");
					StartCoroutine(WaitAndFixTransform(Vector3.forward, "Forward"));
				}
				else if (direction == Vector3.forward && ((int)(transform.rotation.eulerAngles.y / 10)) == 27) {
					animator.SetTrigger("Stop");
					StartCoroutine(WaitAndFixTransform(Vector3.left, "Forward"));
				}
				else if (direction == Vector3.left && ((int)(transform.rotation.eulerAngles.y / 10)) == 18) {
					animator.SetTrigger("Stop");
					StartCoroutine(WaitAndFixTransform(Vector3.back, "Forward"));
				}
				else if (direction == Vector3.back && ((int)(transform.rotation.eulerAngles.y / 10)) == 9) {
					animator.SetTrigger("Stop");
					StartCoroutine(WaitAndFixTransform(Vector3.right, "Forward"));
				}
			}
		}
		//Debug.Log((int)(transform.rotation.eulerAngles.y / 10));
		/*
		animator.MatchTarget(new Vector3(targetPosition.x, transform.position.y, transform.position.z), transform.rotation,
			AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 1f), 0.141f, 0.78f
		);
		*/
	}

	public bool canMoveForward() {
		int nextQuadCol = currentQuadCol + (int)direction.x;
		int nextQuadRow = currentQuadRow + (int)direction.z;
		bool canMove = nextQuadCol >= 0 && nextQuadCol < config.gridNumberX &&
				nextQuadRow >= 0 && nextQuadRow < config.gridNumberZ;

		if (!canMove)
			return false;

		var nextQuad = grid.GetQuad(nextQuadRow, nextQuadCol);

		return nextQuad.GetComponent<QuadBehaviour>().IsFreeToGoIn();

	}
	public void MoveForward() {
		if (isMoving())
			return;

		if (!canMoveForward()) {
		/* @tmp
			DoLose();
			if (currentQuad != null)
				currentQuad.GetComponent<QuadBehaviour>().SetState(QuadStates.ERROR);
		*/
			return;
		}

		currentState = RobotStates.MovingForward;
		animator.SetTrigger("Forward");
		sounds.playSound(sounds.soundStep);
		Debug.Log("Move Forward");
	}
	protected void Turn(int degrees) {
		if (isMoving())
			return;

		if (!(degrees == 90 || degrees == -90))
			return;

		nextAngle = (transform.rotation.eulerAngles.y + degrees) % 360;

		if (degrees > 0) {
			// https://en.wikipedia.org/wiki/Rotation_matrix
			nextDirection = new Vector3(direction.z, 0, -direction.x);

			currentState = RobotStates.TurningRight;
			animator.SetBool("Turn Right", true);
			animator.SetTrigger("Turn");

			Debug.Log("Turn Right");
		}
		else {
			currentState = RobotStates.TurningLeft;
			nextDirection = new Vector3(-direction.z, 0, direction.x);
			animator.SetBool("Turn Right", false);
			animator.SetTrigger("Turn");

			Debug.Log("Turn Left");
		}
		sounds.playStep();
	}

	public void TurnLeft() {
		Turn(-90);
	}
	public void TurnRight() {
		Turn(90);
	}
	
	protected void StopMove() {
		animator.SetTrigger("Stop");
		currentState = RobotStates.WaitingForAnimation;

		StartCoroutine(WaitAndFixTransform());

		Debug.Log("Stop Move");
	}

	public void SetRowCol(int row, int col) {
		currentQuadRow = row;
		currentQuadCol = col;
		//currentQuad = grid.GetQuad(currentQuadRow, currentQuadCol);
	}

	public void SetDirection(Vector3 direction) {
		this.direction = direction;
	}

	public void AfterInit() {
		//currentQuad = grid.GetQuad(currentQuadRow, currentQuadCol);
	}

    void Start() {
        animator = GetComponent<Animator>();
		sounds = GetComponent<RobotSounds>();
		//rotation = GetComponentInChildren<Transform>().rotation;
		gridGameObj = GameObject.Find("Grid");
		config = gridGameObj.GetComponent<Grid>().config;
		gridWidth = config.gridNumberX * (config.size + config.borderSize) + config.borderSize;
		gridHeight = config.gridNumberZ * (config.size + config.borderSize) + config.borderSize;

		nextDirection = direction;
		
		grid = gridGameObj.GetComponent<Grid>();

		currentState = RobotStates.Idle;

		//currentQuad = grid.GetQuad(currentQuadRow, currentQuadCol);

		if (isFreeMove) {
			GetComponent<HighlightBehaviour>().enabled = false;
			animator.SetTrigger("Forward");
			//MoveForward();
		}
		
    }	
	// Update is called once per frame
	void Update() {

		if (isFreeMove) {
			MoveArroundUpdate();
			return;
		}

		switch (currentState)
		{
			case RobotStates.MovingForward:
				var nextQuadPos = grid.getQuadPosition(currentQuadRow + (int)direction.z, currentQuadCol + (int)direction.x);
				if (direction.z == 0) { // Left-Right direction
					if ((nextQuadPos.x - transform.position.x) * direction.x <= 0) {
						UpdateCurrentQuad();
						StopMove();
					}
				}
				else if (direction.x == 0) { // Back-Forward direction
					if ((nextQuadPos.z - transform.position.z) * direction.z <= 0) {
						UpdateCurrentQuad();
						StopMove();
					}
				}
				break;
			case RobotStates.TurningLeft:
				var angleDiff = transform.rotation.eulerAngles.y - nextAngle;
				//Debug.Log(angleDiff);
				if (angleDiff <= 0 || angleDiff > 180 && angleDiff < 360) {
					StopMove();
				}
				break;
			case RobotStates.TurningRight:
				angleDiff = transform.rotation.eulerAngles.y - nextAngle;
				//Debug.Log(angleDiff);
				if (angleDiff >= 0 && angleDiff < 180) {
					StopMove();
				}
				break;
			default:
				break;
		}
	}

	void OnAnimatorIK() {

	}	
}
