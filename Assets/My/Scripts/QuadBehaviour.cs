using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public enum QuadStates { DEFAULT, ON, ACTIVE, CURSOR_ON, CURSOR_ACTIVE, CURSOR_WARNING, WARNING, ERROR, OBSTACLE, PATH, ART, INFO, STAR, CIRCLE };

public class QuadBehaviour : MonoBehaviour {

	public QuadStates mainState;
	public QuadStates otherState;

	public Mesh defaultMesh;

	public Vector3 direction;

	public char letter = ' ';

	public int index;

	public int player = -1;
	public int number = -1;

	private Grid grid;
	private ConfigScriptableObject config;

	private Renderer rend;

	private MeshFilter meshFilter;
	private MeshFilter MyMeshFilter { get {
		if (!meshFilter)
			meshFilter = GetComponent<MeshFilter>();
		return meshFilter;
	} }

	[SerializeField]
	public Vector3 prevDirection;
	public QuadStates prevMainState;
	public QuadStates prevOtherState;

	public void RecordUndo() {
		prevMainState = mainState;
		prevOtherState = otherState;
		prevDirection = direction;
	}

	public void Undo() {
		SetDirection(prevDirection);
		SetMainState(prevMainState);
		SetOtherState(prevOtherState);
		//MyDebug.Log(prevDirection + ", " + prevMainState + ", " + prevOtherState);
	}

	// For material instances
	private static Dictionary<Material, Material> materials = new Dictionary<Material, Material>();

	static public Material GetMaterialForModifying(Material material) {
		Material mat;
		if (materials.TryGetValue(material, out mat)) {
			return mat;
		} else {
			var newMat = new Material(material);
			materials.Add(material, newMat);
			return materials[material];
		}
	}

	private Material GetMaterial(Material material) {
		Material mat;
		if (materials.TryGetValue(material, out mat)) {
			return mat;
		}
		return material;
	}

	void Awake() {
		grid = GridRobyManager.Instance.grid;
		config = grid.config;
		
		mainState = QuadStates.DEFAULT;
		otherState = QuadStates.DEFAULT;
		rend = GetComponent<Renderer>();
		rend.material = GetMaterial(config.quadMaterial);

		defaultMesh = MyMeshFilter.mesh;
	}

	void Start () {
		//grid = GameObject.Find("Grid").GetComponent<Grid>();
		config = grid.config;
		
		RecordUndo();
	}
	

	public void SetState(QuadStates quadState) {

		//direction = Vector3.zero;

		rend = GetComponent<Renderer>();

		#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying && quadState != QuadStates.DEFAULT) {
				return;
			}
		#endif

		switch (quadState) {
			case QuadStates.DEFAULT:
				rend.material = GetMaterial(config.quadMaterial);
				otherState = QuadStates.DEFAULT;
				mainState = quadState;
				break;
			case QuadStates.ACTIVE:
			case QuadStates.CURSOR_ACTIVE:
				rend.material = config.quadActiveMaterial;
				otherState = quadState;
				//StartCoroutine(AnimateLetter());
				break;
			case QuadStates.ON:
			case QuadStates.CURSOR_ON:
				rend.material = config.quadOnMaterial;
				otherState = quadState;
				break;
			case QuadStates.WARNING:
			case QuadStates.CURSOR_WARNING:
				rend.material = config.quadWarningMaterial;
				otherState = quadState;
				break;
			case QuadStates.ERROR:
				rend.material = config.quadErrorMaterial;
				otherState = quadState;
				break;
			case QuadStates.OBSTACLE:
				rend.material = config.obstacleMaterial;
				otherState = QuadStates.DEFAULT;
				mainState = quadState;
				break;
			case QuadStates.PATH:
				rend.material = config.quadPathMaterial;
				otherState = QuadStates.DEFAULT;
				mainState = quadState;
				break;
			case QuadStates.ART:
				rend.material = config.quadActiveMaterial;
				otherState = QuadStates.DEFAULT;
				mainState = quadState;
				break;
			case QuadStates.INFO:
				rend.material = config.quadInfoMaterial;
				otherState = QuadStates.DEFAULT;
				mainState = QuadStates.INFO;
				break;
			case QuadStates.STAR:
				rend.material = config.quadStarMaterial;
				otherState = QuadStates.DEFAULT;
				mainState = quadState;
				break;
			case QuadStates.CIRCLE:
				rend.material = config.quadCircleMaterial;
				otherState = QuadStates.DEFAULT;
				mainState = quadState;
				break;
		}

		//grid.QuadStateChanged(this);
	}
	
	public void SetMainState(QuadStates quadState) {
		SetState(quadState);
	}

	public void SetOtherState(QuadStates quadState) {
		
		if (quadState == QuadStates.DEFAULT)
			SetState(mainState);
		else
			SetState(quadState);
	}

	public bool IsFreeToGoIn() {

		if (otherState == QuadStates.ACTIVE || otherState == QuadStates.ON)
			return false;

		return grid.gameTypeConfig.QuadIsFreeToGoIn(this);
	}

	public void SetDirection(Vector3 direction) {

		this.direction = direction;

		if (direction != Vector3.zero) {
			MyMeshFilter.mesh = GenerateDirectionalMesh();
			// /*
			var rotAng = new Vector3(90, 0, 0);
			// */
			//var rot = new Quaternion();
			if (direction == Vector3.right) {
				rotAng.z = 0;
				//rot.SetLookRotation(Vector3.right, Vector3.forward);
				//transform.rotation = rot;
			} else if (direction == Vector3.forward) {
				rotAng.z = 90;
			} else if (direction == Vector3.left) {
				//rot.SetLookRotation(Vector3.left, Vector3.forward);
				//transform.rotation = rot;
				rotAng.z = 180;
			} else if (direction == Vector3.back) {
				rotAng.z = 270;
			}
			//MyDebug.Log(rotAng + " " + direction);
			transform.rotation = Quaternion.Euler(rotAng);
			//transform.Rotate(new Vector3(-90, 0, 0), Space.World);
			//transform.rotation = Quaternion.LookRotation(direction, Vector3.back);
		}
		else {
			MyMeshFilter.mesh = defaultMesh;
		}
	}

	public RobyDirection GetDirection() {

		if (direction == Vector3.forward) return RobyDirection.North;
		if (direction == Vector3.right) return RobyDirection.East;
		if (direction == Vector3.back) return RobyDirection.South;
		if (direction == Vector3.left) return RobyDirection.West;
		return RobyDirection.Null;
	}

	
	private Mesh mesh;

	private Mesh GenerateDirectionalMesh() {

		if (mesh)
			return mesh;

		mesh = new Mesh();

		var verts = new Vector3[7];

		var offset = new Vector3(0.5f, 0.5f, 0);
		verts[0] = (new Vector3(0, 0, 0) * 0.25f) - offset;
		verts[1] = (new Vector3(4, 0, 0) * 0.25f) - offset;
		verts[2] = (new Vector3(1, 1, 0) * 0.25f) - offset;
		verts[3] = (new Vector3(3, 2, 0) * 0.25f) - offset;
		verts[4] = (new Vector3(4, 4, 0) * 0.25f) - offset;
		verts[5] = (new Vector3(0, 4, 0) * 0.25f) - offset;
		verts[6] = (new Vector3(1, 3, 0) * 0.25f) - offset;

		mesh.vertices = verts;

		var tri = new int[21];

		tri[0]  = 0; tri[1]  = 2; tri[2]  = 1;
		tri[3]  = 1; tri[4]  = 2; tri[5]  = 3;
		tri[6]  = 1; tri[7]  = 3; tri[8]  = 4;
		tri[9]  = 3; tri[10] = 6; tri[11] = 4;
		tri[12] = 6; tri[13] = 5; tri[14] = 4;
		tri[15] = 0; tri[16] = 6; tri[17] = 2;
		tri[18] = 0; tri[19] = 5; tri[20] = 6;
		/*
		*/

		mesh.triangles = tri;

		var normals = new Vector3[7];

		for (var i = 0; i < 7; i++) {
			normals[i] = -Vector3.forward;
		}

		mesh.normals = normals;

		var uv = new Vector2[7];

		for (var i = 0; i < 7; i++) {
			uv[i] = new Vector2(verts[i].x, verts[i].y);
		}

		mesh.uv = uv;

		mesh.name = "CursorMesh";

		return mesh;
	}


	private GameObject CreateCursor(int n) {

		GameObject go = new GameObject();

		go.name = "Cursor " + n;

		var mf = go.AddComponent<MeshFilter>();
		//go.GetComponent<MeshFilter>().mesh = GenerateDirectionalMesh();
		mf.mesh = GenerateDirectionalMesh();

		go.AddComponent<MeshRenderer>();

		return go;
	}


	TextMeshPro text;

	public TextMeshPro GetText() {

		#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying && grid == null) {
				grid = FindObjectOfType<Grid>();
			}
		#endif		
		if (text == null) {
			text = Instantiate(grid.config.quadLetterPrefab).GetComponent<TextMeshPro>();
			text.transform.position = transform.position + new Vector3(0f, 0.0009f, 0f);
			text.transform.parent = transform;
		}
		return text;
	}

    public void SetLetter(char c) {
		letter = c;
        GetText().text = c.ToString();
    }

	public void UseLetter() {
		StartCoroutine(AnimateLetter());
	}

	public IEnumerator AnimateLetter() {

		var animTime = 0.75f;
		if (grid.gameTypeConfig.withLetters && letter != ' ') {
			var textObj = GetText().gameObject;
			var textCloneObj = Instantiate(textObj, text.transform.position, text.transform.rotation, transform);
			if (textCloneObj.transform.childCount == 1) {
				Destroy(textCloneObj.transform.GetChild(0).gameObject);
			}
			textCloneObj.transform.DOMove(Camera.main.transform.position, animTime).SetEase(Ease.InQuad);
			//textCloneObj.transform.DOMoveY(0.5f, animTime).SetEase(Ease.InExpo);
			//textCloneObj.transform.DOScale(textObj.transform.localScale * 2, animTime).SetEase(Ease.InQuad);
			textCloneObj.transform.DOLookAt(Camera.main.transform.forward, animTime).SetEase(Ease.InQuad);
			//yield return new WaitForSeconds(animTime - 0.1f);
			yield return StartCoroutine(grid.gameTypeManager.AppendLetterDelayed(letter, animTime - 0.1f));
			yield return new WaitForSeconds(0.1f);
			Destroy(textCloneObj);
		}
		yield return null;
	}
}
