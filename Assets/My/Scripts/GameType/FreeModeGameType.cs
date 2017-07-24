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

}