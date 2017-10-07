using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInHand : MonoBehaviour, IPointerClickHandler {

	public int index;
	public CardTypes type;

	float interpolationSpeed = 15f;

	HandCards parent;

	Vector3 position;


	public void OnPointerClick(PointerEventData eventData) {
		parent.HandleClick(this);
	}

	public void Init(HandCards parent, int index) {
		this.index = index;
		this.parent = parent;
		interpolationSpeed = parent.interpolationSpeed;
	}

	public void Remove() {
		Destroy(gameObject);
	}

	public void SetPosition(Vector3 pos, bool immediate = false) {
		position = pos;
		if (immediate)
			transform.position = pos;
	}
	
	void Update () {
		transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * interpolationSpeed);
	}
}

