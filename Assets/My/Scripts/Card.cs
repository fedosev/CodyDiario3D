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
            RobotActionFromCard(grid.CurrentRobotController, cardType);
        }
        else { // if (grid.gameType == GameTypes.PATH)
            codingGrid.AppendCard(cardType);
        }
    }

    public static void RobotActionFromCard(RobotController robot, CardTypes cardType) {
        switch (cardType) {
            case CardTypes.LEFT:
                robot.TurnLeft();
                break;
            case CardTypes.FORWARD:
                robot.MoveForward();
                break;
            case CardTypes.RIGHT:
                robot.TurnRight();
                break;
        }
        
    }

}
