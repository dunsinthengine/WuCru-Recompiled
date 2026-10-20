#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class ClearPlayerPrefsMenu
{
    [MenuItem("Tools/Clear PlayerPrefs (Reset Save)")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("<color=green><b>PlayerPrefs deleted successfully! Clean state active.</b></color>");
    }
}
#endif
