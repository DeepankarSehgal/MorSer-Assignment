using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ArcGenerator : MonoBehaviour
{
    public float radius = 0.15f;
    public int segments = 30;

    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void DrawArc(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 BA = (A - B).normalized;
        Vector3 BC = (C - B).normalized;

        //Find the plane normal
        Vector3 normal = Vector3.Cross(BA, BC).normalized;
        if (normal == Vector3.zero) return; // colinear points

        //Signed angle between BA and BC
        float angle = Vector3.SignedAngle(BA, BC, normal);

        int steps = Mathf.Max(3, Mathf.RoundToInt(Mathf.Abs(angle) / (360f / segments)));

        Vector3[] verts = new Vector3[steps + 2];
        int[] tris = new int[steps * 3];

        verts[0] = Vector3.zero;

        //CUSTOM ORIENTATION MATRIX (fixes rotation)
        // Local coordinate axes
        Vector3 xAxis = BA;                    // X = line BA
        Vector3 zAxis = normal;                // Z = plane normal
        Vector3 yAxis = Vector3.Cross(zAxis, xAxis).normalized; // Y perp to both

        Matrix4x4 localToWorld = new Matrix4x4();
        localToWorld.SetColumn(0, new Vector4(xAxis.x, xAxis.y, xAxis.z, 0));
        localToWorld.SetColumn(1, new Vector4(yAxis.x, yAxis.y, yAxis.z, 0));
        localToWorld.SetColumn(2, new Vector4(zAxis.x, zAxis.y, zAxis.z, 0));
        localToWorld.SetColumn(3, new Vector4(0, 0, 0, 1));

        //Build arc vertices using this orientation
        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float currentAngle = Mathf.Lerp(0f, angle, t) * Mathf.Deg2Rad;

            // Local rotation
            float cos = Mathf.Cos(currentAngle);
            float sin = Mathf.Sin(currentAngle);

            Vector3 localDir = new Vector3(
                cos,     // x
                sin,     // y
                0f       // z (in the plane)
            );

            // Convert from arc local  world-aligned plane
            Vector3 worldDir = localToWorld.MultiplyVector(localDir);

            verts[i + 1] = worldDir * radius;
        }

        // triangles
        for (int i = 0; i < steps; i++)
        {
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        // assign mesh
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        transform.position = B;

        //Apply plane orientation to the mesh object
        transform.rotation = Quaternion.LookRotation(normal, yAxis);
    }

    public void ClearArc()
    {
        mesh.Clear();
    }
}
