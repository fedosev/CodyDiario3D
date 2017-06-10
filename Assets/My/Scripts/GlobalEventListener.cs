using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalEventListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SceneManager.LoadScene("SceneSelector", LoadSceneMode.Additive);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
			SceneManager.LoadScene("SceneSelector", LoadSceneMode.Additive);
		}
	}
}
