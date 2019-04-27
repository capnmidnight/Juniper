using UnityEngine;
using UnityEditor;
using System.Collections;
using SimpleMatrixLibrary; 

// not used now, w/o recursive it works good enough and recursive curvature serach is very slow
//#define RECURSIVE_VERTICES_FLAG

//
// prepare POM object
//
// 1. texture to object space ratio and curvature when model has NAMING_STRING in its name
// additionaly we can extrude model using NAMING_EXTRUDE in name, using NAMING_BOTTOM will addtionally cap the bottom of extruded volume
//
// 2. we can prepare volume from existing model via vertex positions (use NAMING_VOLUME string in model/mesh name)
public class UBER_PostProcessModelForPOM : AssetPostprocessor {
	const string NAMING_VOLUME="POM_Volume_";
	const string NAMING_STRING="POM_Baked";
	const string NAMING_EXTRUDE="POM_Extrude_";
	const string NAMING_BOTTOM="BOTTOM";
	//const bool _recursive_vertices_flag=false; // 

	static bool makeBottom; // default - false
	static float extrudeHeight; // default - 0.1
	static int extrusion_color_channel; // default - alpha

	static float volumeRatioU; // default - 10.0 (like on Unity's plane primitive)
	static float volumeRatioV; // default - 10.0 (like on Unity's plane primitive)

	static GameObject go;
	static Mesh mesh;
	static bool is_being_rendered;
	static int current_vertex;
	static int submesh_num=0;
	
	static Vector3[] vertices;
	static Vector3[] normals;
	static Vector2[] uvs;
	static Vector2[] uvs4; // output ratio/curvature (packed)
	static Vector4[] tangents;
	static Vector2[] uvs_scl;
	static Vector2[] curvature;
	static bool[] inside_submesh_set;
	static int[] triangles;

	public static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		for (int i=0; i<importedAssets.Length; i++) {
			mesh = AssetDatabase.LoadAssetAtPath (importedAssets[i], typeof(Mesh)) as Mesh;
			if (mesh && mesh.name.IndexOf (NAMING_STRING) >= 0) {
				Debug.Log ("Processing mesh=" + mesh.name);
				Make();
				if (mesh && mesh.name.IndexOf (NAMING_EXTRUDE) >= 0) {
					TakeExtrusionProps(mesh.name);
					Debug.Log ("Extruding mesh=" + mesh.name);
					ExtrudeMesh();
				}
			}
			if (mesh && mesh.name.IndexOf (NAMING_VOLUME) >= 0) {
				Debug.Log ("Processing POM volume mesh=" + mesh.name);
				TakeVolumeProps(mesh.name);
				PrepareVolumeMesh();
			}
		}
    }

    public void OnPostprocessModel (GameObject g) {
		if (g.name.IndexOf(NAMING_STRING)>=0) {
			go=g;
			PrepareMesh(go.transform);
		} else if (g.name.IndexOf(NAMING_VOLUME)>=0) {
			go=g;
			PrepareVolume(go.transform);
		}
	}

	private static void Make() {
		for(int j=0; j<mesh.subMeshCount; j++) {
			if (mesh.subMeshCount>1) {
				Debug.Log ("  submesh #"+j);
			}
			submesh_num=j;
			Start_Rendering();
		    while(is_being_rendered) {
		    	buildCurvatureInfo();
		    }
		}
    }

    private void PrepareMesh(Transform t) {
		MeshFilter mf=t.GetComponent<MeshFilter>();
		if (mf) {
			mesh=mf.sharedMesh;
			if (mesh) {
				Debug.Log ("Processing POM data for game object="+go.name+", mesh="+mesh.name);
				Make();
				if (go.name.IndexOf(NAMING_EXTRUDE)>=0) {
					Debug.Log ("Extruding mesh for game object="+go.name+", mesh="+mesh.name);
					TakeExtrusionProps(go.name);
					ExtrudeMesh();
				}
			}
		}
		for(int i=0; i<t.childCount; i++) {
			PrepareMesh(t.GetChild(i));
		}
	}

	private void PrepareVolume(Transform t) {
		MeshFilter mf=t.GetComponent<MeshFilter>();
		if (mf) {
			mesh=mf.sharedMesh;
			if (mesh) {
				Debug.Log ("Processing POM volume for game object="+go.name+", mesh="+mesh.name);
				TakeVolumeProps(go.name);
				PrepareVolumeMesh();
			}
		}
		for(int i=0; i<t.childCount; i++) {
			PrepareVolume(t.GetChild(i));
		}
	}

	static void TakeExtrusionProps(string name) {
		makeBottom=false;
		extrudeHeight = 0f;
		extrusion_color_channel = 3;

		int s = name.IndexOf (NAMING_EXTRUDE) + NAMING_EXTRUDE.Length - 1;
		makeBottom=name.IndexOf (NAMING_BOTTOM, s) >= 0;
		if (name.Substring (s, 1).ToUpper() == "R") {
			extrusion_color_channel=0;
		} else if (name.Substring (s, 1).ToUpper() == "G") {
			extrusion_color_channel=1;
		} else if (name.Substring (s, 1).ToUpper() == "B") {
			extrusion_color_channel=2;
		} else if (name.Substring (s, 1).ToUpper() == "A") {
			extrusion_color_channel=3;
		}
		// find extrusion value
		int sIdx = name.Length;
		int eIdx = name.Length;
		for(int i=s; i<name.Length; i++) {
			string c=name.Substring(i,1);
			if (c=="0" || c=="1" || c=="2" || c=="3" || c=="4" || c=="5" || c=="6" || c=="7" || c=="8" || c=="9" || c==".") {
				sIdx=i; break;
			}
		}
		for(int i=sIdx; i<name.Length; i++) {
			string c=name.Substring(i,1);
			if (c!="0" && c!="1" && c!="2" && c!="3" && c!="4" && c!="5" && c!="6" && c!="7" && c!="8" && c!="9" && c!=".") {
				eIdx=i; break;
			}
		}
		string extVal = name.Substring (sIdx, eIdx - sIdx);
		float val;
		if (float.TryParse(extVal, out val)) {
			extrudeHeight=val;
		}
		//Debug.Log (extrusion_color_channel + " " + extrudeHeight + " " + makeBottom);
	}

	static void TakeVolumeProps(string name) {
		volumeRatioU = 10f;
		volumeRatioV = 10f;
		extrusion_color_channel = 3;
		
		int s = name.IndexOf (NAMING_VOLUME) + NAMING_VOLUME.Length - 1;
		if (name.Substring (s, 1).ToUpper() == "R") {
			extrusion_color_channel=0;
		} else if (name.Substring (s, 1).ToUpper() == "G") {
			extrusion_color_channel=1;
		} else if (name.Substring (s, 1).ToUpper() == "B") {
			extrusion_color_channel=2;
		} else if (name.Substring (s, 1).ToUpper() == "A") {
			extrusion_color_channel=3;
		}
		// find volumeRatioU value
		int sIdx = name.Length;
		int eIdx = name.Length;
		for(int i=s; i<name.Length; i++) {
			string c=name.Substring(i,1);
			if (c=="0" || c=="1" || c=="2" || c=="3" || c=="4" || c=="5" || c=="6" || c=="7" || c=="8" || c=="9" || c==".") {
				sIdx=i; break;
			}
		}
		for(int i=sIdx; i<name.Length; i++) {
			string c=name.Substring(i,1);
			if (c!="0" && c!="1" && c!="2" && c!="3" && c!="4" && c!="5" && c!="6" && c!="7" && c!="8" && c!="9" && c!=".") {
				eIdx=i; break;
			}
		}
		string extVal = name.Substring (sIdx, eIdx - sIdx);
		float val;
		if (float.TryParse(extVal, out val)) {
			volumeRatioU=val;
		}
		if (sIdx >= name.Length) {
			// missing volumeRatioU, don't search fo volumeRatioV
			return;
		}
		sIdx=eIdx+1;
		eIdx = name.Length;
		for(int i=s; i<name.Length; i++) {
			string c=name.Substring(i,1);
			if (c=="0" || c=="1" || c=="2" || c=="3" || c=="4" || c=="5" || c=="6" || c=="7" || c=="8" || c=="9" || c==".") {
				sIdx=i; break;
			}
		}
		for(int i=sIdx; i<name.Length; i++) {
			string c=name.Substring(i,1);
			if (c!="0" && c!="1" && c!="2" && c!="3" && c!="4" && c!="5" && c!="6" && c!="7" && c!="8" && c!="9" && c!=".") {
				eIdx=i; break;
			}
		}
		extVal = name.Substring (sIdx, eIdx - sIdx);
		if (float.TryParse(extVal, out val)) {
			volumeRatioV=val;
		}

		//Debug.Log (extrusion_color_channel + " " + volumeRatioU + " " + volumeRatioV);
	}

	static void Cancel_Rendering() {
		is_being_rendered=false;
	}
	
	static void Start_Rendering() {
		is_being_rendered=true;
		current_vertex=0;
		vertices=mesh.vertices;
		normals=mesh.normals;
		tangents=mesh.tangents;
		uvs=mesh.uv;
		if (normals==null || normals.Length==0 || tangents==null || tangents.Length==0 ||  uvs==null || uvs.Length==0) {
			Cancel_Rendering();
			Debug.LogError("Mesh of selected GameObject need normals & tangents & uvs for curvature computation !");
			return;
		}
		uvs4=new Vector2[vertices.Length];
		if (mesh.subMeshCount>1) {
			triangles=mesh.GetTriangles(submesh_num);
		} else {
			triangles=mesh.triangles;
		}
		inside_submesh_set=new bool[vertices.Length];
		for(int i=0; i<inside_submesh_set.Length; i++) {
			for(int j=0; j<triangles.Length; j++) {
				if (triangles[j]==i) {
					inside_submesh_set[i]=true;
					break;
				}
			}
		}
		uvs_scl=new Vector2[vertices.Length];
		curvature=new Vector2[vertices.Length];
		buildCurvatureInfo();
	}

	static void buildCurvatureInfo() {
		if (inside_submesh_set[current_vertex]) {
			ArrayList adjanced_vertices_idx=new ArrayList();
			for(int i=0; i<triangles.Length;) {
				int i1=triangles[i++];
				int i2=triangles[i++];
				int i3=triangles[i++];
				if (is_common_vertex(i1,current_vertex)) {
					if (!adjanced_vertices_idx.Contains(i2)) adjanced_vertices_idx.Add(i2);
					if (!adjanced_vertices_idx.Contains(i3)) adjanced_vertices_idx.Add(i3);
				} else if (is_common_vertex(i2,current_vertex)) {
					if (!adjanced_vertices_idx.Contains(i1)) adjanced_vertices_idx.Add(i1);
					if (!adjanced_vertices_idx.Contains(i3)) adjanced_vertices_idx.Add(i3);
				} else if (is_common_vertex(i3,current_vertex)) {
					if (!adjanced_vertices_idx.Contains(i1)) adjanced_vertices_idx.Add(i1);
					if (!adjanced_vertices_idx.Contains(i2)) adjanced_vertices_idx.Add(i2);
				}
			}
			#if RECURSIVE_VERTICES_FLAG
				ArrayList adjanced_vertices_idx2=new ArrayList();
				for(int k=0; k<adjanced_vertices_idx.Count; k++) {
					for(int i=0; i<triangles.Length;) {
						int i1=triangles[i++];
						int i2=triangles[i++];
						int i3=triangles[i++];
						if (is_common_vertex(i1,(int)adjanced_vertices_idx[k])) {
							if (!adjanced_vertices_idx2.Contains(i2)) adjanced_vertices_idx2.Add(i2);
							if (!adjanced_vertices_idx2.Contains(i3)) adjanced_vertices_idx2.Add(i3);
						} else if (is_common_vertex(i2,(int)adjanced_vertices_idx[k])) {
							if (!adjanced_vertices_idx2.Contains(i1)) adjanced_vertices_idx2.Add(i1);
							if (!adjanced_vertices_idx2.Contains(i3)) adjanced_vertices_idx2.Add(i3);
						} else if (is_common_vertex(i3,(int)adjanced_vertices_idx[k])) {
							if (!adjanced_vertices_idx2.Contains(i1)) adjanced_vertices_idx2.Add(i1);
							if (!adjanced_vertices_idx2.Contains(i2)) adjanced_vertices_idx2.Add(i2);
						}
					}
				}
				adjanced_vertices_idx=adjanced_vertices_idx2;
			#endif
			Vector3 norm=normals[current_vertex];
			Vector3 tangent=new Vector3(tangents[current_vertex].x, tangents[current_vertex].y, tangents[current_vertex].z);
			Vector3 binorm=Vector3.Cross(norm, tangent) * tangents[current_vertex].w;
			Matrix4x4 tR =Matrix4x4.identity;
			tR.SetRow(0, new Vector4(tangent.x, tangent.y,tangent.z,0));
			tR.SetRow(1, new Vector4(binorm.x, binorm.y, binorm.z,0));
			tR.SetRow(2, new Vector4(norm.x, norm.y, norm.z,0));
			Vector3[] adjanced_vertices=new Vector3[adjanced_vertices_idx.Count+1];
			adjanced_vertices[0]=Vector3.zero;
			for(int i=1; i<=adjanced_vertices_idx.Count; i++) {
				adjanced_vertices[i]=tR.MultiplyPoint3x4( ( vertices[ (int)adjanced_vertices_idx[i-1] ] - vertices[current_vertex] ) );
			}
			float scl_u=1;
			float scl_v=1;
			for(int i=0; i<triangles.Length; ) {
				int i1=triangles[i++];
				int i2=triangles[i++];
				int i3=triangles[i++];
				bool process_flag=false;
				if (i1==current_vertex) {
					process_flag=true;
				} else if (i2==current_vertex) {
					i2=i1;
					process_flag=true;
				} else if (i3==current_vertex) {
					i3=i1;
					process_flag=true;
				}
				if (process_flag) {
					Vector2 scl=get_UV2ObjectScale(current_vertex, i2, i3);
					scl_u=scl.x;
					scl_v=scl.y;
					break;
				}
			}
			
			SimpleMatrix M=new SimpleMatrix(adjanced_vertices.Length,2);
			SimpleMatrix b=new SimpleMatrix(adjanced_vertices.Length,1);
			for(int i=0; i<adjanced_vertices.Length; i++) {
				M[i, 0]=adjanced_vertices[i].x*adjanced_vertices[i].x;
				M[i, 1]=adjanced_vertices[i].y*adjanced_vertices[i].y;
				b[i,0]=-adjanced_vertices[i].z;
			}
			
			SimpleMatrix M_t=SimpleMatrix.Transpose(M);
			SimpleMatrix M_inv=(M_t*M).Invert();
			SimpleMatrix coeffs=(M_inv*M_t)*b;
			curvature[current_vertex].x=(float)coeffs[0,0];
			curvature[current_vertex].y=(float)coeffs[1,0];
//			curvature[current_vertex].x=Mathf.Abs(curvature[current_vertex].x)>128 ? Mathf.Sign(curvature[current_vertex].x)*128 : curvature[current_vertex].x;
//			curvature[current_vertex].y=Mathf.Abs(curvature[current_vertex].y)>128 ? Mathf.Sign(curvature[current_vertex].y)*128 : curvature[current_vertex].y;
//			scl_u=scl_u > 255 ? 255 : scl_u;
//			scl_v=scl_v > 255 ? 255 : scl_v;
			uvs_scl[current_vertex].x=scl_u;
			uvs_scl[current_vertex].y=scl_v;
		}
		current_vertex++;
		
		if (current_vertex==vertices.Length) {
			for(int i=0; i<vertices.Length; i++) {
				if (inside_submesh_set[i]) {
//					Debug.Log(uvs_scl[i] +"   "+curvature[i]);
//					Debug.Log((Mathf.Round(uvs_scl[i].x*100)/100)+", "+(Mathf.Round(uvs_scl[i].y*100)/100)+"   "+(Mathf.Round(curvature[i].x*100)/100)+","+(Mathf.Round(curvature[i].y*100)/100));
					float CurvU=Mathf.Clamp(curvature[i].x, -19.9f, 19.9f);
					float CurvV=Mathf.Clamp(curvature[i].y, -19.9f, 19.9f);
					CurvU=CurvU/20.0f+0.5f; // curvature - stored in frac term
					CurvV=CurvV/20.0f+0.5f;
                    float SclU=Mathf.Floor(uvs_scl[i].x*100); // scale is always positive
					float SclV=Mathf.Floor(uvs_scl[i].y*100);
					uvs4[i].x=SclU+CurvU;
					uvs4[i].y=SclV+CurvV;

//					SclU=Mathf.Floor(uvs4[i].x);
//					CurvU=uvs4[i].x-SclU;
//					SclU/=100;
//					CurvU=CurvU*20-10;
//                    
//					SclV=Mathf.Floor(uvs4[i].y);
//					CurvV=uvs4[i].y-SclV;
//					SclV/=100;
//					CurvV=CurvV*20-10;
//
//					Debug.Log((Mathf.Round(SclU*100)/100)+", "+(Mathf.Round(SclV*100)/100)+"   "+(Mathf.Round(CurvU*100)/100)+","+(Mathf.Round(CurvV*100)/100));
                    
                }
			}
			//mesh.uv3=uvs3; // reserved for dyn lightmaps, we can't use it
			// so, pack it into float2
			mesh.uv4=uvs4;
			is_being_rendered=false;
			//Repaint();
		}
	}
	
	private static bool is_common_vertex(int i1, int i2) {
		return (Vector3.Distance(vertices[i1], vertices[i2])<0.0005f && Vector3.Angle(normals[i1], normals[i2])<15.0f);
	}
	
	private static Vector2 get_UV2ObjectScale(int i1, int i2, int i3) {
		float u1,u2,u3;
		float v1,v2,v3;
		u1=uvs[i1].x; u2=uvs[i2].x; u3=uvs[i3].x;
		v1=uvs[i1].y; v2=uvs[i2].y; v3=uvs[i3].y;
		Vector3 p1=vertices[i1];
		Vector3 p2=vertices[i2];
		Vector3 p3=vertices[i3];
		Vector2 scale=new Vector2();
		scale.x=Vector3.Magnitude(( (v3 - v1)*(p2 - p1) - (v2 - v1)*(p3 - p1) ) / ( (u2 - u1)*(v3 - v1) - (v2 - v1)*(u3 - u1) ));
		scale.y=Vector3.Magnitude(( (u3 - u1)*(p2 - p1) - (u2 - u1)*(p3 - p1) ) / ( (v2 - v1)*(u3 - u1) - (u2 - u1)*(v3 - v1) ));
		return scale;
	}

	private static void ExtrudeMesh() {
		Vector3[] vertices=mesh.vertices;
		int[] triangles=mesh.triangles;
		ArrayList outer_edges=new ArrayList();
		for(int i=0; i<triangles.Length; i+=3) {
			if (check_edge(triangles[i], triangles[i+1], i, triangles)) { outer_edges.Add(triangles[i]); outer_edges.Add(triangles[i+1]); }
			if (check_edge(triangles[i+1], triangles[i+2], i, triangles)) { outer_edges.Add(triangles[i+1]); outer_edges.Add(triangles[i+2]); }
			if (check_edge(triangles[i+2], triangles[i], i, triangles)) { outer_edges.Add(triangles[i+2]); outer_edges.Add(triangles[i]); }
		}
		if (outer_edges.Count==0) {
			Debug.LogError("Can't extrude mesh "+mesh.name+"! No outer edges found...");
			return;
		}
		int[] _outer_edges=(int[])outer_edges.ToArray(typeof(int));
		ArrayList new_vertices=new ArrayList();
		for(int i=0; i<outer_edges.Count; i++) {
			if (new_vertices.IndexOf(outer_edges[i])<0) new_vertices.Add(outer_edges[i]);
		}
		int[] _new_vertices=(int[])new_vertices.ToArray(typeof(int));
		
		Color32[] colors=mesh.colors32;		
		Vector3[] normals=mesh.normals;
		Vector4[] tangents=mesh.tangents;
		Vector2[] uv=mesh.uv;
		Vector2[] uv2=mesh.uv2;
		Vector2[] uv3=mesh.uv3;
		Vector2[] uv4=mesh.uv4;
		Vector3[] normals_extruded=new Vector3[normals.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		Vector3[] vertices_extruded=new Vector3[vertices.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		Vector4[] tangents_extruded=new Vector4[tangents.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		Vector2[] uv_extruded=new Vector2[uv.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		Vector2[] uv2_extruded=new Vector2[uv2.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		Vector2[] uv3_extruded=new Vector2[uv3.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		Vector2[] uv4_extruded=new Vector2[uv4.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		Color32[] colors_extruded=new Color32[vertices.Length*(makeBottom ? 2 : 1) + _new_vertices.Length];
		
//		// average scale tangent units / object space units
//		float UVdist=0.0001f;
//		float objectSpaceDist=0.0001f;
//		for(int i=0; i<triangles.Length; i+=3) {
//			UVdist+=Vector2.Distance(uv[triangles[i]], uv[triangles[i+1]]);
//			UVdist+=Vector2.Distance(uv[triangles[i+1]], uv[triangles[i+2]]);
//			UVdist+=Vector2.Distance(uv[triangles[i]], uv[triangles[i+2]]);
//			objectSpaceDist+=Vector3.Distance(vertices[triangles[i]], vertices[triangles[i+1]]);
//			objectSpaceDist+=Vector3.Distance(vertices[triangles[i+1]], vertices[triangles[i+2]]);
//			objectSpaceDist+=Vector3.Distance(vertices[triangles[i]], vertices[triangles[i+2]]);
//		}
//		float UV2objectSpaceRatio=objectSpaceDist/UVdist;
//		float extrudeHeight=extrudeHeight*UV2objectSpaceRatio;

		Vector3[] vertices_extruded_tmp=new Vector3[vertices_extruded.Length]; // temporary vertices with extruded volume (to recalc some initial bounds)
		for(int i=0; i<vertices.Length; i++) { 
			vertices_extruded[i]=vertices[i];
			vertices_extruded_tmp[i]=vertices[i];
			if (makeBottom) vertices_extruded[i+vertices.Length]=vertices[i]-extrudeHeight*normals[i];
			if (makeBottom) vertices_extruded_tmp[i+vertices.Length]=vertices[i]-1*normals[i];
			Color32 col;
			if (i<colors.Length) col=colors[i]; else col=new Color32();
			colors_extruded[i]=col;
			switch(extrusion_color_channel) {
				case 0: colors_extruded[i].r=0; break;
				case 1: colors_extruded[i].g=0; break;
				case 2: colors_extruded[i].b=0; break;
				case 3: colors_extruded[i].a=0; break;
				default: colors_extruded[i].a=0; break;
			}
			if (makeBottom) {
				colors_extruded[i+vertices.Length]=col;
				switch(extrusion_color_channel) {
					case 0: colors_extruded[i+vertices.Length].r=255; break;
					case 1: colors_extruded[i+vertices.Length].g=255; break;
					case 2: colors_extruded[i+vertices.Length].b=255; break;
					case 3: colors_extruded[i+vertices.Length].a=255; break;
					default: colors_extruded[i+vertices.Length].a=255; break;
				}
			}
		}
		int index_offset=vertices.Length*(makeBottom ? 2 : 1);
		for(int i=0; i<_new_vertices.Length; i++) {
			vertices_extruded[i+index_offset]=vertices[_new_vertices[i]];
			vertices_extruded_tmp[i+index_offset]=vertices[_new_vertices[i]];
			Color32 col;
			if (_new_vertices[i]<colors.Length) col=colors[_new_vertices[i]]; else col=new Color32();
			colors_extruded[i+index_offset]=col; 
			switch(extrusion_color_channel) {
				case 0: colors_extruded[i+index_offset].r=0; break;
				case 1: colors_extruded[i+index_offset].g=0; break;
				case 2: colors_extruded[i+index_offset].b=0; break;
				case 3: colors_extruded[i+index_offset].a=0; break;
				default: colors_extruded[i+index_offset].a=0; break;
			}
		}
		
		for(int i=0; i<normals.Length; i++) {
			normals_extruded[i]=normals[i];
			if (makeBottom) normals_extruded[i+vertices.Length]=-normals[i];
		}
		index_offset=normals.Length*(makeBottom ? 2 : 1);
		for(int i=0; i<_new_vertices.Length; i++) {
			normals_extruded[i+index_offset]=-normals[_new_vertices[i]];
		}
		
		for(int i=0; i<tangents.Length; i++) {
			tangents_extruded[i]=tangents[i];
			if (makeBottom) tangents_extruded[i+vertices.Length]=new Vector4(tangents[i].x, tangents[i].y, tangents[i].z, -tangents[i].w);
		}
		index_offset=tangents.Length*(makeBottom ? 2 : 1);
		for(int i=0; i<_new_vertices.Length; i++) {
			tangents_extruded[i+index_offset]=new Vector4(tangents[_new_vertices[i]].x, tangents[_new_vertices[i]].y, tangents[_new_vertices[i]].z, -tangents[_new_vertices[i]].w);
		}
		
		for(int i=0; i<uv.Length; i++) {
			uv_extruded[i]=uv[i];
			if (makeBottom) uv_extruded[i+vertices.Length]=uv[i];
		}
		index_offset=uv.Length*(makeBottom ? 2 : 1);
		for(int i=0; i<_new_vertices.Length; i++) {
			uv_extruded[i+index_offset]=uv[_new_vertices[i]];
		}
		
		if (uv2.Length>0) {
			for(int i=0; i<uv2.Length; i++) {
				uv2_extruded[i]=uv2[i];
				if (makeBottom) uv2_extruded[i+vertices.Length]=uv2[i];
			}
			index_offset=uv2.Length*(makeBottom ? 2 : 1);
			for(int i=0; i<_new_vertices.Length; i++) {
				uv2_extruded[i+index_offset]=uv2[_new_vertices[i]];
			}
		}

		if (uv3.Length>0) {
			for(int i=0; i<uv3.Length; i++) {
				uv3_extruded[i]=uv3[i];
				if (makeBottom) uv3_extruded[i+vertices.Length]=uv3[i];
			}
			index_offset=uv3.Length*(makeBottom ? 2 : 1);
			for(int i=0; i<_new_vertices.Length; i++) {
				uv3_extruded[i+index_offset]=uv3[_new_vertices[i]];
			}
		}

		if (uv4.Length>0) {
			for(int i=0; i<uv4.Length; i++) {
				uv4_extruded[i]=uv4[i];
				uv4_extruded[i].x=Mathf.Floor(uv4_extruded[i].x)+0.5f; // extruded top flag (curvature!=0)
				if (makeBottom) {
					uv4_extruded[i+vertices.Length]=uv4[i];
					uv4_extruded[i+vertices.Length].x=Mathf.Floor(uv4_extruded[i+vertices.Length].x)+0f; // extruded bottom flag (curvature=0)
				}
			}
			index_offset=uv4.Length*(makeBottom ? 2 : 1);
			for(int i=0; i<_new_vertices.Length; i++) {
				uv4_extruded[i+index_offset]=uv4[_new_vertices[i]];
				uv4_extruded[i+index_offset].x=Mathf.Floor(uv4_extruded[i+index_offset].x)+0f; // extruded bottom flag (curvature=0)
			}
		}

		int[] triangles_extruded=new int[triangles.Length*(makeBottom ? 2 : 1) + _outer_edges.Length*3];
		for(int i=0; i<triangles.Length; i+=3) {
			triangles_extruded[i]=triangles[i];
			triangles_extruded[i+1]=triangles[i+1];
			triangles_extruded[i+2]=triangles[i+2];
			if (makeBottom) {
				triangles_extruded[i+triangles.Length]=triangles[i+2]+vertices.Length;
				triangles_extruded[i+triangles.Length+1]=triangles[i+1]+vertices.Length;
				triangles_extruded[i+triangles.Length+2]=triangles[i]+vertices.Length;
			}
		}
		index_offset=triangles.Length*(makeBottom ? 2 : 1); 
		int index_offset2=vertices.Length*(makeBottom ? 2 : 1);
		for(int i=0; i<_outer_edges.Length; i+=2) {
			triangles_extruded[index_offset]=_outer_edges[i]+vertices.Length;
			triangles_extruded[index_offset+1]=_outer_edges[i+1]+vertices.Length;
			for(int j=0; j<_new_vertices.Length; j++) {
				if (_new_vertices[j]==_outer_edges[i]) {
					triangles_extruded[index_offset+2]=j+index_offset2;
					break; 
				}
			}
			index_offset+=3;
			triangles_extruded[index_offset]=triangles_extruded[index_offset-1];
			triangles_extruded[index_offset+1]=_outer_edges[i+1]+vertices.Length;
			for(int j=0; j<_new_vertices.Length; j++) {
				if (_new_vertices[j]==_outer_edges[i+1]) {
					triangles_extruded[index_offset+2]=j+index_offset2;
					break;
				}
			}
			index_offset+=3;			
		}
		
		if (extrudeHeight==0) {
			mesh.vertices=vertices_extruded_tmp; // temporary vertices with extruded volume
			mesh.RecalculateBounds();
            mesh.vertices=vertices_extruded;
            mesh.tangents=tangents_extruded;
			mesh.uv=uv_extruded;
			mesh.colors32=colors_extruded;
			mesh.triangles=triangles_extruded;

//			mesh.RecalculateNormals(); // recalc to get normals from geometry
//			Vector3[] realNormals=mesh.normals;
//			for(int j=0; j<normals_extruded.Length; j++) {
//				Vector3 binormal=Vector3.Cross(normals_extruded[j], new Vector3(tangents_extruded[j].x, tangents_extruded[j].y, tangents_extruded[j].z))*tangents_extruded[j].w;
//				Matrix4x4 tanBasis=new Matrix4x4();
//				tanBasis.SetRow(0, new Vector4(tangents_extruded[j].x, tangents_extruded[j].y, tangents_extruded[j].z, 0));
//				tanBasis.SetRow(1, new Vector4(binormal.x, binormal.y, binormal.z, 0));
//				tanBasis.SetRow(2, new Vector4(normals_extruded[j].x, normals_extruded[j].y, normals_extruded[j].z, 0));
//				Vector4 realNormalInTanSpace4=tanBasis.MultiplyVector(realNormals[j]);
//				Vector3 realNormalInTanSpace=Vector3.Normalize(new Vector3(realNormalInTanSpace4.x, realNormalInTanSpace4.y, realNormalInTanSpace4.z));
//				// encode normal in tangent space at "start point"
//				// for ceil it's supposed to be (0,0,1), for extruded sidewalls we need to encode initial normal state
//				if (uv4.Length>0) {
//					uv4_extruded[j].x = Mathf.Floor(uv4_extruded[j].x) + (realNormalInTanSpace.x*0.5f+0.5f)*0.999f; // uv tan2object scale + norm.x encoded
//					uv4_extruded[j].y = Mathf.Floor(uv4_extruded[j].y) + (realNormalInTanSpace.y*0.5f+0.5f)*0.999f; // uv tan2object scale + norm.y encoded
//				}
//			}
			mesh.normals=normals_extruded; // set correct normals

			if (uv2.Length>0) mesh.uv2=uv2_extruded;
			if (uv3.Length>0) mesh.uv3=uv3_extruded;
			if (uv4.Length>0) mesh.uv4=uv4_extruded;
		} else {
			// we're extracting collider only - vertices + triangles buffer is enough
			mesh.vertices=vertices_extruded;
			mesh.normals=null;
			mesh.tangents=null;
			mesh.uv=null;
			mesh.uv2=null;
			mesh.uv3=null;
			mesh.uv4=null;
			mesh.colors32=null;
//			// flip triangles to correct normals for collider convex hull
//			for(int i=0; i<triangles_extruded.Length; i+=3) {
//				int _t=triangles_extruded[i];
//				triangles_extruded[i]=triangles_extruded[i+2];
//				triangles_extruded[i+2]=_t;
//			}
			mesh.triangles=triangles_extruded;
			mesh.RecalculateBounds();
		}


		;
	}
	
	static void PrepareVolumeMesh() {
		vertices=mesh.vertices;
//		for (int i=1; i<vertices.Length; i++) { 
//			vertices[i].y-=1;
//			vertices[i].y*=Mathf.Clamp01(Vector2.Distance(new Vector2(vertices[i].x, vertices[i].z), Vector2.zero)/5);
//		}

		normals=new Vector3[vertices.Length]; // constant (0,1,0)
		tangents=new Vector4[vertices.Length]; // constant (1,0,0,1)
		uvs=new Vector2[vertices.Length]; // depends on vertices xz
		uvs4=new Vector2[vertices.Length]; // here we encode ratio (curvature=0)
		Color32[] colors=mesh.colors32;		
		Color32[] colors_extruded=new Color32[vertices.Length]; // extrusion_color_channel will get 0..1 normalized values of volume tangent Z position

		float yMin = vertices [0].y;
		float yMax = vertices [0].y;
		for (int i=1; i<vertices.Length; i++) { 
			if (vertices[i].y>yMax) {
				yMax = vertices[i].y;
			} else if (vertices[i].y<yMin) {
				yMin = vertices[i].y;
			}
		}

		for(int i=0; i<vertices.Length; i++) { 
			uvs[i].x=-vertices[i].x/volumeRatioU;
			uvs[i].y=-vertices[i].z/volumeRatioV;
			normals[i]=Vector3.up;
			tangents[i]=new Vector4(-1,0,0,-1);

			Color32 col;
			if (i<colors.Length) col=colors[i]; else col=new Color32();
			colors_extruded[i]=col;
			byte tangentZ=(byte)(Mathf.Clamp01 ((vertices[i].y-yMin)/(yMax-yMin)) * 255);
			switch(extrusion_color_channel) {
				case 0: colors_extruded[i].r=tangentZ; break;
				case 1: colors_extruded[i].g=tangentZ; break;
				case 2: colors_extruded[i].b=tangentZ; break;
				case 3: colors_extruded[i].a=tangentZ; break;
				default: colors_extruded[i].a=tangentZ; break;
			}

			float SclU=Mathf.Floor(volumeRatioU*100);
			float SclV=Mathf.Floor(volumeRatioV*100);
			float CurvU=0*0.5f+0.5f; // curv - zero
			float CurvV=0*0.5f+0.5f;
			uvs4[i].x=SclU+CurvU;
			uvs4[i].y=SclV+CurvV;
		}

//		mesh.vertices=vertices;
		mesh.normals=normals;
		mesh.tangents=tangents;
		mesh.uv=uvs;
		mesh.uv4=uvs4;
		mesh.colors32=colors_extruded;
	}

	private static bool check_edge(int ev0, int ev1, int i, int[] triangles) {
		int eV0;
		int eV1;
		
		for(int j=0; j<triangles.Length; j+=3) {
			if (i!=j) {
				eV0=triangles[j]; eV1=triangles[j+1];
				if (((eV0==ev0) && (eV1==ev1)) || ((eV1==ev0) && (eV0==ev1))) return false;
				eV0=triangles[j+1]; eV1=triangles[j+2];
				if (((eV0==ev0) && (eV1==ev1)) || ((eV1==ev0) && (eV0==ev1))) return false;
				eV0=triangles[j+2]; eV1=triangles[j];
				if (((eV0==ev0) && (eV1==ev1)) || ((eV1==ev0) && (eV0==ev1))) return false;
			}
		}
		return true;		
	}

}