using UnityEngine;

public class AngleVisualizer : MonoBehaviour
{
    public PointSelectionManager manager;

    public LineRenderer lineAB;
    public LineRenderer lineCB;
    public ArcGenerator arcGen;

    private void OnEnable()
    {
        manager.PointsChanged += UpdateVisuals;
    }

    private void OnDisable()
    {
        manager.PointsChanged -= UpdateVisuals;
    }

    void UpdateVisuals()
    {
        if (!manager.hasA || !manager.hasB)
        {
            lineAB.gameObject.SetActive(false);
            lineCB.gameObject.SetActive(false);
            return;
        }

        // AB line
        lineAB.gameObject.SetActive(true);
        lineAB.SetPosition(0, manager.pointB);
        lineAB.SetPosition(1, manager.pointA);

        // CB line
        if (manager.hasC)
        {
            lineCB.gameObject.SetActive(true);
            lineCB.SetPosition(0, manager.pointB);
            lineCB.SetPosition(1, manager.pointC);
        }

        // Arc
        if (manager.hasA && manager.hasB && manager.hasC)
            arcGen.DrawArc(manager.pointA, manager.pointB, manager.pointC);
    }
}
