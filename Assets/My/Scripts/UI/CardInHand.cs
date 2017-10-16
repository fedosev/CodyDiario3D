using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInHand : MonoBehaviour, IPointerClickHandler {

	public int index;
	public CardTypes type;
	[HideInInspector] public bool isSelected = false;

	float interpolationSpeed = 15f;

	bool useLocalPosition = false;

	ICardsContainer parent;

	Vector3 position;


	public void OnPointerClick(PointerEventData eventData) {
		parent.HandleClick(this);
	}

	public void Init(ICardsContainer parent, int index, bool useLocalPosition = false) {
		this.index = index;
		this.parent = parent;
		interpolationSpeed = parent.GetInterpolationSpeed();
		this.useLocalPosition = useLocalPosition;
	}

	public void Remove() {
		Destroy(gameObject);
	}

	public void SetPosition(Vector3 pos, bool immediate = false) {
		position = pos;
		if (immediate) {
		if (useLocalPosition)
			transform.localPosition = pos;
		else
			transform.position = pos;
		}
	}
	
	void Update () {
		if (useLocalPosition)
			transform.localPosition = Vector3.Lerp(transform.position, position, Time.deltaTime * interpolationSpeed);
		else
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * interpolationSpeed);
	}
}

