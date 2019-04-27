using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public enum UBERColorChannels {
	R, G, B, A
}

public class UBER_TextureChannelMixer : EditorWindow
{
	public bool sizeMismatch = false;

	public Material target_mat;

	public Texture2D source_tex0;
	public int source_col0=255;
	public Texture2D source_tex1;
	public int source_col1=255;
	public Texture2D source_tex2;
	public int source_col2=255;
	public Texture2D source_tex3;
	public int source_col3=255;
	public Texture2D rendered_tex;

	public UBERColorChannels sourceChannel0=UBERColorChannels.R;
	public UBERColorChannels sourceChannel1=UBERColorChannels.R;
	public UBERColorChannels sourceChannel2=UBERColorChannels.R;
	public UBERColorChannels sourceChannel3=UBERColorChannels.R;
	public string save_path="";
	public string directory="";
	public string file="output.png";
	public bool linearTexture=true;
	bool finalize=false;

	// shown from UBER material inspector
//	[MenuItem("Window/UBER/Texture channels mixer")]
//	public static void ShowWindow() {
//		UBER_TextureChannelMixer window=EditorWindow.GetWindow(typeof(UBER_TextureChannelMixer)) as UBER_TextureChannelMixer;
// 		window.title="Texture mixer";
//		window.minSize=new Vector2(360,626);
//		window.maxSize=new Vector2(370,628);
//	}

	void OnGUI() {
		if (finalize) {
			// select created texture
			Selection.activeObject=AssetDatabase.LoadAssetAtPath(save_path, typeof(Texture2D));
			finalize=false;
		}

		EditorGUILayout.Space();

		target_mat=EditorGUILayout.ObjectField ("Target material", target_mat, typeof(Material), true ) as Material;
		if (target_mat) {
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.LabelField ("Fill input textures from material:");
			EditorGUILayout.BeginHorizontal();
			bool _2layers = target_mat.GetTag("TWO_LAYERS", false)=="On";
			bool _specSetup = (target_mat.HasProperty("_SpecColor") && target_mat.HasProperty("_SpecGlossMap") && target_mat.GetTexture("_SpecGlossMap")!=null);
			bool _metSetup = (target_mat.HasProperty("_Metallic") && target_mat.HasProperty("_MetallicGlossMap") && target_mat.GetTexture("_MetallicGlossMap")!=null);
			if (_2layers) {
				if (_specSetup) {
					GetFromMaterialTexProp("_SpecGlossMap", "Spec/Gloss 1");
					GetFromMaterialTexProp("_ParallaxMap", "Height 1");
					GetFromMaterialTexProp("_SpecGlossMap2", "Spec/Gloss 2");
					GetFromMaterialTexProp("_ParallaxMap2", "Height 2");
				} else if (_metSetup) {
					GetFromMaterialTexProp("_MetallicGlossMap", "Metal/gloss 1");
					GetFromMaterialTexProp("_ParallaxMap", "Height 1");
					GetFromMaterialTexProp("_MetallicGlossMap2", "Metal/gloss 2");
					GetFromMaterialTexProp("_ParallaxMap2", "Height 2");
				} else {
					GetFromMaterialTexProp("_ParallaxMap", "Height");
					GetFromMaterialTexProp("_ParallaxMap2", "2nd Height");
				}
			} else {
				if (_specSetup) {
					GetFromMaterialTexProp("_SpecGlossMap", "Spec/Gloss");
					GetFromMaterialTexProp("_SpecularRGBGlossADetail", "Detail Spec/Gloss");
				} else if (_metSetup) {
					GetFromMaterialTexProp("_MetallicGlossMap", "Metal/gloss");
					GetFromMaterialTexProp("_MetallicGlossMapDetail", "Detail Metal/Gloss");
				}
				GetFromMaterialTexProp("_ParallaxMap", "Height");
			}
			GetFromMaterialTexProp("_OcclusionMap", "Occlusion");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		float skinAlpha = EditorGUIUtility.isProSkin ? 1.0f : 0.4f;
		float skinCol = EditorGUIUtility.isProSkin ? 1.0f : 0.7f;

		GUI.backgroundColor = new Color (skinCol, 0, 0, skinAlpha);
		EditorGUILayout.BeginVertical ("Box");
		source_tex0 = EditorGUILayout.ObjectField("Target R", source_tex0, typeof(Texture2D), false) as Texture2D;
		EditorGUI.BeginDisabledGroup(!source_tex0);
		GUI.backgroundColor = new Color (1, 1, 1, 1);
		if (GUILayout.Button ("Clear this texture slot")) {
			source_tex0=null;
		}
		GUI.backgroundColor = new Color (skinCol, 0, 0, skinAlpha);
		sourceChannel0 = (UBERColorChannels)EditorGUILayout.EnumPopup("Get from channel", sourceChannel0);
		EditorGUI.EndDisabledGroup();
		GUI.backgroundColor = new Color (1,1,1,1);
		EditorGUI.BeginDisabledGroup(source_tex0);
		source_col0 = EditorGUILayout.IntSlider("Constant Value", source_col0, 0, 255);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();

		GUI.backgroundColor = new Color (0, skinCol, 0, skinAlpha);
		EditorGUILayout.BeginVertical ("Box");
		source_tex1 = EditorGUILayout.ObjectField("Target G", source_tex1, typeof(Texture2D), false) as Texture2D;
		EditorGUI.BeginDisabledGroup(!source_tex1);
		GUI.backgroundColor = new Color (1, 1, 1, 1);
		if (GUILayout.Button ("Clear this texture slot")) {
			source_tex1=null;
		}
		GUI.backgroundColor = new Color (0, skinCol, 0, skinAlpha);
		sourceChannel1 = (UBERColorChannels)EditorGUILayout.EnumPopup("Get from channel", sourceChannel1);
		EditorGUI.EndDisabledGroup();
		GUI.backgroundColor = new Color (1,1,1,1);
		EditorGUI.BeginDisabledGroup(source_tex1);
		source_col1 = EditorGUILayout.IntSlider("Constant Value", source_col1, 0, 255);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();

		GUI.backgroundColor = new Color (0, 0, skinCol, skinAlpha);
		EditorGUILayout.BeginVertical ("Box");
		source_tex2 = EditorGUILayout.ObjectField("Target B", source_tex2, typeof(Texture2D), false) as Texture2D;
		EditorGUI.BeginDisabledGroup(!source_tex2);
		GUI.backgroundColor = new Color (1, 1, 1, 1);
		if (GUILayout.Button ("Clear this texture slot")) {
			source_tex2=null;
		}
		GUI.backgroundColor = new Color (0, 0, skinCol, skinAlpha);
		sourceChannel2 = (UBERColorChannels)EditorGUILayout.EnumPopup("Get from channel", sourceChannel2);
		EditorGUI.EndDisabledGroup();
		GUI.backgroundColor = new Color (1,1,1,1);
		EditorGUI.BeginDisabledGroup(source_tex2);
		source_col2 = EditorGUILayout.IntSlider("Constant Value", source_col2, 0, 255);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();

		GUI.backgroundColor = new Color (1, 1, 1, skinAlpha);
		EditorGUILayout.BeginVertical ("Box");
		source_tex3 = EditorGUILayout.ObjectField("Target A", source_tex3, typeof(Texture2D), false) as Texture2D;
		EditorGUI.BeginDisabledGroup(!source_tex3);
		GUI.backgroundColor = new Color (1, 1, 1, 1);
		if (GUILayout.Button ("Clear this texture slot")) {
			source_tex3=null;
		}
		GUI.backgroundColor = new Color (1, 1, 1, skinAlpha);
		sourceChannel3 = (UBERColorChannels)EditorGUILayout.EnumPopup("Get from channel", sourceChannel3);
		EditorGUI.EndDisabledGroup();
		GUI.backgroundColor = new Color (1,1,1, skinAlpha);
		EditorGUI.BeginDisabledGroup(source_tex3);
		source_col3 = EditorGUILayout.IntSlider("Constant Value", source_col3, 0, 255);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();

		int w=1024;
		int h=1024;
		if (source_tex0) { w=source_tex0.width; h=source_tex0.height; }
		else if (source_tex1) { w=source_tex1.width; h=source_tex1.height; }
		else if (source_tex2) { w=source_tex2.width; h=source_tex2.height; }
		else if (source_tex3) { w=source_tex3.width; h=source_tex3.height; }

		bool[] sources_ready_flag = new bool[4] { false, false, false, false };
		sources_ready_flag[0] = check_texture(source_tex0, 0, w, h);
		sources_ready_flag[1] = check_texture(source_tex1, 1, w, h);
		sources_ready_flag[2] = check_texture(source_tex2, 2, w, h);
		sources_ready_flag[3] = check_texture(source_tex3, 3, w, h);

		if (!sizeMismatch && GUILayout.Button("Render mixed texture")) {
			if (sources_ready_flag[0]) { w=source_tex0.width; h=source_tex0.height; }
			else if (sources_ready_flag[1]) { w=source_tex1.width; h=source_tex1.height; }
			else if (sources_ready_flag[2]) { w=source_tex2.width; h=source_tex2.height; }
			else if (sources_ready_flag[3]) { w=source_tex3.width; h=source_tex3.height; }
			rendered_tex=new Texture2D(w, h, TextureFormat.ARGB32, true);
			byte[] colsR;
			if (sources_ready_flag[0]) {
				colsR=get_color_channel(source_tex0, sourceChannel0);
			} else {
				colsR=new byte[w*h];
				for(int i=0; i<colsR.Length; i++) {
					colsR[i]=(byte)source_col0;
				}
			}
			byte[] colsG;
			if (sources_ready_flag[1]) {
				colsG=get_color_channel(source_tex1, sourceChannel1);
			} else {
				colsG=new byte[w*h];
				for(int i=0; i<colsG.Length; i++) {
					colsG[i]=(byte)source_col1;
				}
			}
			byte[] colsB;
			if (sources_ready_flag[2]) {
				colsB=get_color_channel(source_tex2, sourceChannel2);
			} else {
				colsB=new byte[w*h];
				for(int i=0; i<colsB.Length; i++) {
					colsB[i]=(byte)source_col2;
				}
			}
			byte[] colsA;
			if (sources_ready_flag[3]) {
				colsA=get_color_channel(source_tex3, sourceChannel3);
			} else {
				colsA=new byte[w*h];
				for(int i=0; i<colsA.Length; i++) {
					colsA[i]=(byte)source_col3;
				}
			}
			Color32[] cols=rendered_tex.GetPixels32();

			for(int i=0; i<cols.Length; i++) {
				cols[i].r=colsR[i];
				cols[i].g=colsG[i];
				cols[i].b=colsB[i];
				cols[i].a=colsA[i];
			}
			rendered_tex.SetPixels32(cols);
			if (save_path=="") {
				directory=Application.dataPath;
				file="output.png";
			}
			if (Selection.activeObject is Texture2D) {
				save_path=AssetDatabase.GetAssetPath(Selection.activeObject as Texture2D);
				directory=Path.GetDirectoryName(save_path);
				file=Path.GetFileNameWithoutExtension(save_path)+".png";
			}
		}

		if (rendered_tex) {
			linearTexture=GUILayout.Toggle(linearTexture, "Linear texture (Bypass sRGB Sampling)");
			if (GUILayout.Button("Save rendered texture")) {
				SaveTexture(directory, file);
			}
			if (target_mat) {
				EditorGUILayout.BeginVertical("Box");
				EditorGUILayout.LabelField ("Save rendered texture and put it to material:");
				EditorGUILayout.BeginHorizontal();
				bool _2layers = target_mat.GetTag("TWO_LAYERS", false)=="On";
				bool _specSetup = (target_mat.HasProperty("_SpecColor") && target_mat.HasProperty("_SpecGlossMap") && target_mat.GetTexture("_SpecGlossMap")!=null);
				bool _metSetup = (target_mat.HasProperty("_Metallic") && target_mat.HasProperty("_MetallicGlossMap") && target_mat.GetTexture("_MetallicGlossMap")!=null);
				if (_2layers) {
					if (_specSetup) {
						SaveToMaterialTex("_SpecGlossMap", "Spec/Gloss 1");
						SaveToMaterialTex("_ParallaxMap", "Height 1");
						SaveToMaterialTex("_SpecGlossMap2", "Spec/Gloss 2");
						SaveToMaterialTex("_ParallaxMap2", "Height 2");
					} else if (_metSetup) {
						SaveToMaterialTex("_MetallicGlossMap", "Metal/gloss 1");
						SaveToMaterialTex("_ParallaxMap", "Height 1");
						SaveToMaterialTex("_MetallicGlossMap2", "Metal/gloss 2");
						SaveToMaterialTex("_ParallaxMap2", "Height 2");
					} else {
						SaveToMaterialTex("_ParallaxMap", "Height 1");
						SaveToMaterialTex("_ParallaxMap2", "Height 2");
					}
				} else {
					if (_specSetup) {
						SaveToMaterialTex("_SpecGlossMap", "Spec/Gloss");
						SaveToMaterialTex("_SpecularRGBGlossADetail", "Detail Spec/Gloss");
					} else if (_metSetup) {
						SaveToMaterialTex("_MetallicGlossMap", "Metal/gloss");
						SaveToMaterialTex("_MetallicGlossMapDetail", "Detail Metal/Gloss");
					}
					SaveToMaterialTex("_ParallaxMap", "Height");
				}
				SaveToMaterialTex("_OcclusionMap", "Occlusion");
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
		}
	}

	void SaveToMaterialTex(string propName, string butName) {
		if (target_mat.HasProperty(propName) && target_mat.GetTexture(propName)) {
			string path=AssetDatabase.GetAssetPath(target_mat.GetTexture(propName));
			if (path!="" && GUILayout.Button(butName)) {
				directory=Path.GetDirectoryName(path);
				file=Path.GetFileNameWithoutExtension(path)+".png";
				SaveTexture (directory, file);
				Texture tex=AssetDatabase.LoadAssetAtPath<Texture>(save_path);
				if (tex) {
					target_mat.SetTexture(propName, tex);
				}
			}
		}
	}

	void GetFromMaterialTexProp(string propName, string butName) {
		if (target_mat.HasProperty(propName) && target_mat.GetTexture(propName)) {
			if (GUILayout.Button(butName)) {
				source_tex0=target_mat.GetTexture(propName) as Texture2D;
				sourceChannel0=UBERColorChannels.R;
				source_tex1=target_mat.GetTexture(propName) as Texture2D;
				sourceChannel1=UBERColorChannels.G;
				source_tex2=target_mat.GetTexture(propName) as Texture2D;
				sourceChannel2=UBERColorChannels.B;
				source_tex3=target_mat.GetTexture(propName) as Texture2D;
				sourceChannel3=UBERColorChannels.A;
			}
		}
	}
	
	void SaveTexture(string directory, string file) {
		string path = EditorUtility.SaveFilePanel("Save texture", directory, file, "png");
		if (path!="") {
			save_path=path=path.Substring(Application.dataPath.Length-6);
	 		byte[] bytes = rendered_tex.EncodeToPNG();
		    System.IO.File.WriteAllBytes(path, bytes);
			//AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			AssetImporter _importer=AssetImporter.GetAtPath(path);
			TextureImporter tex_importer=(TextureImporter)_importer;
			if (tex_importer) {
				tex_importer.sRGBTexture=!linearTexture;
			}
			AssetDatabase.ImportAsset(path,  ImportAssetOptions.ForceUpdate);
			if (target_mat==null) {
				finalize=true;
			}
			rendered_tex=null;
		}							
	}
				
	bool check_texture(Texture2D tex, int num, int w, int h) {
		if (!tex) return false;
		
		AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
		if (_importer) {
			TextureImporter tex_importer=(TextureImporter)_importer;
			if (!tex_importer.isReadable) {
				Debug.LogWarning("Texture ("+tex.name+") has been reimported as readable.");
				tex_importer.isReadable=true;
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tex),  ImportAssetOptions.ForceUpdate);
			}
		}			
		
		string path = AssetDatabase.GetAssetPath(tex);
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
		if (!textureImporter.isReadable) {
			EditorGUILayout.LabelField("Mark your source texture "+num+" as readable...");
			return false;
		}
		if (tex.width != w || tex.height != h) {
			EditorGUILayout.LabelField ("Source tex " + num + " must fit dimensions of other textures");
			sizeMismatch = true;
			return false;
		} else {
			sizeMismatch=false;
		}
		return true;
	}
				
	byte[] get_color_channel(Texture2D source_tex, UBERColorChannels sourceChannel) {
		Color32[] cols=source_tex.GetPixels32();
		byte[] ret=new byte[cols.Length];
		if (sourceChannel==UBERColorChannels.R) for(int i=0; i<cols.Length; i++) 	ret[i]=cols[i].r;
		else if (sourceChannel==UBERColorChannels.G) for(int i=0; i<cols.Length; i++) 	ret[i]=cols[i].g;
		else if (sourceChannel==UBERColorChannels.B) for(int i=0; i<cols.Length; i++) 	ret[i]=cols[i].b;
		else for(int i=0; i<cols.Length; i++) ret[i]=cols[i].a;
		return ret;
	}

	byte get_color_channel(Color col, UBERColorChannels sourceChannel) {
		byte ret=0;
		if (sourceChannel==UBERColorChannels.R) ret=(byte)Mathf.RoundToInt(col.r*255);
		else if (sourceChannel==UBERColorChannels.G) ret=(byte)Mathf.RoundToInt(col.g*255);
		else if (sourceChannel==UBERColorChannels.B) ret=(byte)Mathf.RoundToInt(col.b*255);
		else ret=(byte)Mathf.RoundToInt(col.a*255);
		return ret;
	}
}