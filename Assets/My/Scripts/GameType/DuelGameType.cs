using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class DuelGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Duello";
	} }

	public override string generalInfo { get {
		return "@todo";
	} }

    public bool onePlayer = false;
    public string deck = "";

    const int cardsNumber = 5;

	bool didWin;

    public override void InitBody() {

		didWin = false;

        grid.gameType = GameTypes.DUEL;
        grid.playersNumber = onePlayer ? 1 : 2;

		grid.OnNextTurn += NextTurn;
		grid.OnLose += Lose;

        grid.Init();

		grid.SetActiveUI(false);
		grid.SetRobotsColliderActive(true);

        
        if (deck.Length > 0) {
            gridRobyManager.deck = new Deck(deck);
		} else {
			//gridRobyManager.deck = new Deck("ADASAADSADAASDAADAAASADAAAADSSAASAADSAAA");
			gridRobyManager.deck = new Deck("AAAAAAAAAAAAAAAAAAAAAAAASSSSSSSSDDDDDDDD");
			gridRobyManager.deck.Shuffle();
		}
		gridRobyManager.codingGrid.Show();
		gridRobyManager.codingGrid.HideUI();
		gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());

		gridRobyManager.cardsSelection.Init(gridRobyManager.deck, cardsNumber, withLetters ? 0f : -100f);
		gridRobyManager.cardsSelection.OnUseCards += UseCards;
		gridRobyManager.OnGameTypeStart += NextTurn;
		/*
		gridRobyManager.cardsSelection.TakeCards();
		gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
		// */
		if (!onePlayer)
			gridRobyManager.InitLettersTextLine(1);
    }

    void UseCards(CardTypes[] cards) {

        for (var i = 0; i < cards.Length; i++) {
            grid.AddAction(cards[i]);
        }
        grid.NextAction();
		gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
    }

    void NextTurn() {
		if (didWin)
			return;

        if (gridRobyManager.deck.RemainingCards() <= 0) {
            gridRobyManager.WinTextAction("Finite le carte...");
        } else {
			gridRobyManager.cardsSelection.TakeCards();
			gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
			/*
			if (gridRobyManager.GetLettersText(grid.GetNextPlayerTurn()).Length > 0)
				gridRobyManager.AppendLetter(' ', false, grid.GetNextPlayerTurn());
			*/
		}
    }

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
		prevQuad.SetOtherState(QuadStates.DEFAULT);

		if (nextQuad.otherState == QuadStates.ON) {
            nextQuad.SetState(QuadStates.ACTIVE);
			didWin = true;
			grid.ClearActions();
			robot.OnStopMove += () => {
				gridRobyManager.WinTextAction("IL GIOCATORE " + (robot.index + 1) + " VINCE!");
			};
		} else if (nextQuad.IsFreeToGoIn()) {
            nextQuad.SetState(QuadStates.ACTIVE);
        } else {
            nextQuad.SetState(QuadStates.ERROR);
            robot.DoLose();
        }
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return true;
    }

}