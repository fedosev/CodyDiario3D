using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using easyar;

namespace ARFormOptions {

// class TransformCopy {
//     public Vector3 pos;
//     public Quaternion rot;
//     public Vector3 localScale;

//     public TransformCopy(Transform transform) {
//         pos = transform.position;
//         rot = transform.rotation;
//         localScale = transform.localScale;
//     }

//     public void CopyTo(Transform transform) {
//         transform.position = pos;
//         transform.rotation = rot;
//         transform.localScale = localScale;
//     }
// }

public class ARMarkerFormContainer : MonoBehaviour {

    private ARForm form;

    public float offset = 3.84f;

    public RenderTexture renderTexture;
    public GameObject outObj;
    public Color bgColor;

    public int gridCols = 15;
    public int gridRows = 15;
    public int texturePixelScale = 5;
    public int texturePixelPadding = 0;
    

    [Range(0.01f, 0.2f)]
    public float minChangeToApply = 0.03f;

    public int avgFrames = 15;

    int textureWidth;
    int textureHeight;

    Camera cam;
    // TransformCopy camTransform;

    [HideInInspector]
    public int screenWidth = 0;

    [HideInInspector]
    public int screenHeight = 0;

    //[SerializeField]
    private Texture2D tex2d;
    private Color[] tex2dColors;

    Vec2 texOffset;

    private Color GetText2dPixel(int x, int y) {
        return tex2dColors[renderTexture.width * y + x];
    }

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

    ARSession arSession;
    void Update() {
        
        if (arSession) return;
        arSession = FindObjectOfType<ARSession>();
        if (!arSession) return;
        // arSession.FrameChange += OnFrameChange;
        arSession.FrameUpdate += OnFrameUpdate;
    }

    // Update is called once per frame

    // public void UpdateFrame() {
    // private void OnFrameChange(OutputFrame outputFrame, Matrix4x4 displayCompensation) {
    private void OnFrameUpdate(OutputFrame outputFrame) {

        gameObject.layer = 16;

        transform.rotation = Camera.main.transform.rotation;
        transform.position = Camera.main.transform.position;
        transform.Translate(Vector3.forward * offset, Space.Self);

        _OnWillRenderObject();

    }

    void _OnWillRenderObject() {

        if (!cam) {
            GameObject go = new GameObject("_ARFormCamera");
            cam = go.AddComponent<Camera>();
            //go.transform.parent = transform.parent;
            cam.hideFlags = HideFlags.HideAndDontSave;
        }
        cam.CopyFrom(Camera.main);
        // if (camTransform != null) {
        //     camTransform.Load(cam.transform);
        // }
        // camTransform = new StoredTransform(Camera.main.transform);
        cam.depth = 0;
        cam.cullingMask = 1 << 8;
        // MyDebug.Log(cam.cullingMask, true);
        cam.backgroundColor = bgColor;
        //cam.orthographic = true;
        //cam.orthographicSize = 10f;


        // Viewport frustum width and height
        var distanceY = (cam.ViewportToWorldPoint(new Vector3(0, 0, offset)) - cam.ViewportToWorldPoint(new Vector3(0, 1, offset))).magnitude;
        var distanceX = (cam.ViewportToWorldPoint(new Vector3(0, 0, offset)) - cam.ViewportToWorldPoint(new Vector3(1, 0, offset))).magnitude;
        
        // var distance = Mathf.Min(distanceX, distanceY);
        // transform.localScale = Vector3.one * distance;
        
        float ratioScreen = (float)Screen.width / (float)Screen.height;
        float ratioObject = (float)gridCols / (float)gridRows;

        if (ratioObject > ratioScreen) {
            transform.localScale = new Vector3(distanceX, distanceX / ratioObject, 1f);
        } else {
            transform.localScale = new Vector3(distanceY * ratioObject, distanceY, 1f);
        }


        if (!renderTexture || screenWidth != Screen.width || screenHeight != Screen.height) {
            //MyDebug.Log(new Vector4(screenWidth, Screen.width, screenHeight, Screen.height));
            if (ratioObject > ratioScreen) {
                // Add top and bottom padding
                /*
                var width = textureWidth * Screen.width / Screen.height;
                width -= (textureWidth - width) % texturePixelScale; // ?
                texOffset = new Vec2((width - textureWidth) / 2, 0);
                renderTexture = new RenderTexture(width, textureWidth, -50);
                // */
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
                // */
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

        //tex2dColors = tex2d.GetPixels();
        //outObj2.GetComponent<RawImage>().texture = tex2d;
        //tex2d.ReadPixels(new Rect(0, 0, 150*3, 150), 0, 0);
        //tex2d.Apply();

        form.CheckElements();

        screenWidth = Screen.width;
        screenHeight = Screen.height;

    }

    Dictionary<int, AvgVal> avgFramesVals = new Dictionary<int, AvgVal>();

    public float GetAvgGrayscale(Vec2 pos) {
        float avg = 0;
        int n = texturePixelScale - texturePixelPadding * 2;
        int pd = texturePixelPadding;
        if (n < 1) {
            n = texturePixelScale;
            pd = 0;
        }
        for (var i = 0; i < n; i++) {
            for (var j = 0; j < n; j++) {
                avg += tex2d.GetPixel(
                //avg += GetText2dPixel(
                    i + texOffset.x + pd + pos.x * texturePixelScale,
                    j + texOffset.y + pd + pos.y * texturePixelScale
                ).grayscale;
            }
        }

        avg = avg / (n * n);

        int key = pos.y * gridCols + pos.x;
        
        AvgVal avgVal;
        if (!avgFramesVals.TryGetValue(key, out avgVal)) {
            avgVal = new AvgVal(avgFrames);
            avgFramesVals.Add(key, avgVal);
        }
        
        avgVal.Add(avg);

        return avgVal.Get();
    }

    void OnDestroy() {
        if (cam) {
            Destroy(cam.gameObject);
        }
        if (arSession) {
            arSession.FrameUpdate -= OnFrameUpdate;
        }
    }

}

}

class AvgVal {

    float val;
    float[] vals;

    int size;
    int index = 0;
    float weight;

    public AvgVal(int size) {
        this.size = size;
        vals = new float[size];
        weight = 1f / size;
    }

    public AvgVal(int size, float avg) : this(size) {
        val = avg;
        for (int i = 0; i < size; i++) {
            vals[i] = avg;
        }
        index = size;
    }

    public void Add(float v) {
        if (index < size) {
            vals[index] = v;
            val = val * index / ++index + v / index;
        } else {
            val += (v - vals[index % size]) * weight;
            vals[index % size] = v;
            index = (index + 1) % size + size;
        }
    }

    public void Add(float v, int times) {
        for (int i = 0; i < times; i++) {
            this.Add(v);
        }
    }

    public float Get() {
        return val;
    }
    
}
