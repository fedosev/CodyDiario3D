using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RobotStates { Idle, MovingForward, TurningLeft, TurningRight, WaitingForAnimation };

public class RobotController : MonoBehaviour, IDirection {

	[HideInInspector] public int index = -1;

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
			catch (Exception e) {
                Debug.LogError(e.Message);
            }

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
		MyDebug.Log(direction);
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
	public event Action OnFinishMovementOnce;
	public event Action<int> OnLose;

	bool isUpdatingCurrentQuad = false;
    public bool isFirstMove = true;

    private void ChangeQuadByGameType(GameObject currentQuad, GameObject nextQuad) { // @todo refactorig

		var quadBh = nextQuad.GetComponent<QuadBehaviour>();
		var prevQuadBh = currentQuad.GetComponent<QuadBehaviour>();

		grid.gameTypeConfig.ChangeQuad(this, prevQuadBh, quadBh);		

		/*
		switch (grid.gameType) {
			case GameTypes.FREE:
				break;
			case GameTypes.SNAKE:
			case GameTypes.PATH:
				break;
		}
		*/
		if (grid.gameTypeConfig.withLetters) {
			StartCoroutine(quadBh.AnimateLetter());
			MyDebug.Log(quadBh.letter);
		}
		
		isFirstMove = false;
	}

	private void UpdateCurrentQuadBegin() {
		ChangeQuadByGameType(
			grid.GetQuad(currentQuadRow, currentQuadCol),
			grid.GetQuad(currentQuadRow + (int)direction.z, currentQuadCol + (int)direction.x)
		);
		isUpdatingCurrentQuad = true;
	}

	public void UpdateCurrentQuadEnd() {
		prevQuad = grid.GetQuad(currentQuadRow, currentQuadCol);

		currentQuadCol += (int)direction.x;
		currentQuadRow += (int)direction.z;

		currentQuad = grid.GetQuad(currentQuadRow, currentQuadCol);

		isUpdatingCurrentQuad = false;
	}

	public void DoLose() {
		animator.SetTrigger("Lose");
		sounds.playLose();
		MyDebug.Log("You lose!");
		grid.inPause = true;
		if (OnLose != null) {
			OnLose(index);
		}
	}

	public bool IsMoving() {
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

		sounds.StopPlaying();

		yield return new WaitForSeconds(0.025f);

		transform.rotation = Quaternion.LookRotation(direction);
		
		var quadPos = grid.GetQuadPosition(currentQuadRow, currentQuadCol);
		transform.position = new Vector3(quadPos.x, transform.position.y, quadPos.z);

		currentState = RobotStates.Idle;
		
		StopCoroutine("WaitAndFixTransform");

		if (OnFinishMovementOnce != null) {
			OnFinishMovementOnce();
			OnFinishMovementOnce = null;
		}

		if (grid.inPause) {
			yield return new WaitUntil(() => !grid.inPause);
		}
		grid.NextAction();
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
		//MyDebug.Log((int)(transform.rotation.eulerAngles.y / 10));
		/*
		animator.MatchTarget(new Vector3(targetPosition.x, transform.position.y, transform.position.z), transform.rotation,
			AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 1f), 0.141f, 0.78f
		);
		*/
	}

    public bool CanMoveForward() {
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
		if (IsMoving())
			return;

		if (!CanMoveForward()) {
			if (grid.gameType != GameTypes.FREE) {
				DoLose();
				if (currentQuad != null)
					currentQuad.GetComponent<QuadBehaviour>().SetState(QuadStates.ERROR);
			}
			return;
		}

		currentState = RobotStates.MovingForward;
		animator.SetTrigger("Forward");
		sounds.PlaySound(sounds.soundStep);
		//sounds.PlayMoving();
		MyDebug.Log("Move Forward");
	}

	protected void Turn(int degrees) {
		if (IsMoving())
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

			MyDebug.Log("Turn Right");
		}
		else {
			currentState = RobotStates.TurningLeft;
			nextDirection = new Vector3(-direction.z, 0, direction.x);
			animator.SetBool("Turn Right", false);
			animator.SetTrigger("Turn");

			MyDebug.Log("Turn Left");
		}
		sounds.PlayStep();
		//sounds.PlayMoving();
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

		//MyDebug.Log("Stop Move");
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

		switch (currentState){
			case RobotStates.MovingForward:
				var nextQuadPos = grid.GetQuadPosition(currentQuadRow + (int)direction.z, currentQuadCol + (int)direction.x);
				if (direction.z == 0) { // Left-Right direction
					if (!isUpdatingCurrentQuad && (nextQuadPos.x - transform.position.x) * direction.x - grid.QuadSize / 2 <= 0) {
						UpdateCurrentQuadBegin();
					}
					if ((nextQuadPos.x - transform.position.x) * direction.x <= 0) {
						UpdateCurrentQuadEnd();
						StopMove();
					}
				}
				else if (direction.x == 0) { // Back-Forward direction
					if (!isUpdatingCurrentQuad && (nextQuadPos.z - transform.position.z) * direction.z - grid.QuadSize / 2 <= 0) {
						UpdateCurrentQuadBegin();
					}
					if ((nextQuadPos.z - transform.position.z) * direction.z <= 0) {
						UpdateCurrentQuadEnd();
						StopMove();
					}
				}
				break;
			case RobotStates.TurningLeft:
				var angleDiff = transform.rotation.eulerAngles.y - nextAngle;
				//MyDebug.Log(angleDiff);
				if (angleDiff <= 0 || angleDiff > 180 && angleDiff < 360) {
					StopMove();
				}
				break;
			case RobotStates.TurningRight:
				angleDiff = transform.rotation.eulerAngles.y - nextAngle;
				//MyDebug.Log(angleDiff);
				if (angleDiff >= 0 && angleDiff < 180) {
					StopMove();
				}
				break;
			default:
				break;
		}
	}

    void OnAnimatorMove() {
        switch (currentState) {
            case RobotStates.MovingForward:
                var vel = animator.velocity;
                vel.x *= Mathf.Abs(direction.x);
                vel.z *= Mathf.Abs(direction.z);
                transform.position += vel * Time.deltaTime;
                break;
            case RobotStates.TurningLeft:
            case RobotStates.TurningRight:
                transform.rotation *= Quaternion.Euler(animator.angularVelocity * Time.deltaTime * Mathf.Rad2Deg);
                break;
        }
    }

    void OnAnimatorIK() {

	}	
}
