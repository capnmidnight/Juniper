using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum UBERTexBakeType {
	Normals,
	Tangents
}

public static class UBER_BakeTanNormalsEditor {
	static int size=1024;
	static string save_path_Norm = "";
	static string save_path_Tan = "";

	static Mesh lastMesh=null;

	[MenuItem("CONTEXT/MeshFilter/Bake Object Topology to Texture")]
	public static void BakeNormals (MenuCommand menuCommand) {
		Process (menuCommand.context as MeshFilter, UBERTexBakeType.Normals, ref save_path_Norm);
		Process (menuCommand.context as MeshFilter, UBERTexBakeType.Tangents, ref save_path_Tan);
	}

//	[MenuItem("CONTEXT/MeshFilter/Bake Object Tangents to Texture")]
//	public static void BakeTangents (MenuCommand menuCommand) {
//		Process (menuCommand.context as MeshFilter, UBERTexBakeType.Tangents, ref save_path_Tan);
//	}

	private static void Process(MeshFilter mf, UBERTexBakeType type, ref string save_path) {
		if (mf == null) return;
		Mesh m = mf.sharedMesh;
		if (m==null) return;
		Shader shader = Shader.Find ("Hidden/UBER_BakeProps");
		if (shader == null) {
			Debug.LogError("Can't find Hidden/UBER_BakeProps shader !");
		}

        TextureFormat format = TextureFormat.ARGB32;
		RenderTexture rt = new RenderTexture (size, size, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

		RenderTexture rendTarget = RenderTexture.active;
		RenderTexture.active = rt;

		Material mat = new Material (shader);
		mat.SetFloat("_NormalTangentFlag", (float)type); // 0 - normals, 1 - tangents 
		mat.SetPass(0);
		Graphics.DrawMeshNow(m, Matrix4x4.identity);
		UnityEngine.Object.DestroyImmediate(mat);

		// grab render texture to regular texture
		Texture2D tex = new Texture2D(size, size, format, false, true);
		tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
		tex.Apply(false,false);

		RenderTexture.active=rendTarget;
		UnityEngine.Object.DestroyImmediate(rt);

		MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
		string cur_path = "";
		if (mr && mr.sharedMaterial && mr.sharedMaterial.HasProperty ("_ObjectNormalsTex") && mr.sharedMaterial.HasProperty ("_ObjectTangentsTex")) {
			if (type==UBERTexBakeType.Normals) {
				if (mr.sharedMaterial.GetTexture("_ObjectNormalsTex")) {
					cur_path=AssetDatabase.GetAssetPath(mr.sharedMaterial.GetTexture("_ObjectNormalsTex"));
				}
			} else {
				if (mr.sharedMaterial.GetTexture("_ObjectTangentsTex")) {
					cur_path=AssetDatabase.GetAssetPath(mr.sharedMaterial.GetTexture("_ObjectTangentsTex"));
				}
			}
			if (cur_path!="") {
				cur_path=Application.dataPath.Substring(0,Application.dataPath.Length-6)+cur_path;
			}
		}

		string path;
		string dialogName = "Save texture - " + (type == UBERTexBakeType.Normals ? "Normals & Texture U coord to object space ratio" : "Tangents & Texture V coord object space ratio");
		if (cur_path != "") {
			path = EditorUtility.SaveFilePanel (dialogName, System.IO.Path.GetDirectoryName (cur_path), System.IO.Path.GetFileNameWithoutExtension (cur_path), System.IO.Path.GetExtension (cur_path).Substring (1));
		} else if (save_path != "" && lastMesh==m) {
			path = EditorUtility.SaveFilePanel(dialogName, System.IO.Path.GetDirectoryName(save_path), System.IO.Path.GetFileNameWithoutExtension(save_path), System.IO.Path.GetExtension(save_path).Substring(1));
		} else {
			path = EditorUtility.SaveFilePanel(dialogName, "Assets/", m.name+(type==UBERTexBakeType.Normals ? "_ObjNormals_RatioU" : "_ObjTangets_RatioV"), "png");
		}
		if (string.IsNullOrEmpty(path)) return;
		save_path = path;

		lastMesh = m;
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.DestroyImmediate(tex);
		System.IO.File.WriteAllBytes(path, bytes);
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		path = FileUtil.GetProjectRelativePath(path);
		AssetImporter _importer=AssetImporter.GetAtPath(path);
		if (_importer) {
			TextureImporter tex_importer=(TextureImporter)_importer;
			tex_importer.wrapMode=TextureWrapMode.Repeat;
			tex_importer.sRGBTexture=false;
			tex_importer.mipmapEnabled=false;
			tex_importer.maxTextureSize=128;
			tex_importer.textureCompression=TextureImporterCompression.Uncompressed;
		}
		AssetDatabase.ImportAsset(path,  ImportAssetOptions.ForceUpdate);

		if (mr && mr.sharedMaterial && mr.sharedMaterial.HasProperty("_ObjectNormalsTex") && mr.sharedMaterial.HasProperty("_ObjectTangentsTex")) {
			if (type==UBERTexBakeType.Normals) {
				mr.sharedMaterial.SetTexture("_ObjectNormalsTex", AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D);
			} else { 
				mr.sharedMaterial.SetTexture("_ObjectTangentsTex", AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D);
			}
		}

	}

}
