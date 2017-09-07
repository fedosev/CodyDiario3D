#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Screenshot : MonoBehaviour {

	
	void Update () {
		if (Input.GetKey(KeyCode.Space)) {
			ScreenCapture.CaptureScreenshot("screenshots/screen-" + System.DateTime.Now.Ticks + ".png", 2);
		}
	}
}

#endif
