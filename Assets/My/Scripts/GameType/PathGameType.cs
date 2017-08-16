using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PathGameType : BaseGridRobyGameType {

	public string[] path;
	
	public string code; //@todo maybe not needed

	[HideInInspector] public CodingGrid codingGrid;

	public override void InitBody() {

		grid.gameType = GameTypes.PATH;
		grid.playersNumber = 1;

		grid.Init();

		gridRobyManager.codingGrid.gameObject.SetActive(true);
		
		
		if (code.Length > 0) {
			gridRobyManager.codingGrid.SetCards(code);
			gridRobyManager.codingGrid.DisableEdit();
		}

		if (path.Length == 0 && grid.state.IsNull()) {
			grid.state = grid.state.GoToState<StateSwitchQuad>();
		}
	}

	public override void SetupQuad(QuadBehaviour quad, int col, int row) {

		base.SetupQuad(quad, col, row);

		if (path.Length > 0) {
			if (path[grid.nRows - 1 - row][col] != '0') {
				quad.SetState(QuadStates.PATH);
			}
		}
	}

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
        if (robot.isFirstMove) {
            prevQuad.SetOtherState(QuadStates.DEFAULT);
        } else {
            //prevQuad.SetState(QuadStates.OBSTACLE);
            prevQuad.SetState(QuadStates.PATH);
        }
        
        if (nextQuad.IsFreeToGoIn()) {
            nextQuad.SetState(QuadStates.ACTIVE);
            //robot.sounds.playSound(sounds.soundStep);
        } else {
            nextQuad.SetState(QuadStates.ERROR);
            robot.DoLose();
        }
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return quad.mainState == QuadStates.PATH;;
    }	

}