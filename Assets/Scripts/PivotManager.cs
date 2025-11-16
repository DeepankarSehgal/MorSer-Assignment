using UnityEngine;

public class PivotManager : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Transform modelRoot;            // Parent of B1007 or your model
    public GameObject pivotMarkerPrefab;

    [Header("Settings")]
    public float rotateSpeed = 0.3f;
    public float zoomSpeed = 5f;
    public float zoomMultiplier = 0.2f;
    public float panSpeed = 0.005f;

    private Vector3 pivotPoint;
    private GameObject pivotMarker;

    private bool pivotSet = false;
    private bool rotating = false;
    private Vector3 lastMousePos;

    private int modelLayerMask;

    void Start()
    {
        // Layer mask for raycast
        modelLayerMask = 1 << LayerMask.NameToLayer("Model");

        // Set DEFAULT pivot = center of model
        ResetPivotToCenter();
    }

    void Update()
    {
        HandleZoom();

        // DEFAULT MODE — rotate + pan
        if (ToolManager.ActiveTool == ToolType.Default)
        {
            HandleDefaultRotation();
            HandlePan();
        }

        // CHANGE PIVOT MODE — left click sets pivot
        if (ToolManager.ActiveTool == ToolType.ChangePivot)
        {
            HandlePivotSelection();
            HandleDefaultRotation();
            HandlePan();
        }

    }

    //  DEFAULT ROTATION

    void HandleDefaultRotation()
    {
        if (!pivotSet) return;

        if (Input.GetMouseButtonDown(1))
        {
            rotating = true;
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            rotating = false;
        }

        if (rotating)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            float rotX = delta.y * rotateSpeed;
            float rotY = -delta.x * rotateSpeed;

            transform.Rotate(Vector3.up, rotY, Space.World);
            transform.Rotate(cam.transform.right, rotX, Space.World);

            lastMousePos = Input.mousePosition;
        }
    }

    //  DEFAULT PAN (MMB)

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(2))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            Vector3 move =
                cam.transform.right * -delta.x +
                cam.transform.up * -delta.y;

            cam.transform.position += move * panSpeed;

            lastMousePos = Input.mousePosition;
        }
    }

    //  CHANGE PIVOT TOOL

    void HandlePivotSelection()
    {
        if (ToolManager.ActiveTool != ToolType.ChangePivot)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 500f, modelLayerMask))
            {
                SetPivot(hit.point);
            }
        }
    }


    //  SET PIVOT FUNCTION

    public void SetPivot(Vector3 point)
    {
        pivotSet = true;
        pivotPoint = point;

        // Create or move marker
        if (pivotMarker == null)
            pivotMarker = Instantiate(pivotMarkerPrefab);

        pivotMarker.transform.position = pivotPoint;

        // TRICK: Move pivot root to hit point but keep model still
        Vector3 offset = modelRoot.position - pivotPoint;
        transform.position = pivotPoint;
        modelRoot.position = pivotPoint + offset;
    }


    // RESET PIVOT TO MODEL CENTER

    public void ResetPivotToCenter()
    {
        Bounds b = new Bounds(modelRoot.position, Vector3.zero);

        foreach (Renderer r in modelRoot.GetComponentsInChildren<Renderer>())
            b.Encapsulate(r.bounds);

        SetPivot(b.center);
    }


    //  ZOOM (Mouse Scroll Wheel)

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.001f) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Vector3 zoomDir;
        float distance;

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, modelLayerMask))
        {
            zoomDir = (hit.point - cam.transform.position).normalized;
            distance = Vector3.Distance(cam.transform.position, hit.point);
        }
        else
        {
            zoomDir = cam.transform.forward;
            distance = 5f;
        }

        cam.transform.position += zoomDir * scroll * zoomSpeed * distance * zoomMultiplier;
    }

}
