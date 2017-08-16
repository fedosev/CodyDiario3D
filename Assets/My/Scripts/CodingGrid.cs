using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodingGrid : MonoBehaviour {

    public GameObject controlsUI;

    //bool isControlsUIVisible = false;

    const int maxCardsNumber = 25;
    int cardsNumber = 0;
    CardTypes?[] cards;

    Grid grid;

    public Text text;

    public GameObject removeButton;
    public GameObject panelCards;

    public char GetLetterFromType(CardTypes cardType) {
        switch (cardType) {
            case CardTypes.FORWARD: return 'A';
            case CardTypes.LEFT: return 'S';
            case CardTypes.RIGHT: return 'D';
        }
        return 'X';
    }

    public string WrapWithColor(char cardLetter) {
        switch (cardLetter) {
            case 'A': return "<color=#00ff00ff>A</color>";
            case 'S': return "<color=#ffff00ff>S</color>";
            case 'D': return "<color=#ff0000ff>D</color>";
        }
        return cardLetter.ToString();
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

    public void Clear() {
        cards = new CardTypes?[maxCardsNumber];
        text.text = "";
    }

    public void AppendCard(CardTypes type) {

        if (cardsNumber >= maxCardsNumber) 
            return;

        cards[cardsNumber] = type;
        cardsNumber++;

        text.text += (WrapWithColor(GetLetterFromType(type)));
    }
    public void AppendCard(char cardLetter) {

        if (cardsNumber >= maxCardsNumber) 
            return;

        cards[cardsNumber] = GetTypeFromLetter(cardLetter);
        cardsNumber++;

        text.text += (WrapWithColor(cardLetter));
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
            text.text = text.text.Substring(0, text.text.Length - 26);
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

    public void DisableEdit(bool disable = true) {
        removeButton.SetActive(!disable);
        panelCards.SetActive(!disable);
    }

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        Clear();
    }
	
	// Update is called once per frame
	void Update () {

        // /*
        if (cardsNumber > 0 && !controlsUI.activeSelf) {
            controlsUI.SetActive(true);
        } else if (cardsNumber == 0 && controlsUI.activeSelf) {
            controlsUI.SetActive(false);
        }
        // */
        /* 
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Execute();
		}
        // */
	}
}
