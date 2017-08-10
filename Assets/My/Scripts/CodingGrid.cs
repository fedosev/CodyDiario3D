﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodingGrid : MonoBehaviour {

    public GameObject controlsUI;

    //bool isControlsUIVisible = false;

    int maxCardsNumber = 25;
    int cardsNumber = 0;
    CardTypes?[] cards;

    Grid grid;

    public char GetLetterFromType(CardTypes cardType) {
        switch (cardType) {
            case CardTypes.FORWARD: return 'A';
            case CardTypes.LEFT: return 'S';
            case CardTypes.RIGHT: return 'D';
        }
        return 'X';
    }

    public CardTypes GetTypeFromLetter(char cardLetter) {
        switch (cardLetter) {
            case 'A': return CardTypes.FORWARD;
            case 'S': return CardTypes.LEFT;
            case 'D': return CardTypes.RIGHT;
            default:
                Debug.LogError(cardLetter);
                Debug.LogError("Should be A, S or D");
                return CardTypes.FORWARD;
        }
    }

    public void AppendCard(CardTypes type) {

        if (cardsNumber >= maxCardsNumber) 
            return;

        cards[cardsNumber] = type;
        cardsNumber++;

        GridRobyManager.Instance.AppendLetter(GetLetterFromType(type));
    }
    public void AppendCard(char cardLetter) {

        if (cardsNumber >= maxCardsNumber) 
            return;

        cards[cardsNumber] = GetTypeFromLetter(cardLetter);
        cardsNumber++;

        GridRobyManager.Instance.AppendLetter(cardLetter);
    }

    public void SetCards(string cardLetters) {
        for (var i = 0; i < cardLetters.Length; i++) {
            AppendCard(cardLetters[i]);
        }
    }    

    public void RemoveLastCard() {
        if (cardsNumber > 0) {
            cards[cardsNumber - 1] = null;
            cardsNumber--;
            GridRobyManager.Instance.RemoveLastLetter();
        }
    }

    public void Execute() {
        if (grid.inPause)
            return;
        for (var i = 0; i < maxCardsNumber && cards[i] != null; i++) {
            grid.AddAction(cards[i]);
            cards[i] = null;
        }
        cardsNumber = 0;
        grid.NextAction();
    }

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        cards = new CardTypes?[maxCardsNumber];
    }
	
	// Update is called once per frame
	void Update () {

        if (cardsNumber > 0 && !controlsUI.activeSelf) {
            controlsUI.SetActive(true);
        } else if (cardsNumber == 0 && controlsUI.activeSelf) {
            controlsUI.SetActive(false);
        }
        /* 
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Execute();
		}
        // */
	}
}
