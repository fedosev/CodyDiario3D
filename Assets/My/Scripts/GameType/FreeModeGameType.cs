using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class FreeModeGameType : BaseGridRobyGameType {

    public bool trace = false;

    public override void InitBody() {

        grid.gameType = GameTypes.FREE;
        grid.playersNumber = 1;

        grid.Init();
    }

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {

        if (trace) {
            if (!robot.isFirstMove)
                prevQuad.SetState(QuadStates.PATH);
        } else {
            prevQuad.SetState(QuadStates.DEFAULT);
        }
        nextQuad.SetState(QuadStates.ACTIVE);
    }

    public override bool QuadIsFreeToGoIn(QuadBehaviour quad) {
        return true;
    }

}