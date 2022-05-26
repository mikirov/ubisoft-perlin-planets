#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EditorUtils
{
    public enum BuildErrorType
    {
        NoError = 0,
        UnityBuildFail = -1,
        BuiltDataCopyFail = -2
    }

    public static void FinishBuild(BuildErrorType errorType)
    {
        if (errorType == BuildErrorType.NoError)
        {
            Debug.Log("All build steps succeeded");
        }
        else
        {
            Debug.LogError("Build failed for reason: " + errorType);
        }
#if UNITY_EDITOR
        // EditorApplication.Exit((int)errorType);
#endif
    }
}