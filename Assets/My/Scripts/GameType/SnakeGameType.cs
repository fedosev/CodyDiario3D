using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SnakeGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Serpente";
	} }

    public override void InitBody() {

        grid.gameType = GameTypes.SNAKE;
        grid.playersNumber = 1;

        grid.Init();
    }

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {
        
        if (robot.isFirstMove) {
            prevQuad.SetOtherState(QuadStates.DEFAULT);
        } else {
            prevQuad.SetState(QuadStates.OBSTACLE);
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
        return quad.mainState != QuadStates.OBSTACLE;
    }

}