using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardTypes { LEFT, FORWARD, RIGHT };

public class Card : MonoBehaviour, IPointerClickHandler
{
    public CardTypes cardType;

    CodingGrid codingGrid;
    Grid grid;

    void Awake() {

        grid = GameObject.FindObjectOfType<Grid>();
        codingGrid = GameObject.FindObjectOfType<CodingGrid>();
    }

    public void OnPointerClick(PointerEventData eventData) {

        if (grid.inPause)
            return;

        if (grid.gameType != GameTypes.PATH) {
            if (grid.CurrentRobotController == null)
                return;
                
            switch (cardType) {
                case CardTypes.LEFT:
                    grid.CurrentRobotController.TurnLeft();
                    break;
                case CardTypes.FORWARD:
                    grid.CurrentRobotController.MoveForward();
                    break;
                case CardTypes.RIGHT:
                    grid.CurrentRobotController.TurnRight();
                    break;
            }
        }
        else { // if (grid.gameType == GameTypes.PATH)
            codingGrid.AppendCard(cardType);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
