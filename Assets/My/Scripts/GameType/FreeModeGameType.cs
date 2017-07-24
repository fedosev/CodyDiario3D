using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class FreeModeGameType : BaseGridRobyGameType {

    public bool trace = false;

    public override void Init() {

		grid.gameType = GameTypes.SNAKE;
		grid.playersNumber = 2;

		grid.Init();
    }

}