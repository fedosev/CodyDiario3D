using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class FreeModeGameType : BaseGridRobyGameType {

	public override string title { get {
		return "Modalità libera";
	} }

	public override string generalInfo { get {
		return base.generalInfo +
            (!startPosition.IsSet() ?
                "Seleziona la posizione di partenza facendo il tap sulla casella desiderata e poi sulla direzione.\n" : "")
            + "Puoi far muovere il robot sulla scacchiera in modalità libera usando la carte.";
	} }
    

    public bool trace = false;

    public override void InitBody() {

        grid.gameType = GameTypes.FREE;
        grid.playersNumber = 1;

        grid.Init();
    }

    public override void ChangeQuad(RobotController robot, QuadBehaviour prevQuad, QuadBehaviour nextQuad) {

        if (trace) {
            if (robot.isFirstMove && !useFirstQuad)
                prevQuad.SetState(QuadStates.DEFAULT);
            else
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