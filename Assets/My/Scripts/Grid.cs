﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum GameTypes { FREE, TAP, SNAKE, PATH, DUEL, CONQUEST, ART, AUTO };

public enum PlayerTypes { VIRTUAL, REAL };

//[ExecuteInEditMode]
public class Grid : MonoBehaviour {

	public Vector3 baseScale;

	public ConfigScriptableObject config;

	public Transform[] quadTransforms;
	public QuadBehaviour[] quadBhs;
	//public Vector3[] quadPositions;
	public GameObject[] robotPrefabs;

	public GridMovableBehaviour movableBehaviour;

	public GameTypes gameType;

	public bool inPause;

	public bool _arMode = true;

    private float uiSize = 1;

	public float scaleSize = 4f;

	public GameObject UIControls;
	[HideInInspector] public bool canBeActiveUI = true;

	public int nCols = 0;
	public int nRows = 0;

	public int playersNumber = 1;

	public PlayerTypes[] playerTypes;
	
	public GameObject[] players;

	public int playerTurn = 1;

	public Queue<CardTypes?> actionsQueue;

	public bool InExecution { get; private set; }


	public GridRobyManager gameTypeManager;

	public GameObjectState state;

	public PositionInGrid startPosInGrid = new PositionInGrid(-1, -1);

	public List<QuadBehaviour> selectDirectionQuads = new List<QuadBehaviour>();

	public BaseGridRobyGameType gameTypeConfig;

	public Keyboard keyboard;

    public event Action OnNextTurn;

    public event Action<int> OnLose;

    public event Action<QuadBehaviour> OnQuadStateChange;

    [HideInInspector] public float nextActionDelay;
	
	RobotController[] robots;


	public float QuadSize { get {
		return quadTransforms[0].lossyScale.x;
	} }

	public RobotController CurrentRobotController { get {
		if (playerTurn >= 0 && players.Length > 0 && robots[playerTurn] != null)
			return robots[playerTurn];

		return null;
	} }

	public RobotController NextRobotController { get {
		var nextTurn = (playerTurn + 1) % playersNumber;
		if (playerTurn >= 0 && players.Length > nextTurn && robots[nextTurn] != null)
			return robots[nextTurn];

		return null;
	} }

	public RobotController GetRobotController(int index, bool next = false) {
		if (next)
			index = (index + 1) % playersNumber;
		return robots[index];
	}

	public int GetNextPlayerTurn() {
		return (playerTurn + 1) % playersNumber;
	}

    public void UpdateSize(float size) {
        uiSize = size;
		this.transform.localScale = baseScale * size;
	}

    public void UpdateSize() {
        UpdateSize(uiSize);
    }

    public Vector3 GetQuadPosition(int row, int col) {

		var quadSize = quadTransforms[0].lossyScale.x;
		var ratio = quadSize / config.size;
		var borderSize = config.borderSize * ratio;
		var a = quadSize + borderSize;
		var bx = (quadSize - transform.lossyScale.x) / 2 + borderSize;
		var bz = (quadSize - transform.lossyScale.y) / 2 + borderSize;
		
		return new Vector3(
			col * a + bx,
			0,
			row * a + bz
		) + transform.position;
	}

	public int GetOtherIndex(int index) {
		var row = index / nCols;
		var col = index % nCols;
		return ((nRows - row - 1) * nCols + col);
	}

	public GameObject GetQuad(int row, int col) {
        return quadTransforms[row * nCols + col].gameObject;
	}
	public QuadBehaviour GetQuadBh(int row, int col) {
		return quadBhs[row * nCols + col];
	}

	public QuadBehaviour GetQuadBh(int index) {
		return quadBhs[index];
	}

	public PositionInGrid GetQuadPositionInGrid(GameObject quad) {
		int i = 0;
		foreach (var quadTransform in quadTransforms) {
			if (quad.transform == quadTransform) {
				return new PositionInGrid((int)i / nCols, i % nRows);
			}
			i++;
		}
		return new PositionInGrid(-1, -1);
	}

	public void ClearGrid() {

		MyDebug.Log("Clear grid");

		//this.transform.parent = null;
		foreach (Transform child in this.transform) {
			#if UNITY_EDITOR
				DestroyImmediate(child.gameObject, true);
			#else
				Destroy(child.gameObject);
			#endif
				
		}
	}

	private void CreateBorder(Vector3 pos, float width, float height, Material mat) {

		//var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		var quad = Instantiate(config.borderPrefab);
		quad.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
		quad.transform.position = pos;
		if (width > 0)
			quad.transform.localScale = new Vector3(width, config.borderSize, 1f);
		else
			quad.transform.localScale = new Vector3(config.borderSize, height, 1f);
		quad.transform.parent = this.transform;
		//quad.tag = "Border";
		var rend = quad.GetComponent<Renderer>();

		rend.material = mat;
		mat.color = config.gameConfig.GetBorderColor();
		/*
		*/
	}

	private float GetAngleFromDirection(RobyDirection direction) {
        switch (direction) {
            case RobyDirection.North: return 0f;
            case RobyDirection.East: return 90f;
            case RobyDirection.South: return -180f;
            case RobyDirection.West: return -90f;
            default: return 0f;
		}
	}

	private Vector3 GetVector3FromDirection(RobyDirection direction) {
        switch (direction) {
            case RobyDirection.North: return Vector3.forward;
            case RobyDirection.East: return Vector3.right;
            case RobyDirection.South: return Vector3.back;
            case RobyDirection.West: return Vector3.left;
            default: return Vector3.forward;
		}
	}

	public void InitRobot(int index, string name, Vector3 pos, int x, int z, RobyDirection direction, GameObject quad, QuadStates quadState) {

		GameObject robotObj;
		if (index == 0 && (MainGameManager.Instance.today.IsChristmasPeriod() || gameTypeConfig.isChristmasPeriod))
			robotObj = config.robotWithHatPrefab;
		else
			robotObj = config.robotPrefabs[index];
		players[index] = Instantiate(robotObj, pos, Quaternion.AngleAxis(GetAngleFromDirection(direction), Vector3.up));
		players[index].name = name;
		RobotController rc = players[index].GetComponent<RobotController>();
		robots[index] = rc;
		rc.Init(this);
		rc.index = index;
		rc.SetRowCol(z, x);
		rc.SetDirection(RobyStartPosition.GetDirection(direction));
		rc.CurrentQuad = quad;
		rc.OnLose += OnLose;
		players[index].transform.localScale *= 2.5f * config.size;
		players[index].transform.parent = this.transform;
		players[index].GetComponent<PlayerBehaviour>().Index = index;
		//robotController = robotPrefabs.GetComponent<RobotController>();
		players[index].GetComponent<RobotSounds>().isAudioOn = config.gameConfig.isSoundOn;
		var quadBh = quad.GetComponent<QuadBehaviour>();
		quadBh.SetState(quadState);

		gameTypeConfig.OnInitRobot(rc, quadBh);
	}

	public void InitRobot(int index, string name, int col, int row, RobyDirection direction, QuadStates quadState) {
		var quad = GetQuad(row, col);
		InitRobot(index, name, quad.transform.position, col, row, direction, quad, quadState);
	}

	public void InitRobot1(int col, int row, RobyDirection direction, bool animated = false) {
		if (animated) {
			StartCoroutine(InitRobotAnimated(0, "Player 1", col, row, direction, QuadStates.ACTIVE));
		} else {
			InitRobot(0, "Player 1", col, row, direction, QuadStates.ACTIVE);
		}
	}

	public IEnumerator InitRobotAnimated(int index, string name, int col, int row, RobyDirection direction, QuadStates quadState) {
		var quad = GetQuad(row, col);
		if (particleSystemShowRoby == null)
			particleSystemShowRoby = Instantiate(config.particleSystemShowRobyPrefab).GetComponent<ParticleSystem>();
		particleSystemShowRoby.Stop();
		particleSystemShowRoby.transform.position = quad.transform.position;
		particleSystemShowRoby.Play();

		yield return new WaitForSeconds(0.5f);

		InitRobot(index, name, quad.transform.position, col, row, direction, quad, quadState);

		yield return null;
	}

	public QuadBehaviour InitDirectionalQuad(Vector3 pos, RobyDirection direction) {
		var quad = Instantiate(config.quadPrefab, pos, Quaternion.AngleAxis(90, Vector3.right));
		quad.transform.parent = this.transform;
		quad.tag = "DirectionSelector";
		quad.GetComponent<Renderer>().material = config.quadInfoMaterial;
		var quadBh = quad.GetComponent<QuadBehaviour>();
		quadBh.SetDirection(GetVector3FromDirection(direction));
		return quadBh;
	}


    ParticleSystem particleSystemShowRoby;

	 // @tmp ANIMATION VALUES {
	//[SerializeField]
	Ease e = Ease.OutElastic;
	//[SerializeField]
	float t = 0.5f;
	//[SerializeField]
	float v1 = 0.8f;
	//[SerializeField]
	float v2 = 0f;
    // }

    public void InitDirectionalQuads() {
		ClearDirectionalQuads(); //@tmp
		//var yOffset = new Vector3(0f, 0.01f, 0f);
		var yOffset = new Vector3(0f, 0.5f, 0f);
		selectDirectionQuads.Add(InitDirectionalQuad(GetQuadPosition(startPosInGrid.row + 1, startPosInGrid.col) + yOffset, RobyDirection.North));
		selectDirectionQuads.Add(InitDirectionalQuad(GetQuadPosition(startPosInGrid.row, startPosInGrid.col + 1) + yOffset, RobyDirection.East));
		selectDirectionQuads.Add(InitDirectionalQuad(GetQuadPosition(startPosInGrid.row - 1, startPosInGrid.col) + yOffset, RobyDirection.South));
		selectDirectionQuads.Add(InitDirectionalQuad(GetQuadPosition(startPosInGrid.row, startPosInGrid.col - 1) + yOffset, RobyDirection.West));

		// Animation
		var i = 0;
		foreach (var quad in selectDirectionQuads) {
			quad.transform.DOMoveY(0.01f, t).SetEase(e, v1, v1).SetDelay(i++ * 0.1f);
		}
	}

	public void ClearDirectionalQuads() {
		foreach (var quad in selectDirectionQuads) {
			Destroy(quad.gameObject);
		}
		selectDirectionQuads.Clear();
	}

	public void UpdateColors() {
		/*
		for (var i = 0; i < quadTransforms.Length; i++) {
			quadTransforms[i].
		}
		*/
		QuadBehaviour.GetMaterialForModifying(config.quadMaterial).color = config.gameConfig.GetQuadColor();
		QuadBehaviour.GetMaterialForModifying(config.borderMaterial).color = config.gameConfig.GetBorderColor();
	}

	public void GenerateGrid() {

		MyDebug.Log("Generate grid");

		var width = nCols * (config.size + config.borderSize) + config.borderSize;
		var height = nRows * (config.size + config.borderSize) + config.borderSize;

		var maxScale = Mathf.Max(width, height);
		baseScale = new Vector3(maxScale, maxScale, maxScale);
		this.transform.localScale = baseScale;

		config.quadPrefab.transform.localScale = new Vector3(
			config.size,
			config.size,
			config.size
		);

		players = new GameObject[playersNumber];
		robots = new RobotController[playersNumber];

		// BORDERS

		var borderMat = config.borderMaterial;
		
		borderMat = QuadBehaviour.GetMaterialForModifying(config.borderMaterial);
		borderMat.color = config.gameConfig.GetBorderColor();

		for (var x = 0; x <= nCols; x++) {
			Vector3 pos = new Vector3(
				x * (config.size + config.borderSize) + config.borderSize / 2 - width / 2,
				0.0001f,
				0
			) + transform.position;
			CreateBorder(pos, 0, height, borderMat);
		}
		for (var z = 0; z <= nRows; z++) {
			Vector3 pos = new Vector3(
				0,
				0.0001f,
				z * (config.size + config.borderSize) + config.borderSize / 2 - height / 2
			) + transform.position;
			CreateBorder(pos, width, 0, borderMat);
		}

		// QUADS

		var quadLength = nCols * nRows;
		quadTransforms = new Transform[quadLength];
		quadBhs = new QuadBehaviour[quadLength];
		//quadPositions = new Vector3[quadLength];

		QuadBehaviour.GetMaterialForModifying(config.quadMaterial).color = config.gameConfig.GetQuadColor();

		var i = 0;
		for (var z = 0; z < nRows; z++) {
			for (var x = 0; x < nCols; x++) {
				Vector3 pos = new Vector3(
					x * (config.size + config.borderSize) + config.size / 2 + config.borderSize - width / 2,
					0.0001f,
					z * (config.size + config.borderSize) + config.size / 2 + config.borderSize - height / 2
				) + transform.position;
				var quad = Instantiate(config.quadPrefab, pos, Quaternion.AngleAxis(90, Vector3.right));
				quad.transform.parent = this.transform;

				//quadPositions[i] = new Vector3(pos.x, pos.y, pos.z);
				quadTransforms[i] = quad.transform;

				var quadBh = quad.GetComponent<QuadBehaviour>();
				quadBhs[i] = quadBh;
				quadBh.index = i;

				i ++;


				if (gameTypeConfig.withLetters && gameTypeConfig.letters != null && gameTypeConfig.letters.Length == nRows) {
					quadBh.SetLetter(gameTypeConfig.letters[nRows - 1 - z][x]);
				}
				gameTypeConfig.SetupQuad(quadBh, x, z);

				if (playerTypes[0] == PlayerTypes.VIRTUAL && x == gameTypeConfig.startPosition.col && z == gameTypeConfig.startPosition.row) {

					InitRobot(0, "Player 1", pos, x, z, gameTypeConfig.startPosition.direction, quad, QuadStates.ACTIVE);
				}
				if (playersNumber > 1 &&
					// playerTypes[1] == PlayerTypes.VIRTUAL &&
					// x == nCols - 1 && z == nRows - 1
					x == gameTypeConfig.startPositionP2.col && z == gameTypeConfig.startPositionP2.row
				) {

					InitRobot(1, "Player 2", pos, x, z, gameTypeConfig.startPositionP2.direction, quad, QuadStates.ON);

					// @todo 
					if (playerTypes[1] == PlayerTypes.REAL) {
						var playerCursorImgTarget = GameObject.Find("PlayerImageTarget1");
						var cb = playerCursorImgTarget.GetComponent<PlayerTrackableBehaviour>();
						cb.playerGameObject = players[1];
						cb.enabled = true;
					}
				}
			}
		}

		/*
		foreach (var pl in players) {
			pl.GetComponent<RobotController>().AfterInit();
		}
		*/

		//CurrentRobotController.CurrentQuad.GetComponent<QuadBehaviour>().SetState(QuadStates.ACTIVE);

		var imgTarget = GameObject.Find("MainImageTarget");
		if (_arMode) {
			this.transform.parent = imgTarget.transform;
			baseScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}

		// @todo: remove {
		var cylTarget = GameObject.Find("CylinderTarget");
		if (cylTarget) {
			cylTarget.transform.position = new Vector3(-100, 1, 0);
		}
		var playerImgTarget = GameObject.Find("PlayerImageTarget");
		if (playerImgTarget) {
			playerImgTarget.transform.position = new Vector3(-50, 0, 0);
		}
		// }

		/*
		foreach (Transform child in this.transform) {
			child.gameObject.layer = 9;
		}
		*/
		
		//@tmp
		transform.parent.localScale *= scaleSize;
		transform.localScale *= ((float)config.gridNumberX / (float)nCols);

	}

    public void ShowBorders(bool show) {

        var borders = GameObject.FindGameObjectsWithTag("Border");
        if (show) {
            foreach (var border in borders) {
                border.GetComponent<Renderer>().material = config.borderMaterial; ;
            }
        }
        else {
            foreach (var border in borders) {
                border.GetComponent<Renderer>().material = config.transparentMaterial;
            }
        }

    }

	public void QuadStateChanged(QuadBehaviour quad) {
		if (OnQuadStateChange != null) {
			OnQuadStateChange(quad);
		}
	}

	public void Init() {
		//gameType = GameTypes.SNAKE;
		/*
		GenerateGrid();
		ClearGrid();
		// */

		if (nCols == 0 || nRows == 0) {
			nCols = config.gridNumberX;
			nRows = config.gridNumberZ;
		}

		if (playersNumber > 2) {
			Debug.LogError("Max 2 players");
		}

		for (var i = 0; i < 9; i++) // @tmp
			ClearGrid();
		//config.gameConfig.Init();
		GenerateGrid();

		inPause = false;

		if (gameTypeConfig.withLetters && (gameTypeConfig.letters == null || gameTypeConfig.letters.Length == 0)) {
			state.InitState<StateGridLettersSelector>();
		} else if (gameType != GameTypes.TAP && !gameTypeConfig.startPosition.IsSet()) {
			// Waiting for Roby's position
			state.InitState<StateGridPlayerPosition>();
		}

        actionsQueue = new Queue<CardTypes?>();

	}

	// Use this for initialization
	void Awake () {

		movableBehaviour = GetComponent<GridMovableBehaviour>();

		state = GameObjectState.Init(this.gameObject, newState => { state = newState; });

		if (UIControls == null) {
			UIControls = GameObject.Find("PanelCards");
		}
		//gameTypeManager = FindObjectOfType<BaseGridRobyManager>();

		/*
		for (var i = 0; i < 9; i++) // @tmp
			ClearGrid();
		*/
		//gameTypeManager.Init();
	}
	
	/*
	void OnEnable() {
		#if UNITY_EDITOR
			for (var i = 0; i < 9; i++)
				ClearGrid();
			GenerateGrid();
  		#endif
	}
	// */

	public void SetActiveUI(bool active) {
		UIControls.SetActive(canBeActiveUI && active);
	}

	public void SetActiveUIAnimated(bool active, float duration = 0.5f) {

		/*
		if (false && !canBeActiveUI) {
			SetActiveUI(false);
			return;
		}
		// */

		var hiddenY = -240f;
		var tr = UIControls.transform;
		if (active && !UIControls.activeSelf) {
			UIControls.SetActive(true);
			for (int i = 0; i < tr.childCount; i++) {
				var rt = tr.GetChild(i).GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, hiddenY);
				rt.DOAnchorPosY(40f, duration).SetEase(Ease.OutExpo);
			}
		} else if (!active && UIControls.activeSelf) {
			for (int i = 0; i < tr.childCount; i++) {
				var rt = tr.GetChild(i).GetComponent<RectTransform>();
				rt.DOAnchorPosY(hiddenY, duration * 0.5f).OnComplete(() => {
					UIControls.SetActive(false);
				});
			}
		}
	}

	public void SetRobotsColliderActive(bool active) {
		foreach (var robot in players) {
			var collider = robot.GetComponent<Collider>();
			if (collider != null) {
				collider.enabled = true;
			}
		}
	}

	public void RobotMoveForward() {
		if (inPause)
			return;
		var robotController = CurrentRobotController;
		if (robotController)
			CurrentRobotController.MoveForward();
	}
	public void RobotTurnLeft() {
		if (inPause)
			return;
		var robotController = CurrentRobotController;
		if (robotController)
			CurrentRobotController.TurnLeft();
	}
	public void RobotTurnRight() {
		if (inPause)
			return;
		var robotController = CurrentRobotController;
		if (robotController)
			CurrentRobotController.TurnRight();
	}

	public void NextTurn() {
		if (playersNumber > 1) {
			CurrentRobotController.CurrentQuadBh.SetState(QuadStates.ON);
			playerTurn = (playerTurn + 1) % playersNumber;
			CurrentRobotController.CurrentQuadBh.SetState(QuadStates.ACTIVE);
		}

		if (OnNextTurn != null) {
			OnNextTurn();
		}

		// @todo {
		/*
		if (playerTypes[playerTurn] == PlayerTypes.VIRTUAL) {
			UIControlls.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
		}
		else if (playerTypes[playerTurn] == PlayerTypes.REAL) {
			UIControlls.GetComponent<RectTransform>().offsetMin = new Vector2(1000, 1000);
		}
		*/
		// }
	}

	IEnumerator DoActionDelayed(CardTypes? cardType) {
		if (nextActionDelay > 0f)
			yield return new WaitForSeconds(nextActionDelay);

		switch (cardType) {
			case CardTypes.LEFT:
				CurrentRobotController.TurnLeft();
				break;
			case CardTypes.FORWARD:
				CurrentRobotController.MoveForward();
				break;
			case CardTypes.RIGHT:
				CurrentRobotController.TurnRight();
				break;
		}
		nextActionDelay = 0f;
	}

    public void NextAction() {

        if (gameType == GameTypes.SNAKE) {
            NextTurn();
        } else {
            if (actionsQueue.Count > 0) {
                InExecution = true;
                var cardType = actionsQueue.Dequeue();
				StartCoroutine(DoActionDelayed(cardType));
            } else {
                NextTurn();
                InExecution = false;
            }
        }
        
    }

    public void ClearActions() {
        actionsQueue.Clear();
    }

    public void AddAction(CardTypes? type) {
        actionsQueue.Enqueue(type);
    }
	
	private void PlayerLose(int player) {
		if (OnLose != null)
			OnLose(player);
	}

	public int PlayerQuadCount(int player) {
		int count = 0;
		foreach (var quad in quadBhs) {
			if (quad.player == player)
				count++;
		}
		return count;
	}

	public int QuadCount(Func<QuadBehaviour, bool> Condition) {
		int count = 0;
		foreach (var quad in quadBhs) {
			if (Condition(quad))
				count++;
		}
		return count;
	}

	void OnDestroy() {
		if (particleSystemShowRoby != null)
			Destroy(particleSystemShowRoby.gameObject);
	}
}
