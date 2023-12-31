﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PathGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Percorso";
	} }

	public override string generalInfo { get {
		var str = base.generalInfo;
		var strSelPos = "Seleziona la posizione di partenza facendo il tap sulla casella desiderata e poi sulla direzione.\n";
		if (code.Length > 0 && path.Length == 0) {
			str += "La sequenza di istruzioni è nota. Devi trovare il percorso.\n";
			if (!startPosition.IsSet())
				str += strSelPos;
			str += "Seleziona le caselle della scacchiera che faranno parte del percorso e premi su \"Esegui\".\n";
		} else if (code.Length == 0 && path.Length > 0 && !ignoreCheckPath) {
			str += "Il percorso è noto. Devi trovare trovare la sequenza di istruzioni.\n";
			if (useDevBoard && MainGameManager.Instance && MainGameManager.Instance.useAR) {
				str += "Annerisci bene le caselle opportune del programmatore sulla pagina del diario. ";
				str += "Assicurati di trovarti in un posto ben illuminato. Stendi bene la pagina con la mano senza coprire il programmatore. ";
				str += "Con l'altra mano inquadra il programmatore. Quando compariranno i valori premi su \"Usa la sequenza\". ";
				str += "Se i valori sono stati presi male potrai correggerli nella fase successiva.\n";
			} else {
				str += "Premi sulle carte per aggiungerle alla sequenza di istruzioni e premi su \"Esegui\".\n";
			}
		} else if (code.Length == 0 && path.Length == 0) {
			if (!startPosition.IsSet())
				str += strSelPos;
			str += "Devi trovare sia il percorso che la sequenza di istruzioni.";
		}

		return str;
	} }

	// [Header("Path")]

	public string[] path;
	
	public string code;

	public bool ignoreCheckPath = false;

	[Header("Win conditions")]

	public PositionInGrid endPosition = new PositionInGrid(-1, -1);

	[HideInInspector] public CodingGrid codingGrid;

	public override void InitBody() {

		grid.gameType = GameTypes.PATH;
		grid.playersNumber = 1;

		grid.OnNextTurn += CheckWin;
		grid.OnLose += Lose;

		grid.Init();

		gridRobyManager.codingGrid.Show();
		
		
		if (code.Length > 0) {
			gridRobyManager.codingGrid.SetCards(code);
			gridRobyManager.codingGrid.DisableEdit();
			grid.canBeActiveUI = false;
			grid.SetActiveUI(false);
		}

		if (path.Length == 0 && grid.state.IsNull() && !ignoreCheckPath) {
			grid.state.GoToState<StateSwitchQuad>();
		}
	}

	public override void SetupQuad(QuadBehaviour quad, int col, int row) {

		base.SetupQuad(quad, col, row);

		if (path.Length > 0) {
			//MyDebug.Log(grid.nRows);
			if (path[grid.nRows - 1 - row][col] != '0') {
				quad.SetState(QuadStates.PATH);
			}
		}
	}

	public override void OnInitRobot(RobotController robot, QuadBehaviour quad) {
		base.OnInitRobot(robot, quad);
	}
    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
        if (robot.isFirstMove && !useFirstQuad) {
            prevQuad.SetOtherState(QuadStates.DEFAULT);
        } else {
            //prevQuad.SetState(QuadStates.OBSTACLE);
            prevQuad.SetState(QuadStates.PATH);
			prevQuad.player = 0;
        }
        
        if (nextQuad.IsFreeToGoIn()) {
            nextQuad.SetState(QuadStates.ACTIVE);
			nextQuad.player = 0;
            //robot.sounds.playSound(sounds.soundStep);
        } else {
            nextQuad.SetState(QuadStates.ERROR);
            robot.DoLose();
        }
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return ignoreCheckPath || quad.mainState == QuadStates.PATH;
    }

	public void CheckWin() {
		if (ignoreCheckPath) { // Probably is just a text
			if (withLetters) {
				var text = gridRobyManager.GetLettersText();
				if (text.Length > 0)
					gridRobyManager.WinTextAction(text);
				else
					gridRobyManager.WinAction();
			} else {
				gridRobyManager.WinAction();
			}
		} else if (grid.PlayerQuadCount(0) == grid.QuadCount((QuadBehaviour quad) => quad.mainState == QuadStates.PATH)) {
			if (!endPosition.IsNull() && !grid.GetQuadPositionInGrid(grid.CurrentRobotController.CurrentQuad).Equals(endPosition))
				gridRobyManager.LoseAction();
			else
				gridRobyManager.WinAction();
		} else {
			gridRobyManager.LoseAction();
		}
	}

}