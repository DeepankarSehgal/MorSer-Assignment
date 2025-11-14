using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniMapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Camera miniMapCam;
    public Transform cameraTarget;   // CameraTarget object
    public Transform modelRoot;

    public float smoothSpeed = 8f;
    public bool smoothMovement = true;

    private Bounds modelBounds;

    void Start()
    {
        modelBounds = new Bounds(modelRoot.position, Vector3.zero);
        foreach (Renderer r in modelRoot.GetComponentsInChildren<Renderer>())
            modelBounds.Encapsulate(r.bounds);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rect = transform as RectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        float u = Mathf.InverseLerp(rect.rect.xMin, rect.rect.xMax, localPoint.x);
        float v = Mathf.InverseLerp(rect.rect.yMin, rect.rect.yMax, localPoint.y);

        if (u < 0 || u > 1 || v < 0 || v > 1)
            return;

        float worldX = Mathf.Lerp(modelBounds.min.x, modelBounds.max.x, u);
        float worldZ = Mathf.Lerp(modelBounds.min.z, modelBounds.max.z, v);

        worldX = Mathf.Clamp(worldX, modelBounds.min.x, modelBounds.max.x);
        worldZ = Mathf.Clamp(worldZ, modelBounds.min.z, modelBounds.max.z);

        Vector3 targetPos = new Vector3(
            worldX,
            cameraTarget.position.y,   // keep height the same
            worldZ
        );

        if (smoothMovement)
            StartCoroutine(SmoothMove(targetPos));
        else
            cameraTarget.position = targetPos;
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        while (Vector3.Distance(cameraTarget.position, target) > 0.1f)
        {
            cameraTarget.position = Vector3.Lerp(
                cameraTarget.position,
                target,
                Time.deltaTime * smoothSpeed
            );
            yield return null;
        }

        cameraTarget.position = target;
    }
}
