using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
public class UBER_MaterialProp {
	#if UNITY_EDITOR
	[SerializeField] public ShaderUtil.ShaderPropertyType type;
	#endif
	[SerializeField] public string name;
	[SerializeField] public float floatValue;
	[SerializeField] public Color colorValue;
	[SerializeField] public Vector4 vectorValue;
	[SerializeField] public Texture textureValue;
	[SerializeField] public Vector2 textureOffset;
	[SerializeField] public Vector2 textureScale;
}

[System.Serializable]
public class UBER_MaterialPreset {
	[SerializeField] public string name;
	[SerializeField] public Shader shader;
	[SerializeField] public UBER_MaterialProp[] props;

#if UNITY_EDITOR
	public UBER_MaterialProp[] GetMaterialProps(Material mat) {
		if (mat == null)
			return null;

		int cnt=ShaderUtil.GetPropertyCount(mat.shader);
		if (cnt == 0)
			return null;
		UBER_MaterialProp[] props=new UBER_MaterialProp[cnt];

		for (int i=0; i<cnt; i++) {
			props[i]=new UBER_MaterialProp();
			props[i].name=ShaderUtil.GetPropertyName(mat.shader, i);
			ShaderUtil.ShaderPropertyType propType=ShaderUtil.GetPropertyType(mat.shader, i);
			props[i].type=propType;

			if (propType==ShaderUtil.ShaderPropertyType.Color) {
				props[i].colorValue=mat.GetColor(props[i].name);
			} else if (propType==ShaderUtil.ShaderPropertyType.Float || propType==ShaderUtil.ShaderPropertyType.Range) {
				props[i].floatValue=mat.GetFloat(props[i].name);
			} else if (ShaderUtil.GetPropertyType(mat.shader, i)==ShaderUtil.ShaderPropertyType.Vector) {
				props[i].vectorValue=mat.GetVector(props[i].name);
			} else if (ShaderUtil.GetPropertyType(mat.shader, i)==ShaderUtil.ShaderPropertyType.TexEnv) {
				props[i].textureValue=mat.GetTexture(props[i].name);
				props[i].textureOffset=mat.GetTextureOffset(props[i].name);
				props[i].textureScale=mat.GetTextureScale(props[i].name);
			}

		}

		return props;
	}

	public void SetProps(Material mat)  {
		UBER_MaterialProp[] newProps = GetMaterialProps (mat);
		if (newProps!=null) {
			if (props!=null) {
				// replace existing props
				for(int i=0; i<newProps.Length; i++) {
					// find the prop and replace values (we need to identify property by name, not index because it can change when we modify shader property block !)
					for(int j=0; j<props.Length; j++) {
						if (props[j].name==newProps[i].name) {
							//Debug.Log (newProps[i].name+"  "+newProps[i].colorValue);
							CopyValues(newProps[i], props[j]);
						}
					}
				}
			} else {
				// assign new props
				props=newProps;
			}
		}
		shader = mat.shader;
	}

	public static readonly string[] GlitterParams=new string[] {
		"_Glitter",
		"_GlitterShown",
		"_SparkleMap",
		"_SparkleMapGlitter",
		"_GlitterColor",
		"_GlitterColor2",
		"_GlitterColorization",
		"_GlitterDensity",
		"_GlitterTiling",
		"_GlitterAnimationFrequency",
		"_GlitterFilter",
		"_GlitterMask"
	};
	public static readonly string[] SnowParams=new string[] {
		"_Snow",
		"_SnowShown",
		"_RippleMapSnow",
		"_SnowColorAndCoverage",
		"_Frost",
		"_SnowSpecGloss",
		"_SnowSlopeDamp",
		"_SnowDiffuseScatteringColor",
		"_SnowDiffuseScatteringExponent",
		"_SnowDiffuseScatteringOffset",
		"_SnowDeepSmoothen",
		"_SparkleMapSnow",
		"_SnowEmissionTransparency",
		"_SnowMicroTiling",
		"_SnowBumpMicro",
		"_SnowMacroTiling",
		"_SnowWorldMapping",
		"_SnowBumpMacro",
		"_SnowDissolve",
		"_SnowDissolveMaskOcclusion",
		"_SnowTranslucencyColor",
		"_SnowGlitterColor",
		"_SnowHeightThreshold",
		"_SnowHeightThresholdTransition",
		"_SnowLevelFromGlobal",
		"_FrostFromGlobal",
		"_SnowBumpMicroFromGlobal",
		"_SnowDissolveFromGlobal",
		"_SnowSpecGlossFromGlobal",
		"_SnowGlitterColorFromGlobal"
	}; 
	public static readonly string[] TranslucencyParams=new string[] {
		"_Translucency",
		"_TranslucencyShown",
		"_TranslucencyColor",
		"_TranslucencyColor2",
		"_TranslucencyStrength",
		"_TranslucencyPointLightDirectionality",
		"_TranslucencyConstant",
		"_TranslucencyNormalOffset",
		"_TranslucencyExponent",
		"_TranslucencyOcclusion"
	}; 
	public static readonly string[] WetnessParams=new string[] {
		"_Wetness",
		"_WetnessShown",
		"_WetnessLevel",
		"_WetnessConst",
		"_WetnessColor",
		"_WetnessDarkening",
		"_WetnessSpecGloss",
		"_WetnessEmissiveness",
		"_WetnessNormalInfluence",
		"_WetnessUVMult",
		"_RippleMap",
		"_WetRipples",
		"_RippleMapWet",
		"_RippleStrength",
		"_RippleTiling",
		"_RippleSpecFilter",
		"_RippleAnimSpeed",
		"_FlowCycleScale",
		"_RippleRefraction",
		"_WetnessNormMIP",
		"_WetnessNormStrength",
		"_WetnessEmissivenessWrap",
		"_WetnessLevelFromGlobal",
		"_WetnessConstFromGlobal",
		"_WetnessFlowGlobalTime",
		"_WetnessMergeWithSnowPerMaterial",
		"_RippleStrengthFromGlobal",
		"_RainIntensityFromGlobal",
		"_WetDroplets",
		"_DropletsMap",
		"_RainIntensity",
		"_DropletsTiling",
		"_DropletsAnimSpeed",
		"_RainIntensityFromGlobal"
	}; 

	public void RestoreProps(Material mat, UBER_PresetParamSection whatToRestore)  {
		if (props!=null) {
			for(int i=0; i<props.Length; i++) {
				bool restore_flag=false;
				if (whatToRestore==UBER_PresetParamSection.All) {
					restore_flag=true;
				} else if (whatToRestore==UBER_PresetParamSection.Glitter) {
					restore_flag=CheckParamCompatibility(GlitterParams, props[i].name);
				} else if (whatToRestore==UBER_PresetParamSection.Snow) {
					restore_flag=CheckParamCompatibility(SnowParams, props[i].name);
				} else if (whatToRestore==UBER_PresetParamSection.Translucency) {
					restore_flag=CheckParamCompatibility(TranslucencyParams, props[i].name);
				} else if (whatToRestore==UBER_PresetParamSection.Wetness) {
					restore_flag=CheckParamCompatibility(WetnessParams, props[i].name);
				}
				if (restore_flag) {
					SetMaterialProp(mat, props[i]);
				}
			}
			if (whatToRestore==UBER_PresetParamSection.All) {
				mat.shader = shader;
			}
		}
	}

	private bool CheckParamCompatibility(string[] propNamesArray, string propName) {
		for(int i=0; i<propNamesArray.Length; i++) {
			if (propNamesArray[i]==propName) {
				return true;
			}
		}
		return false;
	}

	private void CopyValues(UBER_MaterialProp source, UBER_MaterialProp target) {
		target.type=source.type;
		target.name=source.name;
		target.floatValue=source.floatValue;
		target.colorValue=source.colorValue;
		target.vectorValue=source.vectorValue;
		target.textureValue=source.textureValue;
		target.textureOffset=source.textureOffset;
		target.textureScale=source.textureScale;
	}

	private void SetMaterialProp(Material mat, UBER_MaterialProp prop) {
		string propName=prop.name;
		//Debug.Log (mat + " " + propName + " " + prop.type);
		if (mat.HasProperty(propName)) {
			if (prop.type==ShaderUtil.ShaderPropertyType.Float || prop.type==ShaderUtil.ShaderPropertyType.Range) {
				mat.SetFloat(propName, prop.floatValue);
			} else if (prop.type==ShaderUtil.ShaderPropertyType.Color) {
				mat.SetColor(propName, prop.colorValue);
			} else if (prop.type==ShaderUtil.ShaderPropertyType.Vector) {
				mat.SetVector(propName, prop.vectorValue);
			} else if (prop.type==ShaderUtil.ShaderPropertyType.TexEnv) {
				mat.SetTexture(propName, prop.textureValue);
				mat.SetTextureOffset(propName, prop.textureOffset);
				mat.SetTextureScale(propName, prop.textureScale);
			}
		}
	}

#endif
}
