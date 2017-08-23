using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ScanOptionsGameType : BaseGameType {

	public override string title { get {
		return "Scanzione delle opzioni";
	} }

	public override string generalInfo { get {
		return "Annerisci bene le caselle desiderate. Assicurati di trovarti in un luogo ben illuminato. "
			 + "Inquadra il marker lentamente evitando di creare ombre sul marker. Quando vedrai i valori stabilizzati premi su \"Usa i valori\". "
			 + "Non preoccuparti, nel caso di errori potrai sempre cambiare le opzioni successivamente dal tuo dispositivo.";
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