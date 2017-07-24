using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SnakeGameType : BaseGridRobyGameType {

    public override void InitBody() {

        grid.gameType = GameTypes.SNAKE;
        grid.playersNumber = 1;

        grid.Init();
    }

}