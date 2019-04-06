using UnityEngine;
using System.Collections;

public enum UBER_PresetParamSection {
	All, Wetness, Glitter, Translucency, Snow
}

public class UBER_MaterialPresetCollection : ScriptableObject { 
	[SerializeField] [HideInInspector] public string currentPresetName;
	[SerializeField] [HideInInspector] public UBER_PresetParamSection whatToRestore=UBER_PresetParamSection.All;
	[SerializeField] [HideInInspector] public UBER_MaterialPreset[] matPresets;
	[SerializeField] [HideInInspector] public string[] names; // from matPresets[n].name for quick access in material inspector

#if UNITY_EDITOR
	public int PresetIndex(string name) {
		if (matPresets==null) return -1;
		for (int i=0; i<matPresets.Length; i++) {
			if (matPresets[i]!=null && matPresets[i].name==name) {
				return i;
			}
		}
		return -1;
	}

	public void AddPreset(Material mat, string name) { 
		int presetIndex=PresetIndex(name);
		if (presetIndex == -1) {
			// new preset
			int newCount;
			if (matPresets != null) {
				newCount = matPresets.Length + 1;
			} else {
				newCount = 1; 
			}
			UBER_MaterialPreset[] newMatPresets = new UBER_MaterialPreset[newCount];
			for (int i=0; i<newCount-1; i++) {
				newMatPresets [i] = matPresets [i];
			}
			newMatPresets[newCount - 1] = new UBER_MaterialPreset();
			newMatPresets[newCount - 1].name = name;
			newMatPresets[newCount - 1].SetProps(mat);
			// sort by name
			for(int i=0; i<newMatPresets.Length; i++) {
				bool changed=false;
				for(int j=i+1; j<newMatPresets.Length; j++) {
					if (string.CompareOrdinal(newMatPresets[i].name,newMatPresets[j].name)>0) {
						changed=true;
						UBER_MaterialPreset tmp_mat=newMatPresets[i];
						newMatPresets[i]=newMatPresets[j];
						newMatPresets[j]=tmp_mat;
					}
				}
				if (!changed) {
					// already sorted
					break;
				}
			}
			matPresets=newMatPresets;
		} else {
			// replace existing preset
			matPresets[presetIndex].SetProps(mat);
		}
		names=new string[matPresets.Length];
		for(int i=0; i<names.Length; i++) {
			names[i]=matPresets[i].name;
		}
	}

	public void RemovePreset(int presetIndex) {
		int currentPresetIndex = PresetIndex (currentPresetName);
		for (int i=presetIndex; i<matPresets.Length-1; i++) {
			matPresets[i]=matPresets[i+1];
			names[i]=names[i+1];
		}
		if (currentPresetIndex != -1) {
			if (currentPresetIndex==matPresets.Length-1) {
				currentPresetIndex=matPresets.Length-2;
			}
			if (currentPresetIndex!=-1) {
				currentPresetName=names[currentPresetIndex];
			} else {
				currentPresetName="";
			}
		}
		UBER_MaterialPreset[] newMatPresets = new UBER_MaterialPreset[matPresets.Length - 1];
		string[] newNames = new string[matPresets.Length - 1];
		for(int i=0; i<newNames.Length; i++) {
			newNames[i]=names[i];
			newMatPresets[i]=matPresets[i];
		}
		names = newNames;
		matPresets = newMatPresets;
	}
#endif
}
