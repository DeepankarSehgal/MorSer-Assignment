using UnityEngine;

public class UI_ToolButtons : MonoBehaviour
{
    void Start()
    {
        ToolManager.SetTool(ToolType.Default);
    }

    public void OnClick_Default()
    {
        ToolManager.SetTool(ToolType.Default);
    }

    public void OnClick_AngleTool()
    {
        ToolManager.SetTool(ToolType.AngleMeasure);
    }

    public void OnClick_ChangePivot()
    {
        ToolManager.SetTool(ToolType.ChangePivot);
    }
}
