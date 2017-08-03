using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodingGrid : MonoBehaviour {

    int maxCardsNumber = 26;
    int cardsNumber = 0;
    CardTypes?[] cards;

    Grid grid;

    public void AppendCard(CardTypes type) {

        if (cardsNumber >= maxCardsNumber) 
            return;

        cards[cardsNumber] = type;
        cardsNumber++;
    }
    public void AppendCard(char cardLetter) {

        if (cardsNumber >= maxCardsNumber) 
            return;
        
        switch (cardLetter) {
            case 'A': cards[cardsNumber] = CardTypes.FORWARD; break;
            case 'S': cards[cardsNumber] = CardTypes.LEFT; break;
            case 'D': cards[cardsNumber] = CardTypes.RIGHT; break;
            default:
                Debug.LogError(cardLetter);
                Debug.LogError("Should be A, S or D");
                return;
        }
        cardsNumber++;
    }

    public void SetCards(string cardLetters) {
        for (var i = 0; i < cardLetters.Length; i++) {
            AppendCard(cardLetters[i]);
        }
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
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Execute();
		}
	}
}
