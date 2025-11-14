using UnityEngine;

public enum ToolType
{
    Default,
    AngleMeasure,
    ChangePivot
}

public class ToolManager : MonoBehaviour
{
    public static ToolType ActiveTool = ToolType.Default;

    public static void SetTool(ToolType tool)
    {
        ActiveTool = tool;
    }
}
