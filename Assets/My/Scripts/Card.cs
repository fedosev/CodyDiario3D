using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardTypes { LEFT, FORWARD, RIGHT };

public class Card : MonoBehaviour, IPointerClickHandler {

    public CardTypes cardType;

    public Action<CardTypes> OnClick;

    CodingGrid codingGrid;
    Grid grid;


    void Awake() {

        grid = GameObject.FindObjectOfType<Grid>();
        codingGrid = GameObject.FindObjectOfType<CodingGrid>();
    }

    public void OnPointerClick(PointerEventData eventData) {

        if (grid.inPause)
            return;
            
        if (OnClick != null) {
            OnClick(cardType);
            return;
        }

        if (grid.gameType == GameTypes.PATH ||
            grid.gameType == GameTypes.AUTO ||
            grid.gameType == GameTypes.ART
        ) {
            codingGrid.AppendCard(cardType);
        }
        else {
            if (grid.CurrentRobotController == null)
                return;
            RobotActionFromCard(grid.CurrentRobotController, cardType);
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


    #if UNITY_EDITOR
        void Update() {
            
            if (cardType == CardTypes.LEFT && Input.GetKeyDown(KeyCode.S) ||
                cardType == CardTypes.FORWARD && Input.GetKeyDown(KeyCode.A) ||
                cardType == CardTypes.RIGHT && Input.GetKeyDown(KeyCode.D)
            ) {
                OnPointerClick(new PointerEventData(EventSystem.current));
            }
        }
    #endif

}
