using UnityEngine;
using System.Collections;

public class UBER_Global {
	public static bool initPresetCollection=true;
}

[AddComponentMenu("")] // N/A from component menu
public class UBER_MaterialPresetCollectionReference : MonoBehaviour {
	[HideInInspector] public UBER_MaterialPresetCollection collectionReference;

#if UNITY_EDITOR
	void OnValidate() {
		// reinit presetCollection between loaded scenes and scripts got recompiled
		UBER_Global.initPresetCollection = true;
	}
#endif

}