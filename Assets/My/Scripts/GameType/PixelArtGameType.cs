using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PixelArtGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Pixel art";
	} }

	public override string generalInfo { get {
		return 
            (!startPosition.IsSet() ?
                "Seleziona la posizione di partenza facendo il tap sulla casella desiderata e poi sulla direzione.\n" : "")
            + "Puoi far muovere il robot sulla scacchiera usando la carte. Per ogni casella del disegno che riuscirai a colorare avrai 1 punto, "
			+ "ma fai attenzione: non puoi portare Roby fuori dal disegno e se torni su una casella già colorata perdi 1 punto.";
	} }


	public string[] drawing;

	public bool usePath = false;

	int drawingSize;


    public override void InitBody() {

        grid.gameType = usePath ? GameTypes.ART : GameTypes.FREE;
        grid.playersNumber = 1;

		grid.OnLose += Lose;
		grid.OnNextTurn += CheckWin;

        grid.Init();

		drawingSize = 0;
		foreach (var str in drawing) {
			foreach (var chr in str) {
				if (chr != '0')
					drawingSize++;
			}
		}

		if (usePath) {
			gridRobyManager.codingGrid.gameObject.SetActive(true);
		}
    }

    public override void OnInitRobot(RobotController robot, QuadBehaviour quad) {
		base.OnInitRobot(robot, quad);
        robot.score = 1;
    }

	public override void SetupQuad(QuadBehaviour quad, int col, int row) {

		base.SetupQuad(quad, col, row);

		if (drawing.Length > 0) {
			if (drawing[grid.nRows - 1 - row][col] != '0') {
				quad.SetState(QuadStates.PATH);
			}
		}
	}

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
        if (robot.isFirstMove) {
			prevQuad.player = 0;
        }
		prevQuad.SetState(QuadStates.ART);
        
        if (nextQuad.mainState == QuadStates.PATH || (drawing.Length == 0 && nextQuad.mainState != QuadStates.ART)) {
            nextQuad.SetState(QuadStates.ACTIVE);
			nextQuad.player = 0;
			grid.CurrentRobotController.score++;
		} else if (nextQuad.mainState == QuadStates.ART) {
            nextQuad.SetState(QuadStates.ACTIVE);
			grid.CurrentRobotController.score--;
        } else {
            nextQuad.SetState(QuadStates.ERROR);
            robot.DoLose();
        }
    }
	
    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return true;
    }

	public void CheckWin() {

		if (usePath) {
			gridRobyManager.WinTextAction("{ " + grid.CurrentRobotController.score + " }");
		} else if (grid.PlayerQuadCount(0) == drawingSize) {
			var score = grid.CurrentRobotController.score;
			if (score == drawingSize)
				gridRobyManager.WinTextAction("OTTIMO! { " + score + " }");
			else
				gridRobyManager.WinTextAction("{ " + score + " }");
		}
	}

}