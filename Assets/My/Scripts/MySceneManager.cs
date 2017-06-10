using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {

	public string sceneName;
	public void LoadScene() {
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
