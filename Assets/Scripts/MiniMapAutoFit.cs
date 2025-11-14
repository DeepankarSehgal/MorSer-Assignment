using UnityEngine;

public class MiniMapAutoFit : MonoBehaviour
{
    public Transform modelRoot;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        Bounds b = new Bounds(modelRoot.position, Vector3.zero);
        foreach (Renderer r in modelRoot.GetComponentsInChildren<Renderer>())
            b.Encapsulate(r.bounds);

        float size = Mathf.Max(b.extents.x, b.extents.z);
        cam.orthographicSize = size * 1.1f;
    }
}
