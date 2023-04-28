//===================================================
//Author      : DESKTOP-81VJOOT
//CreateTime  ：2019/11/24 15:28:26
//Description ：
//===================================================
using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorUtils
{
    public static void ExplorerFolder(string folder)
    {
        folder = string.Format("\"{0}\"", folder);
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                System.Diagnostics.Process.Start("Explorer.exe", folder.Replace('/', '\\'));
                break;
            case RuntimePlatform.OSXEditor:
                System.Diagnostics.Process.Start("open", folder);
                break;
            default:
               Debug.LogError(string.Format("Not support open folder on '{0}' platform.", Application.platform.ToString()));
                break;
        }
    }

}
