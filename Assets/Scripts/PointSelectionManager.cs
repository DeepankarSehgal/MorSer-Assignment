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
            // Ensure collider is a MeshCollider
            MeshCollider meshCol = hit.collider as MeshCollider;
            if (meshCol == null || meshCol.sharedMesh == null)
                return;

            Mesh mesh = meshCol.sharedMesh;

            // Get triangle vertices (local mesh space)
            int triIndex = hit.triangleIndex * 3;
            Vector3 v0 = mesh.vertices[mesh.triangles[triIndex]];
            Vector3 v1 = mesh.vertices[mesh.triangles[triIndex + 1]];
            Vector3 v2 = mesh.vertices[mesh.triangles[triIndex + 2]];

            // Convert to world space
            Transform t = meshCol.transform;
            v0 = t.TransformPoint(v0);
            v1 = t.TransformPoint(v1);
            v2 = t.TransformPoint(v2);

            // Pick nearest vertex
            Vector3 chosenVertex = v0;
            float d0 = Vector3.Distance(hit.point, v0);
            float d1 = Vector3.Distance(hit.point, v1);
            float d2 = Vector3.Distance(hit.point, v2);

            if (d1 < d0) { chosenVertex = v1; d0 = d1; }
            if (d2 < d0) { chosenVertex = v2; }

            // Assign A, B, or C
            if (!hasA)
            {
                pointA = chosenVertex; hasA = true;
                Instantiate(pointPrefab, pointA, Quaternion.identity);
            }
            else if (!hasB)
            {
                pointB = chosenVertex; hasB = true;
                Instantiate(pointPrefab, pointB, Quaternion.identity);
            }
            else if (!hasC)
            {
                pointC = chosenVertex; hasC = true;
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
