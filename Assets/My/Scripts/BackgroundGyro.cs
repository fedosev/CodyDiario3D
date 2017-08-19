using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGyro : MonoBehaviour {

	public Vector3 angRot;
	public float scaleRotation = 0.5f;
	public float speed = 0.08f;

	const int samples = 15;

	Vector3[] vals = new Vector3[samples];
	int index = 0;

	bool defaultCompensateSensors;

	void Start () {
		Input.gyro.enabled = true;
		defaultCompensateSensors = Input.compensateSensors;
		Input.compensateSensors = true;
		Input.gyro.updateInterval = 0.01f;

		transform.rotation = GyroToUnity(Quaternion.Lerp(Quaternion.identity, Input.gyro.attitude, scaleRotation));

		/*
		var acc = Input.acceleration;
		for (int i = 0; i < samples; i++) {
			vals[i] = acc;
		}
		transform.rotation = GetRotation(acc);
		//transform.rotation *= Quaternion.FromToRotation(transform.forward, acc);
		// */
	}

	Quaternion GetRotation(Vector3 dir) {
		return Quaternion.Lerp(
			Quaternion.identity,
			Quaternion.LookRotation(transformV3(dir), Vector3.Project(transform.up, Vector3.forward).normalized),
			scaleRotation
		);
	}

	Vector3 transformV3(Vector3 vec3) {
		return new Vector3(vec3.x, -vec3.y, vec3.z);
	}

    Quaternion GyroToUnity(Quaternion q) {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

	void Update () {
		// /*
		transform.rotation = Quaternion.Lerp(
			transform.rotation,
			GyroToUnity(Quaternion.Lerp(Quaternion.identity, Input.gyro.attitude, scaleRotation)),
			//GyroToUnity(Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(angRot), scaleRotation)),
			speed * Time.deltaTime
		);
		// */
		/*
		transform.rotation = Quaternion.Lerp(
			transform.rotation,
			//Quaternion.Lerp(transform.rotation, transform.rotation * GyroToUnity(Quaternion.Euler(Input.gyro.rotationRateUnbiased  * Mathf.Rad2Deg)), scaleRotation),
			Quaternion.Lerp(transform.rotation, transform.rotation * GyroToUnity(Quaternion.Euler(angSpeed * Time.deltaTime)), scaleRotation),
			speed
		);
		*/
		/*
		vals[index++] = Input.acceleration;
		Vector3 avg = Vector3.zero;
		for (int i = 0; i < samples; i++) {
			avg += vals[i];
		}
		avg /= samples;

		transform.rotation = GetRotation(avg);
		//transform.rotation *= Quaternion.FromToRotation(transform.forward, avg);


		index = (index + 1) % samples;
		*/
	}

	void OnDestroy() {
		 Input.gyro.enabled = false;
		 Input.compensateSensors = defaultCompensateSensors;
	}
}
