using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CipherRotGameType : BaseGameType {

	public bool withSpace = false;
	public int rotNumber = 0;
	public bool isFixed = false;

    public override string sceneName { get {
        return "CipherRot";
    } }

	public override void InitBody() {
	
	}

}
