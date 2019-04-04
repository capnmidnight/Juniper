#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleCreate
{
    [MenuItem("WaveVR/Controller/Create AssetBundles")]
	static void CreateAnAssetBundle()
	{
		// Clean all loaded assetbundles
#if UNITY_2017_1_OR_NEWER
		Caching.ClearCache();
#else
		Caching.CleanCache();
#endif

        var currentSelect = Selection.activeObject;
        if (currentSelect == null)
        {
            Debug.Log("Without any selection");
            return;
        }

        string folderName = currentSelect.name;
        Debug.Log("folderName = " + folderName);

        var folderPath = AssetDatabase.GetAssetPath(currentSelect.GetInstanceID());
        Debug.Log("folderPath = " + folderPath);
        if (!Directory.Exists(folderPath))
        {
            Debug.Log("Must be root of controller model");
            return;
        }

        Object[] SelectedAsset = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);

		var assetBundleBuilds = new List<AssetBundleBuild>();

		foreach (Object obj in SelectedAsset)
		{
			string sourcePath = AssetDatabase.GetAssetPath (obj);

			assetBundleBuilds.Add (new AssetBundleBuild(){
				assetBundleName = obj.name,
				assetBundleVariant = "",
				assetNames = new string[] {
					sourcePath
				}
			});
		}

		var path = Path.GetFullPath(Application.dataPath + "/../Unity");
		if (!Directory.Exists (path))
		{
			Directory.CreateDirectory (path);
		} else
		{
			DirectoryInfo di = new DirectoryInfo (path);
			foreach (FileInfo fi in di.GetFiles())
				fi.Delete ();
		}

		BuildPipeline.BuildAssetBundles (
			path,
			assetBundleBuilds.ToArray (),
			BuildAssetBundleOptions.ForceRebuildAssetBundle,
			BuildTarget.Android);

		AssetDatabase.Refresh ();
	}
}
#endif
