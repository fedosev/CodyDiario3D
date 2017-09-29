using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : MonoBehaviour {

	public GameObject playerAvatar;
	public float baseSpeed;
	public float minimumSpeed;
	public float baseAngularSpeed;
	public float minimumAngularSpeed;
	public float scaleFactor;
	public float mainImgTargetWidth;
	public float scaleOffset;
	public int framesForAverage = 3;

	protected Animator animator;
	protected Vector3 prevPosition;
	protected Quaternion prevRotation;
	protected Vector3 direction;

	private int indexSpeed = 0;
	private int indexAngSpeed = 0;
	private float[] speeds;
	private float[] angularSpeeds;
	// Use this for initialization
	void Start () {

		//direction = playerAvatar.transform.forward;
		direction =	playerAvatar.transform.forward;
		transform.rotation = playerAvatar.transform.rotation;

		animator = playerAvatar.GetComponent<Animator>();

		speeds = new float[framesForAverage];
		angularSpeeds = new float[framesForAverage];
	}
	
	// Update is called once per frame
	void Update () {
		
		if (prevPosition != transform.position) {

			Vector3 deltaPos = transform.position - prevPosition;
			float speed = Vector3.Dot(direction, deltaPos) / playerAvatar.transform.lossyScale.y / Time.deltaTime;

			indexSpeed = (indexSpeed + 1) % framesForAverage;
			speeds[indexSpeed] = speed;
			float speedsSum = 0;
			foreach (var sp in speeds) {
				speedsSum += sp;
			}
			var avgSpeed = speedsSum / framesForAverage;

			MyDebug.Log(avgSpeed);

			if (avgSpeed > minimumSpeed)
				animator.SetFloat("Speed", speed / baseSpeed);
			else if (avgSpeed < minimumSpeed * 0.5)
				animator.SetFloat("Speed", 0);

			MyDebug.Log("Speed: " + speed);

			prevPosition = transform.position;

			playerAvatar.transform.position = new Vector3(
				transform.position.x, 0,
				transform.position.z
			);
			playerAvatar.transform.localScale = Vector3.one * (transform.localPosition.y * scaleFactor / mainImgTargetWidth - scaleOffset);

		}
		else {
			//animator.SetFloat("Speed", 0);
		}

		if (prevRotation != transform.rotation) {

			//float deltaAngle = transform.rotation.eulerAngles.y - prevRotation.eulerAngles.y;
			float deltaAngle = Mathf.Sign(Vector3.Cross(direction, transform.forward).y) * Vector3.Angle(
				Vector3.ProjectOnPlane(direction, Vector3.up),
				Vector3.ProjectOnPlane(transform.forward, Vector3.up)
			);
			float angularSpeed = deltaAngle / Time.deltaTime;
			
			//MyDebug.Log(AngularSpeed);

			//MyDebug.Log(transform.rotation.x + ", " + transform.rotation.y + ", " + transform.rotation.z + ", " + transform.rotation.w);
			//MyDebug.Log(transform.localEulerAngles.x + ", " + transform.localEulerAngles.y + ", " + transform.localEulerAngles.z) ;
			//MyDebug.Log(Mathf.Atan2(playerAvatar.transform.forward.x, direction.x));
			/*
			MyDebug.Log(Vector3.Angle(
				Vector3.ProjectOnPlane(transform.forward, Vector3.up),
				Vector3.ProjectOnPlane(direction, Vector3.up)
			));
			*/
			/*
			MyDebug.Log(
				Vector3.ProjectOnPlane(transform.forward, Vector3.up) + " - " +
				Vector3.ProjectOnPlane(direction, Vector3.up)
			);
			MyDebug.Log(direction);
			MyDebug.Log(transform.forward);
			// */
			/*
			MyDebug.Log("D time: " + Time.deltaTime);
			MyDebug.Log("AngularSpeed: " + AngularSpeed);
			MyDebug.Log("deltaAngle: " + deltaAngle);
			*/

			indexAngSpeed = (indexAngSpeed + 1) % framesForAverage;
			angularSpeeds[indexAngSpeed] = angularSpeed;
			float angSpeedsSum = 0;
			foreach (var sp in angularSpeeds) {
				angSpeedsSum += sp;
			}
			var avgAngSpeed = angSpeedsSum / framesForAverage;

			if (Mathf.Abs(avgAngSpeed) > minimumAngularSpeed)
				animator.SetFloat("AngularSpeed", angularSpeed / baseAngularSpeed);
			else if (Mathf.Abs(avgAngSpeed) < minimumAngularSpeed * 0.5 )
				animator.SetFloat("AngularSpeed", 0);
			
			prevRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);

			playerAvatar.transform.rotation = Quaternion.AngleAxis(playerAvatar.transform.rotation.eulerAngles.y + deltaAngle, Vector3.up);

			direction = transform.forward;
		}
		else {
			//animator.SetFloat("AngularSpeed", 0);
		}
		
	}
}
