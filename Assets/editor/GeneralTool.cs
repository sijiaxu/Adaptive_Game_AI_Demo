using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class GeneralTool : Editor {

    /// <summary>
    /// 场景打包 （包括依赖的资源文件和所有的代码），代码层级unity不会自动找到依赖
    /// </summary>
    [MenuItem("PL/Export Scene")]
    public static void testfunction2()
    {
        var paths = AssetDatabase.GetAllAssetPaths();

        List<string> allExportPaths = new List<string>();

        foreach (var path in paths)
        {
            if (path.EndsWith(".cs") || path.EndsWith(".js") || path.EndsWith(".dll"))
                allExportPaths.Add(path);
        }

        //allExportPaths.Add("Assets/Scene/ImagewallScene1.prefab");
		allExportPaths.Add("Assets/Demo.unity");

        AssetDatabase.ExportPackage(allExportPaths.ToArray(), "scene.unitypackage", ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);
    }

   
}
