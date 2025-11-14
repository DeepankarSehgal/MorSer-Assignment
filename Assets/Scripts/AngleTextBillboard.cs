using UnityEngine;
using TMPro;

public class AngleTextBillboard : MonoBehaviour
{
    public PointSelectionManager manager;
    public TextMeshPro angleText;
    public TMP_Text angleTextUi;
    public Camera cam;

    public float offsetDistance = 0.03f;
    public float heightOffset = 0.02f;

    void LateUpdate()
    {
        if (!(manager.hasA && manager.hasB && manager.hasC))
        {
            angleText.text = "";
            angleTextUi.text = "";
            return;
        }

        float angle = Vector3.Angle(
            manager.pointA - manager.pointB,
            manager.pointC - manager.pointB
        );

        angleText.text = angle.ToString("F2") + "°";
        angleTextUi.text = angle.ToString("F2") + "°";

        Vector3 B = manager.pointB;

        Vector3 camDir = (cam.transform.position - B).normalized;
        Vector3 forwardOffset = camDir * offsetDistance;
        Vector3 upOffset = Vector3.up * heightOffset;

        angleText.transform.position = B + forwardOffset + upOffset;

        angleText.transform.rotation = Quaternion.LookRotation(
            angleText.transform.position - cam.transform.position
        );

        angleText.transform.rotation = Quaternion.Euler(
            0,
            angleText.transform.rotation.eulerAngles.y,
            0
        );
    }
}
