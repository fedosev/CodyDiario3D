using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : GridRobyManager {

	public override void AfterInit() {
        var droneLookAt = FindObjectOfType<LookAt>();
        if (droneLookAt != null) {
            droneLookAt.target = grid.GetQuad(2, 2).transform;
        }
    }

}
