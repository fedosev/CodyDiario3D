using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseGameType : ScriptableObject {

    public bool useAR = true;

    public abstract string sceneName { get; }

    public virtual void Init() {
        Debug.Log("BaseGameType init");
        Debug.Log("Scene: " + sceneName);
    }

}