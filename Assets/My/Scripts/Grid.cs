using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameTypes { SNAKE, PATH };

public enum PlayerTypes { VIRTUAL, REAL };

//[ExecuteInEditMode]
public class Grid : MonoBehaviour {

	public Vector3 baseScale;

	public ConfigScriptableObject config;

	public Transform[] quadTransforms;
	//public Vector3[] quadPositions;
	public GameObject[] robotPrefabs;

	public GridMovableBehaviour movableBehaviour;
	public SwitchQuadStateBehaviour switchQuadStateBehaviour;

	public GameTypes gameType;

	public bool inPause;

	public bool _arMode = true;

    private float uiSize = 1;

	public GameObject UIControlls;

	public int playersNumber = 1;

	public PlayerTypes[] playerTypes;
	
	public GameObject[] players;

	public int playerTurn = 1;

	public RobotController CurrentRobotController { get {
		if (playerTurn >= 0)
			return players[playerTurn].GetComponent<RobotController>();

		return null;
	} }

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
		var a = quadSize + config.borderSize * ratio;
		var bx = (quadSize - transform.lossyScale.x) / 2;
		var bz = (quadSize - transform.lossyScale.y) / 2;
		
		return new Vector3(
			col * a + bx,
			0,
			row * a + bz
		) + transform.position;
	}

	public GameObject GetQuad(int row, int col) {
		var index = row * config.gridNumberX + col;
		return quadTransforms[index].gameObject;
	}

	public void ClearGrid() {

		//this.transform.parent = null;
		foreach (Transform child in this.transform) {
			#if UNITY_EDITOR
				Object.DestroyImmediate(child.gameObject, true);
			#else
				Destroy(child.gameObject);
			#endif
				
		}
	}

	public void GenerateGrid() {

		var width = config.gridNumberX * (config.size + config.borderSize) + config.borderSize;
		var height = config.gridNumberZ * (config.size + config.borderSize) + config.borderSize;

		baseScale = new Vector3(
			width,
			height,
			1f
		);
		this.transform.localScale = baseScale;

		config.quadPrefab.transform.localScale = new Vector3(
			config.size,
			config.size,
			1f
		);

		players = new GameObject[playersNumber];

		for (var x = 0; x <= config.gridNumberX; x++) {
			Vector3 pos = new Vector3(
				x * (config.size + config.borderSize) + config.borderSize / 2 - width / 2,
				0.0001f,
				0
			) + transform.position;
			var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			quad.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
			quad.transform.position = pos;
			quad.transform.localScale = new Vector3(config.borderSize, height, 1f);
			quad.transform.parent = this.transform;
            quad.tag = "Border";
            var rend = quad.GetComponent<Renderer>();
			rend.material = config.borderMaterial;
		}
		for (var z = 0; z <= config.gridNumberZ; z++) {
			Vector3 pos = new Vector3(
				0,
				0.0001f,
				z * (config.size + config.borderSize) + config.borderSize / 2 - height / 2
			) + transform.position;
			var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			quad.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
			quad.transform.position = pos;
			quad.transform.localScale = new Vector3(width, config.borderSize, 1f);
			quad.transform.parent = this.transform;
            quad.tag = "Border";
			var rend = quad.GetComponent<Renderer>();
			rend.material = config.borderMaterial;
		}

		var quadLength = config.gridNumberX * config.gridNumberZ;
		quadTransforms = new Transform[quadLength];
		//quadPositions = new Vector3[quadLength];

		var i = 0;
		for (var z = 0; z < config.gridNumberZ; z++) {
			for (var x = 0; x < config.gridNumberX; x++) {
				Vector3 pos = new Vector3(
					x * (config.size + config.borderSize) + config.size / 2 + config.borderSize - width / 2,
					0.0001f,
					z * (config.size + config.borderSize) + config.size / 2 + config.borderSize - height / 2
				) + transform.position;
				var quad = Instantiate(config.quadPrefab, pos, Quaternion.AngleAxis(90, Vector3.right));
				quad.transform.parent = this.transform;

				//quadPositions[i] = new Vector3(pos.x, pos.y, pos.z);
				quadTransforms[i] = quad.transform;
				i ++;

				if (playerTypes[0] == PlayerTypes.VIRTUAL && x == 0 && z == 0) {
					players[0] = Instantiate(config.robotPrefabs[0], pos, Quaternion.AngleAxis(90, Vector3.up));
					players[0].name = "Player 1";
					RobotController rc = players[0].GetComponent<RobotController>();
					rc.SetRowCol(x, z);
					rc.SetDirection(Vector3.right);
					rc.CurrentQuad = quad;
					players[0].transform.localScale *= 2.5f * config.size;
					players[0].transform.parent = this.transform;
					players[0].GetComponent<PlayerBehaviour>().Index = 0;
					//robotController = robotPrefabs.GetComponent<RobotController>();
				}
				if (playersNumber > 1 &&
					// playerTypes[1] == PlayerTypes.VIRTUAL &&
					x == config.gridNumberX - 1 && z == config.gridNumberZ - 1
				) {
					players[1] = Instantiate(config.robotPrefabs[1], pos, Quaternion.AngleAxis(-90, Vector3.up));
					players[1].name = "Player 2";
					RobotController rc = players[1].GetComponent<RobotController>();
					rc.SetRowCol(x, z);
					rc.SetDirection(Vector3.left);
					rc.CurrentQuad = quad;
					players[1].transform.localScale *= 2.5f * config.size;
					players[1].transform.parent = this.transform;
					players[1].GetComponent<PlayerBehaviour>().Index = 1;
					//robotController = robotPrefabs.GetComponent<RobotController>();

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

		foreach (var pl in players) {
			pl.GetComponent<RobotController>().AfterInit();
		}

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

		foreach (Transform child in this.transform) {
			child.gameObject.layer = 9;
		}
		
		//@tmp
		transform.parent.localScale *= 4;

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

	// Use this for initialization
	void Start () {

		movableBehaviour = GetComponent<GridMovableBehaviour>();
		switchQuadStateBehaviour = GetComponent<SwitchQuadStateBehaviour>();

		gameType = GameTypes.SNAKE;
		/*
		GenerateGrid();
		ClearGrid();
		config.gridNumberX = 7;
		config.gridNumberZ = 7;
		// */

		if (playersNumber > 2) {
			Debug.LogError("Max 2 players");
		}

		for (var i = 0; i < 9; i++)
			ClearGrid();
		GenerateGrid();
		inPause = false;
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
		CurrentRobotController.CurrentQuad.GetComponent<QuadBehaviour>().SetState(QuadStates.ON);
		playerTurn = (playerTurn + 1) % playersNumber;
		CurrentRobotController.CurrentQuad.GetComponent<QuadBehaviour>().SetState(QuadStates.ACTIVE);

		// @todo {
		if (playerTypes[playerTurn] == PlayerTypes.VIRTUAL) {
			UIControlls.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
		}
		else if (playerTypes[playerTurn] == PlayerTypes.REAL) {
			UIControlls.GetComponent<RectTransform>().offsetMin = new Vector2(1000, 1000);
		}
		// }
	}

	// Update is called once per frame
	void Update () {
		
	}
}
