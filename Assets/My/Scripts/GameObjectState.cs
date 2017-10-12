using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjectState : MonoBehaviour {

	Action<GameObjectState> StateChanger;
	
	public static GameObjectState Init(GameObject gameObj, Action<GameObjectState> stateChanger) {

		var state = gameObj.GetComponent<StateNull>();
		if (state == null) {
			state = gameObj.AddComponent<StateNull>();
		}
		state.Setup(stateChanger);
		return state;
	}

	void Setup(Action<GameObjectState> stateChanger) {
		StateChanger = stateChanger;
	}

	public virtual void OnEnter() {}
	public virtual void OnExit() {}

	public virtual bool IsNull() {
		return false;
	}

	public T GetState<T>() where T : GameObjectState {

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

	public T GoToState<T>() where T : GameObjectState {

		var nextState = GetState<T>();
		OnExit();

		if (nextState != null) {
			nextState.enabled = true;
		}
		this.enabled = false;

		StateChanger(nextState);
		nextState.Setup(StateChanger);
		return nextState;
	}

	public T InitState<T>() where T : GameObjectState {

		var state = GoToState<T>();

		this.enabled = false;
		Destroy(this);

		StateChanger(state);
		state.Setup(StateChanger);
		return state;
	}

	public virtual GameObjectState NextState() {

		return GoToState<StateNull>();
	}

	void OnEnable() {
		OnEnter();
	}

	void OnDisable() {

		// @todo maybe destroy

	}

}
