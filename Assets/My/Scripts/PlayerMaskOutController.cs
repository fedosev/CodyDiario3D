using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaskOutController : MonoBehaviour {

    public GameObject playerMasker;
    public float scaleFactor;
    public float mainImgTargetWidth;
    public float scaleOffset;

    protected Vector3 prevPosition;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (prevPosition != transform.parent.position) {

            prevPosition = transform.parent.position;

            var yScale = (transform.parent.localPosition.y * scaleFactor / mainImgTargetWidth - scaleOffset);
            playerMasker.transform.localScale = new Vector3(0.22f * transform.parent.localPosition.y, yScale / 2, 0.22f * transform.parent.localPosition.y);

            playerMasker.transform.position = new Vector3(
                transform.parent.position.x, yScale * 1.5f,
                transform.parent.position.z
            );
        }

    }

}
