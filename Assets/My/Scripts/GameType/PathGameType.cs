using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PathGameType : BaseGridRobyGameType {

				public override void InitBody() {

				grid.gameType = GameTypes.PATH;
				grid.playersNumber = 1;

				grid.Init();

    }

}