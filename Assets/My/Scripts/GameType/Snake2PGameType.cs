using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Snake2PGameType : SnakeGameType {

    public override void InitBody() {

        grid.gameType = GameTypes.SNAKE;
        grid.playersNumber = 2;

        grid.Init();
    }

}