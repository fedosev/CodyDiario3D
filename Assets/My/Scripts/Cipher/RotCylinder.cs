using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;



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

	[HideInInspector] public bool isPaused = false;

	int nChars { get {
		return 26 + (withSpace ? 1 : 0);
	}}

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
		#if UNITY_EDITOR
			while (this.transform.childCount > 0) { // ???
				foreach (Transform child in this.transform) {
					Object.DestroyImmediate(child.gameObject, true);
				}
			}
		#else
			foreach (Transform child in this.transform) {
				Destroy(child.gameObject);
			}
		#endif

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

		//nChars = 26 + (withSpace ? 1 : 0);
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

		if (isPaused)
			return;
		// @tmp
		/*
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
		*/

		if (!isFixed && mainRotCylinder != null && (isTouching || rb.angularVelocity != Vector3.zero)) {
			var rotNum = GetRotNumber();
			if (rotNum != rotNumber) {

				//print(rotNum);
				rotNumber = rotNum;
				
				if (onRotNumberChange != null)
					onRotNumberChange.Invoke();

				//print("" + mainRotCylinder.transform.localRotation.eulerAngles + " - " + transform.localRotation.eulerAngles);
				//print("" + mainRotCylinder.transform.localRotation.eulerAngles + " - " + transform.localRotation.eulerAngles);
			}
		}
		
	}

	public IEnumerator RotateQuaternionAnimation(Quaternion to, float duration) {
		float t0 = Time.time;
		var from = transform.rotation;
		float t;
		do {
			t = (Time.time - t0) / duration;
			transform.rotation = Quaternion.Slerp(from, to, t * (2 - t));
			yield return 0;
		} while(t < 1);

		yield return null;
	}

	public void SetRotNumber(int rotNumber, bool animate = false, bool stop = true) {

		if (stop) {
			foreach (var rotCylRB in rotCylindersRB) {
				rotCylRB.angularVelocity = Vector3.zero;
			}
		}
		var mainRotAngX = mainRotCylinder.transform.localRotation.eulerAngles.x;
		if (mainRotCylinder.transform.forward.z < 0) {
			mainRotAngX = 180f - mainRotAngX;
		}
		var angleDiff = 360f * rotNumber / nChars;
		var rotV3 = new Vector3(mainRotAngX + angleDiff, 0f, 90f);
		if (animate && this.gameObject.activeInHierarchy) {
			//Debug.Log(transform.rotation.eulerAngles);
			//Debug.Log(rotV3);
			/*
			if (Mathf.Abs(rotV3.x - transform.eulerAngles.x) > 180f) {
				rotV3.x -= (360f * 4);
			}
			// Debug.Log(rotV3);
			// */
			//transform.DORotateQuaternion(Quaternion.Euler(rotV3), 0.4f);
			//transform.DORotate(rotV3, 0.4f);
			StartCoroutine(RotateQuaternionAnimation(Quaternion.Euler(rotV3), 0.4f));
		} else {
			transform.localRotation = Quaternion.Euler(rotV3);
		}
		this.rotNumber = rotNumber;

		if (onRotNumberChange != null)
			onRotNumberChange.Invoke();
	}

	public int GetRotNumber() {

		var mainRot = mainRotCylinder.transform.localRotation;
		var myRot = transform.localRotation;

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
		
		if (isPaused)
			return;

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
						// /*
						foreach (var rotCyl in rotCylinders) {
							if (!rotCyl.isFixed) {
								//rotCyl.SetRotNumber(rotCyl.RotNumber);
								rotCyl.transform.DOKill();
							}
						}
						// */
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
					rotCyl.gameObject.transform.localRotation *= rot;
				}
			} else {
				transform.localRotation *= rot;
			}
			index = (index + 1) % nFramesAvgMomentum;
			
		}
		else if (isTouching || !isTouching && didTouch) { // Mouse UP
			var sumRotationAngles = 0f;
			for (var i = 0; i < nFramesAvgMomentum; i++) {
				sumRotationAngles += rotationAngles[i];
				rotationAngles[i] = 0;
			}
			index = 0;
			isTouching = false;
			didTouch = false;
			lastHit.point = Vector3.zero;
			var angularVelocity = new Vector3(sumRotationAngles / nFramesAvgMomentum * Mathf.Deg2Rad / Time.deltaTime, 0, 0);
			if (isFixed) {
				//List<RotCylinder> rotCylinders = new List<RotCylinder>();
				if (Mathf.Abs(angularVelocity.x) > minAngularSpeed) {
					foreach (var rotCylRB in rotCylindersRB) {
						rotCylRB.angularVelocity = angularVelocity;
					}
				} else {
					foreach (var rotCyl in rotCylinders) {
						// /*
						if (!rotCyl.isFixed)
							rotCyl.SetRotNumber(rotCyl.RotNumber, true);
						// */
					}
				}
			} else { // Is not fixed
				if (Mathf.Abs(angularVelocity.x) > minAngularSpeed) {
					rb.angularVelocity = angularVelocity;
				} else {
					SetRotNumber(rotNumber, true);
				}

			}
			
			//rb.angularVelocity = new Vector3(sumRotationAngles / nFramesAvgMomentum * Mathf.Deg2Rad / Time.deltaTime, 0, 0);
			isTouching = false;
		}

		if (rb.angularVelocity != Vector3.zero && Mathf.Abs(rb.angularVelocity.x) < minAngularSpeed) {
			rb.angularVelocity = Vector3.zero;
			if (!isFixed) {
				SetRotNumber(rotNumber, true);
			} else {
				foreach (var rotCyl in rotCylinders) {
					if (!rotCyl.isFixed)
						rotCyl.SetRotNumber(rotCyl.RotNumber, true);
				}
			}
		}
	}
}
