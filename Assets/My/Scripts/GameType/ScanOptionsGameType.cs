using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ScanOptionsGameType : BaseGameType {

	public override string title { get {
		return "Scanzione delle opzioni";
	} }

	public override string generalInfo { get {

		return "@todo...";
	} }


    protected ScanOptionsManager scanOptionsManager;

    public override void BeforeInit() {
        base.BeforeInit();
        scanOptionsManager = ScanOptionsManager.Instance;
    }

    public override void InitBody() {
        
    }

    public override string sceneName { get {
        return "ScanOptions";
    } }

}