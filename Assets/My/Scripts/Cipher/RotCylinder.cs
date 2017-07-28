using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;



/* @todo {
public class AvgValue<T> {

	T[]	values;
	T zeroValue;
	int size;
	int index = 0;

	public delegate T sum(T a, T b);
	sum sumFunction;
	public delegate T div(T a, int b);
	div divFunction;

	public AvgValue(int size, T zeroValue, sum sumFunction, div divFunction) {
		this.size = size;
		this.zeroValue = zeroValue;
		this.sumFunction = sumFunction;
		this.divFunction = divFunction;
		values = new T[size];
		for (var i = 0; i < size; i++) {
			values[i] = zeroValue;
		}
	}

	public void Add(T value) {
		values[index] = value;
		index = (index + 1) % size;
	}

	public T Get() {
		T sum = zeroValue;
		for (var i = 0; i < size; i++) {
			sum = sumFunction(sum, values[i]);
		}
		
		return divFunction(sum, size);
	}
}
} */

public class RotCylinder : MonoBehaviour {

	public bool withSpace = false;
	public GameObject charPrefab;

	public bool isFixed = false;
	public RotCylinder mainRotCylinder;

	public float minAngularSpeed = 0.03f;

	int nChars;
	bool isTouching = false;
	RaycastHit lastHit;
	float lastRotationAngle = 0f;
	int rotNumber = 0;

	public int RotNumber { get {
		return rotNumber;
	}}

	/*
	float avgRotationAngle = new AvgValue<float>(3, 0f,
		((float a, float b) =>  (float)(a + b)),
		((float a, int b) => (float)(a / b))
	);
	*/

	Rigidbody rb;
	
	const int nFramesAvgMomentum = 3;
	int index = 0;
	float[] rotationAngles = new float[nFramesAvgMomentum];
	bool didTouch = false;

	RotCylinder[] rotCylinders;
	List<Rigidbody> rotCylindersRB = new List<Rigidbody>();

	public UnityEvent onRotNumberChange;

	public void GenerateChars() {

		// Clear
		while (this.transform.childCount > 0) { // ???
			foreach (Transform child in this.transform) {
				#if UNITY_EDITOR
					Object.DestroyImmediate(child.gameObject, true);
				#else
					Destroy(child.gameObject);
				#endif
			}
		}

		// Generate
		var n = 26 + (withSpace ? 1 : 0);
		string charStr;
		GameObject charGameObj;
		for (var i = 0; i < n; i++) {
			charGameObj = Instantiate(/*config.RotCylinderCharPrefab*/charPrefab, transform.position, Quaternion.Euler(-(360f * i / n), 0, 0));
			charGameObj.transform.parent = transform;
			charGameObj.transform.localScale = new Vector3(20f, 1f, 1f);
			if (i < 26) {
				charStr = ((char)(65 + i)).ToString();
			} else {
				charStr = " ";
			}
			charGameObj.GetComponentInChildren<TextMeshPro>().text = charStr;
			charGameObj.name = "Char(" + charStr + ")";
		}
		
	}

	void Awake() {

		nChars = 26 + (withSpace ? 1 : 0);
		rb = GetComponent<Rigidbody>();

		for (var i = 0; i < nFramesAvgMomentum; i++) {
			rotationAngles[i] = 0f;
		}

		if (onRotNumberChange == null) {
			onRotNumberChange = new UnityEvent();
		}
		
	}

	// Use this for initialization
	void Start () {

		lastHit.point = Vector3.zero;
		rotCylinders = GameObject.FindObjectsOfType<RotCylinder>();
		for (var i = 0; i < rotCylinders.Length; i++) {
			rotCylindersRB.Add(rotCylinders[i].gameObject.GetComponent<Rigidbody>());
		}
	}
	
	// Update is called once per frame
	void Update () {

		// @tmp
		if (mainRotCylinder != null) {
			if (Input.GetKey(KeyCode.Alpha1)) {
				SetRotNumber(1);
			}
			if (Input.GetKey(KeyCode.Alpha2)) {
				SetRotNumber(2);
			}
			if (Input.GetKey(KeyCode.Alpha3)) {
				SetRotNumber(3);
			}
		}

		if (!isFixed && mainRotCylinder != null && (isTouching || rb.angularVelocity != Vector3.zero)) {
			var rotNum = GetRotNumber();
			if (rotNum != rotNumber) {

				//print(rotNum);
				rotNumber = rotNum;
				
				if (onRotNumberChange != null)
					onRotNumberChange.Invoke();

				//print("" + mainRotCylinder.transform.rotation.eulerAngles + " - " + transform.rotation.eulerAngles);
				//print("" + mainRotCylinder.transform.localRotation.eulerAngles + " - " + transform.localRotation.eulerAngles);
			}
		}
		
	}

	public void SetRotNumber(int rotNumber) {

		foreach (var rotCylRB in rotCylindersRB) {
			rotCylRB.angularVelocity = Vector3.zero;
		}
		transform.rotation = Quaternion.Euler(
			mainRotCylinder.transform.rotation.eulerAngles.x + 360f * rotNumber / nChars, 0f, 90f
		);
		this.rotNumber = rotNumber;

		if (onRotNumberChange != null)
			onRotNumberChange.Invoke();
	}

	public int GetRotNumber() {

		var mainRot = mainRotCylinder.transform.rotation;
		var myRot = transform.rotation;

		/*
		float mainAngle = mainRot.eulerAngles.x;
		float myAngle = myRot.eulerAngles.x;
		// */
		//print("" + mainAngle + " - " + myAngle);
		// /*
		/*
		if (mainAngle < 0f)
			mainAngle = 360f - mainAngle;
		if (myAngle < 0f)
			myAngle = 360f - myAngle;
		// */

		var angle = Quaternion.Angle(mainRot, myRot);
		if (Vector3.Cross(mainRotCylinder.transform.forward, transform.forward).x < 0f) {
			angle = 360f - angle;
		}

		// */
		//return -(int)((mainAngle - myAngle - 180f / nChars) * nChars / 360f) % nChars;
		return (int)((angle + Mathf.Sign(angle) * (180f / nChars)) * nChars / 360f) % nChars;

	}

    void FixedUpdate() {

		if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Quaternion rot = Quaternion.identity;
			if (Physics.Raycast(ray, out hit, 100)) {
				if (hit.transform.gameObject == gameObject) {
					isTouching = true;
					if (isFixed) {
						//List<RotCylinder> rotCylinders = new List<RotCylinder>();
						foreach (var rotCylRB in rotCylindersRB) {
							rotCylRB.angularVelocity = Vector3.zero;
						}
					} else {
						rb.angularVelocity = Vector3.zero;
					}
					if (lastHit.point != Vector3.zero) {
						rot = Quaternion.FromToRotation(lastHit.point, hit.point);
						didTouch = true;
					}
					lastHit = hit;
				}
			}
			else {
				isTouching = false;
			}
			lastRotationAngle = rot.eulerAngles.x;
			lastRotationAngle = (lastRotationAngle > 180) ? lastRotationAngle - 360 : lastRotationAngle;
			rotationAngles[index] = lastRotationAngle;
			rot = Quaternion.Euler(0, -lastRotationAngle, 0);
			if (isFixed) {
				//List<RotCylinder> rotCylinders = new List<RotCylinder>();
				foreach (var rotCyl in rotCylinders) {
					rotCyl.gameObject.transform.rotation *= rot;
				}
			} else {
				transform.rotation *= rot;
			}
			index = (index + 1) % nFramesAvgMomentum;
			
		}
		if (Input.GetMouseButtonUp(0) && isTouching || !isTouching && didTouch) {
			var sumRotationAngles = 0f;
			for (var i = 0; i < nFramesAvgMomentum; i++) {
				sumRotationAngles += rotationAngles[i];
				rotationAngles[i] = 0;
			}
			index = 0;
			didTouch = false;
			lastHit.point = Vector3.zero;
			var angularVelocity = rb.angularVelocity = new Vector3(sumRotationAngles / nFramesAvgMomentum * Mathf.Deg2Rad / Time.deltaTime, 0, 0);
			if (isFixed) {
				//List<RotCylinder> rotCylinders = new List<RotCylinder>();
				foreach (var rotCylRB in rotCylindersRB) {
					rotCylRB.angularVelocity = angularVelocity;
				}
			} else {
				rb.angularVelocity = angularVelocity;
			}
			
			rb.angularVelocity = new Vector3(sumRotationAngles / nFramesAvgMomentum * Mathf.Deg2Rad / Time.deltaTime, 0, 0);
			isTouching = false;
		}

		if (rb.angularVelocity != Vector3.zero && Mathf.Abs(rb.angularVelocity.x) < minAngularSpeed) {
			rb.angularVelocity = Vector3.zero;
		}
	}
}
