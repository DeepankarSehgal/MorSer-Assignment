using UnityEngine;

public class PointSelectionManager : MonoBehaviour
{
    public Camera cam;
    public GameObject pointPrefab;

    public Vector3 pointA, pointB, pointC;
    public bool hasA, hasB, hasC;

    public delegate void OnPointsUpdated();
    public event OnPointsUpdated PointsChanged;

    void Update()
    {
        if (ToolManager.ActiveTool != ToolType.AngleMeasure)
            return;

        if (Input.GetMouseButtonDown(0))
            PickPoint();
    }

    void PickPoint()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!hasA)
            {
                pointA = hit.point; hasA = true;
                Instantiate(pointPrefab, pointA, Quaternion.identity);
            }
            else if (!hasB)
            {
                pointB = hit.point; hasB = true;
                Instantiate(pointPrefab, pointB, Quaternion.identity);
            }
            else if (!hasC)
            {
                pointC = hit.point; hasC = true;
                Instantiate(pointPrefab, pointC, Quaternion.identity);
            }

            PointsChanged?.Invoke();
        }
    }

    public void ClearPoints()
    {
        hasA = hasB = hasC = false;

        foreach (var p in GameObject.FindGameObjectsWithTag("PointMarker"))
            Destroy(p);

        PointsChanged?.Invoke();
    }
}
