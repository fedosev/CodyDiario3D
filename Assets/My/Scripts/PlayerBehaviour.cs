using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	private int index;

	private GameObject gridGO;
	private Grid grid;

	public int Index { get {
		return index;
	} set {
		index = value;
	} }

	public bool IsMyTurn() {
		return grid.playerTurn == index;
	}
	// Use this for initialization
	void Start () {
		gridGO = GameObject.Find("Grid");
		grid = gridGO.GetComponent<Grid>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
