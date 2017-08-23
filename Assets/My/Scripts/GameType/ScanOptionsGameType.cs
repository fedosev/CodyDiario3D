using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ScanOptionsGameType : BaseGameType {

    protected ScanOptionsManager scanOptionsManager;

    public override void BeforeInit() {
        base.BeforeInit();
        scanOptionsManager = ScanOptionsManager.Instance;
    }

    public override string sceneName { get {
        return "ScanOptions";
    } }

}