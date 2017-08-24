using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARFormOptions {

public class ARMarkerFormContainer : MonoBehaviour {

    private ARForm form;

    public float offset = 3.84f;

    public RenderTexture renderTexture;
    public GameObject outObj;
    public Color bgColor;

    public int gridCols = 15;
    public int gridRows = 15;
    public int texturePixelScale = 1;

    [Range(0f, 1f)]
    public float minChangeToApply = 0.03f;

    int textureWidth;
    int textureHeight;

    Camera cam;

    [HideInInspector]
    public int screenWidth = 0;

    [HideInInspector]
    public int screenHeight = 0;

    [SerializeField]
    private Texture2D tex2d;

    Vec2 texOffset;


    public void SetMinChangeToApply(float val) {
        minChangeToApply = val;
        MyDebug.Log(val, true, true);
    }

    // Use this for initialization
    void Start() {

        textureWidth = gridCols * texturePixelScale;
        textureHeight = gridRows * texturePixelScale;

        form = GetComponentInChildren<ARForm>();
        form.FormContainer = this;
    }

    // Update is called once per frame
    void Update() {

        gameObject.layer = 16;

        transform.rotation = Camera.main.transform.rotation;
        transform.position = Camera.main.transform.position;
        transform.Translate(Vector3.forward * offset, Space.Self);

        _OnWillRenderObject();

    }

    void _OnWillRenderObject() {

        if (!cam) {
            GameObject go = new GameObject("_myCam");
            cam = go.AddComponent<Camera>();
            //go.transform.parent = transform.parent;
            cam.hideFlags = HideFlags.HideAndDontSave;
        }
        cam.CopyFrom(Camera.main);
        cam.depth = 0;
        cam.cullingMask = 1 << 16;
        cam.backgroundColor = bgColor;
        //cam.orthographic = true;
        //cam.orthographicSize = 10f;

        // Viewport frustum width and heigth
        var distanceY = (cam.ViewportToWorldPoint(new Vector3(0, 0, offset)) - cam.ViewportToWorldPoint(new Vector3(0, 1, offset))).magnitude;
        var distanceX = (cam.ViewportToWorldPoint(new Vector3(0, 0, offset)) - cam.ViewportToWorldPoint(new Vector3(1, 0, offset))).magnitude;
        
        /*
        var distance = Mathf.Min(distanceX, distanceY);
        transform.localScale = Vector3.one * distance;
        */
        float ratioScreen = (float)Screen.width / (float)Screen.height;
        float ratioObject = (float)gridCols / (float)gridRows;

        if (ratioObject > ratioScreen) {
            transform.localScale = new Vector3(distanceX, distanceX / ratioObject, 1f);
        } else {
            transform.localScale = new Vector3(distanceY * ratioObject, distanceY, 1f);
        }


        if (!renderTexture || screenWidth != Screen.width || screenHeight != Screen.height) {
            //Debug.Log(new Vector4(screenWidth, Screen.width, screenHeight, Screen.height));
            if (ratioObject > ratioScreen) {
                // Add top and bottom padding
                /*
                var width = textureWidth * Screen.width / Screen.height;
                width -= (textureWidth - width) % texturePixelScale; // ?
                texOffset = new Vec2((width - textureWidth) / 2, 0);
                renderTexture = new RenderTexture(width, textureWidth, -50);
                */
                int heightWithPadding = (int)(textureHeight * ratioObject / ratioScreen);
                // @todo: edge cases (remainder int conversion)
                texOffset = new Vec2(0, (heightWithPadding - textureHeight) / 2);
                renderTexture = new RenderTexture(textureWidth, heightWithPadding, -50);
            } else {
                /*
                var height = textureHeight * Screen.height / Screen.width;
                // if (height % 2 == 0)
                //    height++;
                height -= (textureHeight - height) % texturePixelScale; // ?
                texOffset = new Vec2(0, (height - textureHeight) / 2);
                renderTexture = new RenderTexture(textureHeight, height, -50);
                */
                int widthWithPadding = (int)(textureWidth * ratioScreen);
                // @todo: edge cases (remainder int conversion)
                texOffset = new Vec2((widthWithPadding - textureWidth) / 2, 0);
                renderTexture = new RenderTexture(widthWithPadding, textureHeight, -50);
            }
            renderTexture.filterMode = FilterMode.Point;

        }

        cam.targetTexture = renderTexture;
        cam.Render();

        CheckColors();

        //outObj.GetComponent<Renderer>().material.SetTexture("_MainTex", renderTexture);
        if (outObj != null) {
            var img = outObj.GetComponent<RawImage>();
            if (outObjWidth == 0)
                outObjWidth = (int)img.rectTransform.rect.width;
            img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, outObjWidth * Screen.width / Screen.height);
            //img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height / 3);
            img.texture = renderTexture;
        }
    }

    private int outObjWidth = 0;

    void CheckColors() {

        if (!tex2d || screenWidth != Screen.width || screenHeight != Screen.height) {
            tex2d = new Texture2D(renderTexture.width, renderTexture.height);
            tex2d.filterMode = FilterMode.Point;
        }


        // https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
        RenderTexture tmp = RenderTexture.GetTemporary(tex2d.width, tex2d.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(renderTexture, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;
        tex2d.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        tex2d.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        //outObj2.GetComponent<RawImage>().texture = tex2d;

        //tex2d.ReadPixels(new Rect(0, 0, 150*3, 150), 0, 0);
        //tex2d.Apply();

        form.CheckElements();

        screenWidth = Screen.width;
        screenHeight = Screen.height;

    }

    public float GetAvgGrayscale(Vec2 pos) {
        float avg = 0;
        for (var i = 0; i < texturePixelScale; i++) {
            for (var j = 0; j < texturePixelScale; j++) {
                avg += tex2d.GetPixel(
                    i + texOffset.x + pos.x * texturePixelScale,
                    j + texOffset.y + pos.y * texturePixelScale
                ).grayscale;
            }
        }

        return avg / (texturePixelScale * texturePixelScale);
    }

}

}