using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameObjectState : MonoBehaviour {

	public virtual void OnEnter() {}
	public virtual void OnExit() {}

	public T GetState<T>() where T : BaseGameObjectState {

		T state = GetComponent<T>();
		if (state != null) {
			return GetComponent<T>();
		} else {
			return gameObject.AddComponent<T>() as T;
		}
	}

	/*
	public virtual BaseGameObjectState GetNextState() {
		return null; // END
	}
	*/

	public T GoToState<T>() where T : BaseGameObjectState {

		var nextState = GetState<T>();
		OnExit();

		if (nextState != null) {
			nextState.enabled = true;
		}
		this.enabled = false;

		return nextState;
	}

	public T InitState<T>() where T : BaseGameObjectState {

		var state = GoToState<T>();

		this.enabled = false;
		Destroy(this);

		return state;
	}

	public virtual BaseGameObjectState NextState() {

		return GoToState<StateNull>();
	}

	void OnEnable() {
		OnEnter();
	}

	void OnDisable() {

		// @todo maybe destroy

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
