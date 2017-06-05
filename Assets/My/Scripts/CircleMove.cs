using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMove : MonoBehaviour {

	public Vector3 center;
	public Transform container;
	public float radius = 0.1f;
	public float speed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(
			container.position.x + center.x + container.localScale.x / 4 * radius * Mathf.Cos(Time.time * speed),
			center.y,
			container.position.z + center.z + container.localScale.z / 4 * radius * Mathf.Sin(Time.time * speed)
		);
	}
}
