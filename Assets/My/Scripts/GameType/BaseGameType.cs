using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseGameType : ScriptableObject {

    public abstract string sceneName { get; }
    public virtual string sceneNameNoAR { get {
        return sceneName;
    } }

    public abstract string title { get; }
    public virtual string subTitle { get {
        return "Gioco del " + name + " " + MyDate.GetMonthName(month) + " " + year;
    } }
    public virtual string generalInfo { get; }

    public int year = 2017;
    public int month = 11;

    public bool showInfoOnStart = true;
    [TextArea(10, 10)] public string info = "";

    public string GetInfo() {
        if (info != "")
            return info;
        return generalInfo;
    }
    
    public virtual void Init() {

        BeforeInit();
        InitBody();
        AfterInit();
    }

    public virtual void BeforeInit() {
        MyDebug.Log("BaseGameType INIT");
        MyDebug.Log("Scene: " + sceneName);
    }

    public abstract void InitBody();

    public virtual void AfterInit() {
        /*
        if (useAR) {
            Component.FindObjectOfType<BaseGameTypeManager>().ShowGame(false);
        }
        */
    }

    public virtual void Pause(bool pause) { }

}