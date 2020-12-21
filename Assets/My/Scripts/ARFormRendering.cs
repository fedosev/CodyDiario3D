using UnityEngine;
using UnityEngine.UI;
using easyar;

namespace ARFormOptions {

public class ARFormRendering : MonoBehaviour {
    public CameraImageRenderer CameraRenderer;
    public MeshRenderer renderer;

    public Material material;
    public ImageTargetController imageTarget;
    public RenderTexture renderTexture;
    private RenderTexture renderTexture2;

    private ARMarkerFormContainer aRMarkerFormContainer;
    private Camera cam;

    private void Awake() {
        CameraRenderer = GameObject.FindObjectOfType<CameraImageRenderer>();
        if (imageTarget == null) {
            imageTarget = GetComponentInParent<ImageTargetController>();
        }
        material = renderer.material;
        if (CameraRenderer) {
            CameraRenderer.RequestTargetTexture((camera, texture) => { 
                // cam = camera;
                renderTexture = texture; 
            });
        }
        // aRMarkerFormContainer = GetComponent<ARMarkerFormContainer>();
    }

    // ARSession arSession;
    // void Update() {

    //     if (arSession) return;
    //     arSession = FindObjectOfType<ARSession>();
    //     if (!arSession) return;
    //     // arSession.FrameChange += OnFrameChange;
    //     arSession.FrameUpdate += OnFrameUpdate;
    // }

    // void Renderprepare() {

    //     if (!cam) {
    //         GameObject go = new GameObject("__cam");
    //         cam = go.AddComponent<Camera>();
    //         go.transform.parent = transform.parent;
    //         cam.hideFlags = HideFlags.HideAndDontSave;
    //     }
    //     cam.CopyFrom(Camera.main);
    //     cam.depth = 0;
    //     cam.cullingMask = 1 << 7;

    //     // if (!renderTexture2) {
    //     //     // renderTexture = new RenderTexture(Screen.width, Screen.height, -50);
    //     //     renderTexture2 = new RenderTexture(renderTexture.width, renderTexture.height, 0);
    //     // }

    //     // Graphics.Blit(renderTexture, renderTexture2);

    //     cam.targetTexture = renderTexture;
    //     cam.Render();
    //     // GetComponent<Renderer>().material.SetTexture("_MainTex", renderTexture);
    //     renderer.GetComponent<Renderer>().material.SetTexture("_MainTex", renderTexture);
    // }


    // private void OnFrameChange(OutputFrame outputFrame, Matrix4x4 displayCompensation) {
    // private void OnFrameUpdate(OutputFrame outputFrame) {
    private void Update() {

        if (imageTarget.Target == null) {
            return;
        }

        var halfWidth = 0.5f;
        var halfHeight = 0.5f / imageTarget.Target.aspectRatio();
        Matrix4x4 points = Matrix4x4.identity;
        Vector3 targetAnglePoint1 = imageTarget.transform.TransformPoint(new Vector3(-halfWidth, halfHeight, 0));
        Vector3 targetAnglePoint2 = imageTarget.transform.TransformPoint(new Vector3(-halfWidth, -halfHeight, 0));
        Vector3 targetAnglePoint3 = imageTarget.transform.TransformPoint(new Vector3(halfWidth, halfHeight, 0));
        Vector3 targetAnglePoint4 = imageTarget.transform.TransformPoint(new Vector3(halfWidth, -halfHeight, 0));
        points.SetRow(0, new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
        points.SetRow(1, new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
        points.SetRow(2, new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
        points.SetRow(3, new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
        // Renderprepare();
        material.SetMatrix("_UvPints", points);
        material.SetMatrix("_RenderingViewMatrix", Camera.main.worldToCameraMatrix);
        material.SetMatrix("_RenderingProjectMatrix", GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false));
        material.SetTexture("_MainTex", renderTexture);

        // aRMarkerFormContainer.UpdateFrame();
    }

    void OnDestroy() {
        if (cam)
            DestroyImmediate(cam.gameObject);
        if (renderTexture)
            DestroyImmediate(renderTexture);
    } 
}

}