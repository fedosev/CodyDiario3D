using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ConquestGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Conquista";
	} }

	public override string generalInfo { get {
		var str = "@todo\n";
		if (vsComputer) {
			str += "Oggi giocherai contro il computer";
				if (computerSequence.Length > 0)
					str += ", il quale userà queste istruzioni:\n" + String.Join(" ", computerSequence);
			str += ".\n";
		}
		return str;
	} }

    public string deck = "";
    public bool vsComputer = false;
    public string[] computerSequence;

    int cardsNumber = 5;
	readonly char[] letters = new char[] { 'X', 'O' };

	bool didWin;
	int nQuads;

	int computerTurn;

    public override void InitBody() {

		didWin = false;

        grid.gameType = GameTypes.CONQUEST;
        grid.playersNumber = 2;

		grid.OnNextTurn += NextTurn;
		grid.OnLose += Lose;

        grid.Init();

		nQuads = grid.nCols * grid.nRows;

		grid.SetRobotsColliderActive(true);

		grid.CurrentRobotController.score = 1;
		grid.NextRobotController.score = 1;
		grid.GetRobotController(0).CurrentQuadBh.SetLetter(letters[0]);
		grid.GetRobotController(1).CurrentQuadBh.SetLetter(letters[1]);
        
		if (!vsComputer) {
			grid.SetActiveUI(false);
			if (deck.Length > 0) {
				gridRobyManager.deck = new Deck(deck);
				// @tmp
				if (deck.Length == 6) {
					cardsNumber = 6;
				}
			} else {
				gridRobyManager.deck = new Deck("AAAAAAAAAAAAAAAAAAAAAAAASSSSSSSSDDDDDDDD"); // page 345
				//gridRobyManager.deck = new Deck("AAAAAAAAAAAAAAAAAAAAAAAASSSSSSSSDDDDDDDDAAAAAAAAAAAAAAAAAAAAAAAASSSSSSSSDDDDDDDD"); // page 345 x2
				//gridRobyManager.deck = new Deck("AAAAAAAAAAAAAAAAAAAASSSSSSSSSSDDDDDDDDDD");
				gridRobyManager.deck.Shuffle(true);
			}
			gridRobyManager.codingGrid.Show();
			gridRobyManager.codingGrid.HideUI();
			gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
			gridRobyManager.cardsSelection.Init(gridRobyManager.deck, cardsNumber, withLetters ? 0f : -100f);

		} else { // VS Computer
			computerTurn = 0;
			gridRobyManager.cardsSelection.Init(null, cardsNumber, -200f);
			var uiCards = FindObjectsOfType<Card>();
			foreach (var card in uiCards) {
				card.OnClick = HandleCardClick;
			}
			grid.SetActiveUI(false);
		}
		gridRobyManager.cardsSelection.OnUseCards += UseCards;
		gridRobyManager.OnGameTypeStart += NextTurn;
    }

	public void HandleCardClick(CardTypes type) {

		if (grid.playerTurn != 0)
			return;

		gridRobyManager.cardsSelection.AppendCard(type);
	}

    void UseCards(CardTypes[] cards) {

        for (var i = 0; i < cards.Length; i++) {
            grid.AddAction(cards[i]);
        }
        grid.NextAction();
		if (gridRobyManager.deck != null)
			gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
    }

    void NextTurn() {

		if (didWin)
			return;

		if (vsComputer) {
			if (computerTurn > computerSequence.Length) {
				gridRobyManager.WinTextAction("Il computer ha finito le azioni...");
				return;
			}
			if (grid.playerTurn == 0) {
				grid.SetActiveUIAnimated(true);

			} else { // Computer turn
				grid.SetActiveUIAnimated(false);
				foreach (var letter in computerSequence[computerTurn]) {
					grid.AddAction(CodingGrid.GetTypeFromLetter(letter));
				}
				grid.NextAction();
				computerTurn++;
			}
		} else {
			if (gridRobyManager.deck.RemainingCards() <= 0 /* @tmp: */|| cardsNumber == 6) {
				gridRobyManager.deck.Shuffle();
			}
			gridRobyManager.cardsSelection.TakeCards();
			gridRobyManager.codingGrid.text.SetText(gridRobyManager.deck.GetRichText());
		}
    }

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
		var otherRobot = grid.GetRobotController(robot.index, true);
		var letter = letters[robot.index];
		prevQuad.SetState(QuadStates.DEFAULT);

		if (nextQuad.letter != letter) {
			robot.score++;
		}
		if (nextQuad.letter == letters[(robot.index + 1) % 2]) {
			otherRobot.score--;
		}

		nextQuad.SetLetter(letter);
		nextQuad.SetState(QuadStates.ACTIVE);

		if (robot.score + otherRobot.score == nQuads) {
			didWin = true;
			grid.ClearActions();
			robot.OnStopMove += () => {
				if (robot.score == otherRobot.score) {
					gridRobyManager.WinTextAction("PARI!");
				} else {
					if (vsComputer) {
						if ((robot.score > otherRobot.score ? robot.index : otherRobot.index) == 0)
							gridRobyManager.WinTextAction("HAI VINTO!");
						else
							gridRobyManager.WinTextAction("IL COMPUTER VINCE!");
					} else {
						gridRobyManager.WinTextAction("IL GIOCATORE " +
							((robot.score > otherRobot.score ? robot.index : otherRobot.index) + 1) + " VINCE!"
						);
					}
				}
			};
		}
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return quad.otherState != QuadStates.ON;
    }

}