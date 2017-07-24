using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseGameType : ScriptableObject {

    public bool useAR = true;

    public abstract string sceneName { get; }

    public virtual void Init() {

        BeforeInit();
        InitBody();
        AfterInit();
    }

    public virtual void BeforeInit() {
        Debug.Log("BaseGameType INIT");
        Debug.Log("Scene: " + sceneName);
    }

    public abstract void InitBody();

    public virtual void AfterInit() {
        if (useAR) {
            Component.FindObjectOfType<BaseGameTypeManager>().ShowGame(false);
        }
    }

}