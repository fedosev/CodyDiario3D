using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodingGrid : MonoBehaviour {

    int maxCardsNumber = 10;
    int cardsNumber = 0;
    CardTypes?[] cards;

    Grid grid;

    public void AppendCard(CardTypes type) {
        cards[cardsNumber] = type;
        cardsNumber++;
    }

    public void RemoveLastCard() {
        cards[cardsNumber] = null;
        cardsNumber--;
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
		if (Input.GetKeyDown(KeyCode.A)) {
			Execute();
		}
	}
}
