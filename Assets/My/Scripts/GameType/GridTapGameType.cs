using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class GridTapGameType : BaseGridRobyGameType {

    public override void InitBody() {

        grid.gameType = GameTypes.TAP;
        grid.playersNumber = 0;

		grid.GetComponent<SwitchQuadStateBehaviour>().enabled = true;

        grid.Init();
    }

}