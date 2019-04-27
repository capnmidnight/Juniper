using System;
using UnityEngine;

namespace UnityEditor
{
	internal class UBER_StandardShaderGUI : ShaderGUI
	{
		
		private enum WorkflowMode
		{
			Specular,
			Metallic,
			Dielectric
		}
		
		public enum BlendMode
		{
			Opaque,
			Cutout,
			Fade,		// Old school alpha-blending mode, fresnel does not affect amount of transparency
			Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
		}
		
		public enum CullMode
		{
			Off,
			Front,
			Back
		}
		
		private static class Styles
		{
			public static GUIStyle optionsButton = "PaneOptions";
			public static GUIContent uvSetLabel = new GUIContent("UV Set");
			public static GUIContent[] uvSetOptions = new GUIContent[] { new GUIContent("UV channel 0"), new GUIContent("UV channel 1") };
            /*
            // UBER - decals
            public static GUIContent decalMaskText = new GUIContent("Deferred decal mask", "Select which decals can affect this object");
            public static GUIContent _PierceableText = new GUIContent("Pierceable", "Select to make this object pierceable for decals. Remember that DECAL_PIERCEABLE define should be set to 1 in shader defines section or in UBER_StandardConfig.cginc file.");
            //public static GUIContent _PiercingThresholdText = new GUIContent("threshold (forward)", "Clip pixel threshold - used in forward pierceables (like windows)");
            public static GUIContent decalMaskForSnowText = new GUIContent("Mask for snow", "Select the mask that's used when snow level is above \"Snow level threshold\"");
            public static GUIContent decalMaskForSnowThresholdText = new GUIContent("Snow level threshold", "The snow level that causes surface to select the mask \"Mask for snow\" instead of \"Deferred decal mask\"");
            */

            public static string emptyTootip = "";
			
			// UBER
			public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
			public static GUIContent occlusionStrengthText = new GUIContent("Occlusion", "");
			public static GUIContent albedoOccText = new GUIContent("Albedo", "Albedo (RGB) and Occlusion (A)");
            public static GUIContent albedoSmthText = new GUIContent("Albedo", "Albedo (RGB) and Smoothness (A)");
            public static GUIContent Smoothness_from_albedo_alphaText = new GUIContent("Smoothness (A)", "When PBR (spec/metal/gloss maps are not available\nwe can take this data from albedo texture alpha channel");

            public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
			public static GUIContent specularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
			public static GUIContent metallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");
			public static GUIContent smoothnessText = new GUIContent("Smoothness", "");
			public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map");
			public static GUIContent heightMapText = new GUIContent("Height Map", "Height Map\nSet displace to 0 to turn off parallax mapping\n(color channel configurable in UBER_StandardConfig.cginc)");
			public static GUIContent heightMap2LText = new GUIContent("Height Map", "Height Map\n\n(when \"Height Map 2\" is not present\n2nd layer height is taken from the other channel of main Height Map)\n\n(color channels configurable in UBER_StandardConfig.cginc)");
			public static GUIContent distanceMapText = new GUIContent("Distance Map", "Prepared under texture import - suffix your B&W texture with \"_UBER_DistanceMap\"");
			public static GUIContent extrusionMapText = new GUIContent("Extrusion Map", "It's like heightmap, but displacement is realised orthogonal at 0.5 height threshold");
			public static GUIContent occlusionText = new GUIContent("Occlusion", "Occlusion (G)");
			public static GUIContent occlusionText2 = new GUIContent("Occlusion", "Occlusion (B)"); // 2-layers mode
            public static GUIContent maskingText = new GUIContent("Masking", "Diffuse color masking (A)");
            public static GUIContent maskingText2 = new GUIContent("Masking", "Diffuse color masking (AB)"); // 2-layers mode
            public static GUIContent emissionText = new GUIContent("Emission", "Emission (RGB)");
			
			public static GUIContent emissionAnimatedText = new GUIContent("Emission", "Emission (RGB) Mask (A)");
			public static GUIContent detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A x vertex color R)\nor Wetness (R x vertex color A)\nor Snow (G x vertex color G)\n\nVertex colors used are configurable.");
			public static GUIContent detailAlbedoText = new GUIContent("Detail Albedo", "Albedo (RGB) - blend mode configurable\nin UBER_StandardConfig.cginc or inside shader\n\nTint color (A) - opacity\n       (doesn't affect specularity for textured details)\n       (doesn't affect emission)");
			public static GUIContent detailAlbedoTextMetal = new GUIContent("Detail Albedo", "Albedo (RGB) - blend mode configurable\nin UBER_StandardConfig.cginc or inside shader\n\nTint color (A) - opacity\n       (doesn't affect specularity for textured details)\n       (doesn't affect emission)");
			public static GUIContent detailNormalMapText = new GUIContent("Normal Map", "Normal Map\n  (use lerp slider to override normal\n   instead of blending)");
			//
			
			public static string whiteSpaceString = " ";
			public static string primaryMapsText = "Main Maps";
			public static string secondaryMapsText = "Secondary Maps";
			public static string renderingMode = "Rendering Mode";
			public static GUIContent emissiveWarning = new GUIContent ("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
			public static GUIContent emissiveColorWarning = new GUIContent ("Ensure emissive color is non-black for emission to have effect.");
			public static readonly string[] blendNames = Enum.GetNames (typeof (BlendMode));
			
			
			// UBER
			public static GUIContent _CutoffEdgeGlowText = new GUIContent("Edge emission", "(RGB) - emission color, (A) - edge size");
			
			public static string cullingMode = "Shadow culling mode";
			public static readonly string[] cullingNames = Enum.GetNames (typeof (CullMode));
			public static GUIContent _UVSecOcclusionText = new GUIContent("UV mapping", "UV0 - mapped from main uv - tiled\n\nUV1 - mapped from uv2 (lightmap) - untiled");
			public static GUIContent _SecOcclusionStrengthText = new GUIContent("Strength (2nd)", "Secondary occlusion strength (primary taken from difuse A channel)");
			public static GUIContent _UVSecOcclusionLightmapPackedText = new GUIContent("Lightmap tiling", "Whether secondary occlusion UVs should be transformed by lightmap tiling/offset (useful when you've got atlased occlusion texture shared across many objects)");
			
			public static GUIContent _DiffuseScatteringColorText = new GUIContent("Diffuse scattering", "Albedo x (RGB) x 4\n\n(A) - lerp to (RGB) x 4");
			public static GUIContent _DiffuseScatteringExponentText = new GUIContent ("Exponent", "Diffuse scattering exponent");
			public static GUIContent _DiffuseScatteringOffsetText = new GUIContent ("Offset", "Diffuse scattering offset");
			public static GUIContent _GlossMinText = new GUIContent("Smooth Min", "Input gloss is mapped:\n(0..1) to (Smooth Min..Smooth Max)");
			public static GUIContent _GlossMaxText = new GUIContent("Smooth Max", "Input gloss is mapped:\n(0..1) to (Smooth Min..Smooth Max)");
			
			// bend normals
			public static GUIContent _BendNormalsFreqText = new GUIContent("Bend frequency", "");
			public static GUIContent _BendNormalsStrengthText = new GUIContent("Bend strength", "");
			
			// detail
			public static GUIContent _DetailUVMultText = new GUIContent("Detail mask tiling", "");
			public static GUIContent _DetailNormalLerpText = new GUIContent("Detail normal lerp", "0 - blended normals, 1 - lerped normals");
			
			public static GUIContent _DetailColorText = new GUIContent("Tint", "(RGB+A - opacity)");
			public static GUIContent _DetailEmissivenessText = new GUIContent("Detail emission", "");
			public static GUIContent _DetailSpecGlossText = new GUIContent("Spec/Smoothness Tint", "(RGB - spec tint multiplier/ A - smoothness multiplier)");
			public static GUIContent _DetailSpecLerpText = new GUIContent ("Detail PBR", "");
			// detail - metallic workflow
			public static GUIContent _DetailMetalnessText = new GUIContent("Detail metallic", "");
			public static GUIContent _DetailGlossText = new GUIContent("Detail smoothness", "");
			
			// emission(animated)
			public static GUIContent _PanEmissionMaskText = new GUIContent("Pan Mask", "");
			public static GUIContent _PanUSpeedText = new GUIContent("Pan U speed", "");
			public static GUIContent _PanVSpeedText = new GUIContent("Pan V speed", "");
			public static GUIContent _PulsateEmissionText = new GUIContent("Pulsate", "");
			public static GUIContent _EmissionPulsateSpeedText = new GUIContent("Frequency", "");
			public static GUIContent _MinPulseBrightnessText = new GUIContent("Min brightness", "");
			
			// translucency
			public static GUIContent _TranslucencyText = new GUIContent("Translucency", "");
			public static GUIContent _TranslucencyColorText = new GUIContent("Color", "(RGB) - forward\n\n(A) - deferred realtime lights (x occlusion mask x Strength below = translucency output to G-buffer emission A channel)");
			public static GUIContent _TranslucencyStrengthText = new GUIContent("Strength", "In deferred this is occlusion mask multiplier (clamped 0..1) - written into G-buffer emission A channel, so keep it in reasonable range there.\n\nStrength can be also additionaly controlled per light's alpha color - configurable in UBER_StandardConfig.cginc");
			public static GUIContent _TranslucencyPointLightDirectionalityText = new GUIContent("Point lights directionality", "");
			public static GUIContent _TranslucencyConstantText = new GUIContent("Constant", "Constant translucency add (self emittive)");
			public static GUIContent _TranslucencyNormalOffsetText = new GUIContent("Scattering", "Surface normal dependency");
			public static GUIContent _TranslucencyExponentText = new GUIContent("Spot exponent", "Light transport falloff");
			public static GUIContent _TranslucencyOcclusionText = new GUIContent("Occlusion mask", "Translucency value can depend on one of the occlusion texture channel (configurable in shader defines section - TRANSLUCENCY_OPACITY_CHANNEL)");
			public static GUIContent _TranslucencySuppressRealtimeShadowsText = new GUIContent ("Suppress shadows", "Reduces realtime shadows influence on parts that show translucency. This helps on self-shadowing with translucent objects that casts shadows");
			public static GUIContent _TranslucencyNDotLText = new GUIContent ("NdotL reduction", "Reduces translucency for surfaces that are parallel to light direction. Useful for 2 sided thin objects like vegetation.");
			public static GUIContent _TranslucencyDeferredLightIndexText = new GUIContent ("Deferred setup index", "Select the translucency setup index for deferred (refer to UBER_DeferredParams.cs attached to the camera)");
			
			// POM
			public static GUIContent _POMText = new GUIContent("Parallax Occlusion Mapping", "");
			public static GUIContent _DepthWriteText = new GUIContent("Write into depth", "");
			public static GUIContent _DepthText = new GUIContent ("Depth", "[texture units]\n(can be also relative to world units\nrefer to UBER_StandardConfig.cginc)");
			public static GUIContent _DistStepsText = new GUIContent("Max trace steps", "");
			public static GUIContent _ReliefMIPbiasText = new GUIContent("MIP offset", "Maximize to improve performance");
			
			public static GUIContent _POMPrecomputedFlagText = new GUIContent ("POM data is baked", "When selected POM data is supposed to be baked into uv4 vertex. Name your model using POM_Baked to do it during import.");
			public static GUIContent _POMCurvatureTypeText = new GUIContent ("Curvature type", "Basic - curavutre baked into vertex colors\nMapped - baked into texture (expensive but handles more complex objects)");
			public static GUIContent _DepthReductionDistanceText = new GUIContent ("Depth distance", "We reduce extrusion depth by the distance. Beyond it's zero (you can use different LOD with simplier shader variant there).");
			public static GUIContent _CurvatureCustomUText = new GUIContent("Curvature U val", "Curvature along U texture coordinate");
			public static GUIContent _CurvatureCustomVText = new GUIContent("Curvature V val", "Curvature along V texture coordinate");
			public static GUIContent _CurvatureMultUText = new GUIContent("Curvature U mult", "Curvature multiplier along U texture coordinate");
			public static GUIContent _CurvatureMultVText = new GUIContent("Curvature V mult", "Curvature multiplier along V texture coordinate");
			public static GUIContent _Tan2ObjCustomUText = new GUIContent("Tex2Object U ratio", "Ratio of texture U to Object space units");
			public static GUIContent _Tan2ObjCustomVText = new GUIContent("Tex2Object V ratio", "Ratio of texture V to Object space units");
			//public static GUIContent _Tan2ObjMultUText = new GUIContent("Tex2Object U mult", "Ratio multiplier of texture U to Object space units");
			//public static GUIContent _Tan2ObjMultVText = new GUIContent("Tex2Object V mult", "Ratio multiplier of texture V to Object space units");
			public static GUIContent _ObjectNormalsTexText = new GUIContent ("Object normals", "Normals in object space");
			public static GUIContent _ObjectTangentsTexText = new GUIContent ("Object tangents", "Tangents in object space");
			
			public static GUIContent _UV_ClipText = new GUIContent("UV Clip", "When enabled you can clip pixels outside UV boundaries");
			public static GUIContent _UV_Clip_BordersText = new GUIContent("Borders", "XY - min UV, ZW - max UV");
			public static GUIContent _POM_BottomCutText = new GUIContent("Bottom cut", "Pixels below this threshold on the heightmap will be cut");
			public static GUIContent _POM_MeshIsVolumeText = new GUIContent("Depth per vertex", "Check if a vertex color channel has encoded ray entry Z position in tangent space (it's true for extruded meshes). Channel can be configured via VERTEX_COLOR_CHANNEL_POMZ define in shader header code.");
			public static GUIContent _POM_ExtrudeVolumeText = new GUIContent("Extruded mesh", "Should be checked for extruded meshes (models with \"POM_Baked_POM_Extrude\" suffix) to displace vertices accordingly.");
			
			public static GUIContent _POMShadowsText = new GUIContent("Self-shadowing", "");
			public static GUIContent _ShadowStrengthText = new GUIContent("Strength", "");
			public static GUIContent _DistStepsShadowsText = new GUIContent("Max steps", "");
			public static GUIContent _ShadowMIPbiasText = new GUIContent("MIP offset", "Maximize to improve performance");
			public static GUIContent _SoftnessText = new GUIContent("Softness", "");
			public static GUIContent _SoftnessFadeText = new GUIContent("Softness fade", "");
			// refraction
			public static GUIContent _RefractionText = new GUIContent("Refraction", "");
			public static GUIContent _RefractionBumpScaleText = new GUIContent ("Normalmap scale", "Amount of distortion that comes from normalmap");
			public static GUIContent _RefractionChromaticAberrationText = new GUIContent("Chromatic Aberration", "");
			// wetness
			public static GUIContent _WetnessText = new GUIContent ("Wetness", "");
			public static GUIContent _WetnessHintText = new GUIContent ("Global wetness level can be modified by _UBER_GlobalDry (by default = 0 which means FULL wetness)", "");
			public static GUIContent _WetnessLevelText = new GUIContent ("Water Level", "Controlled by height map & configurable by vertex color");
			public static GUIContent _WetnessConstText = new GUIContent ("Const Wet", "Influences gloss/darkening & configurable by vertex color");
			public static GUIContent _WetnessColorText = new GUIContent ("Color/opacity", "RGB Tint, A Opacity");
			public static GUIContent _WetnessDarkeningText = new GUIContent ("Wet darkening", "How much the albedo color of the surface is dampen when cvered by water");
			public static GUIContent _WetnessSpecGlossText = new GUIContent ("Specular & Gloss", "Specular (RGB) Gloss (A)");
			public static GUIContent _WetnessEmissivenessText = new GUIContent ("Emissiveness", "Emissive power of wet (increase and use Wrap normals checkbox below to simulate lava like mediums)");
			public static GUIContent _WetnessEmissivenessWrapText = new GUIContent ("Wrap normals", "");
			
			public static GUIContent _WetnessLevelFromGlobalText = new GUIContent ("Water Level", "Water level controlled globally (multiplied)");
			public static GUIContent _WetnessConstFromGlobalText = new GUIContent ("Wet", "Wet level controlled globally (multiplied)");
			public static GUIContent _RippleStrengthFromGlobalText = new GUIContent ("Ripple tex strength", "Ripple texture (bumpmap) strength of flow controlled globally (multiplied)");
			public static GUIContent _WetnessFlowGlobalTimeText = new GUIContent ("Global timer", "Global timer allows for flow speed control by script (helpful for \"freezing\" water globally).");
			public static GUIContent _WetnessMergeWithSnowPerMaterialText = new GUIContent ("Pair level with snow", "When checked you can set wetness to be present wherever snow is present on this material. Useful for wet surfaces that remains after global snow level is down (end of global snow melt cycle).\n\n(Enable snow and global control of snow level to make this switch available)");
			
			public static GUIContent _WetnessNormalInfluenceText = new GUIContent ("Normal override", "");
			
			public static GUIContent _WetnessUVMultText = new GUIContent("Detail mask tiling", "");
			
			public static GUIContent _WetRipplesText = new GUIContent ("Ripples (vertex color B)", "");
			public static GUIContent _RippleMapWetText = new GUIContent ("Ripple tex", "");
			public static GUIContent _RippleMapWetSharedText = new GUIContent ("Ripple tex (shared)", "(shared with snow bumps)");
			public static GUIContent _RippleStrengthText = new GUIContent ("Strength", "");
			public static GUIContent _RippleTilingText = new GUIContent ("Tiling", "");
			public static GUIContent _RippleSpecFilterText = new GUIContent ("Gloss filtering", "");
			public static GUIContent _RippleAnimSpeedText = new GUIContent ("Anim speed", "");
			public static GUIContent _FlowCycleScaleText = new GUIContent ("Cycle scale", "");
			public static GUIContent _RippleRefractionText = new GUIContent ("Refraction", "");
			public static GUIContent _WetnessNormMIPText = new GUIContent ("Flow normals MIP", "MIP level of bumpmap taken for flow direction");
			public static GUIContent _WetnessNormStrengthText = new GUIContent ("Normals strength", "Flow taken from normals - strength. Set to 0 to take flow direction from mesh normals only.");
			
			public static GUIContent _DropletsMapText = new GUIContent ("Droplets Map", "");
			public static GUIContent _RainIntensityText = new GUIContent ("Rain Intensity", "");
			public static GUIContent _DropletsTilingText = new GUIContent ("Tiling", "");
			public static GUIContent _DropletsAnimSpeedText = new GUIContent ("Anim speed", "");
			public static GUIContent _RainIntensityFromGlobalText = new GUIContent ("Rain", "Rain intensity controlled globally (multiplied)");
			
			// tessellation
			public static GUIContent _PhongText = new GUIContent ("Phong smoothing", "Amount of phong smoothing applied");
			public static GUIContent _TessDepthText = new GUIContent ("Displacement depth", "Set to zero to disable displacement calculations");
			public static GUIContent _TessOffsetText = new GUIContent ("Midpoint", "Height threshold:\n- height above - we displace outward\n- height below - we displace inward");
			public static GUIContent _TessText = new GUIContent ("Tessellation amount", "Number of tessellation subdivisions applied to edge (depends on the distance from camera - values below)");
			public static GUIContent minDistText = new GUIContent ("Min camera disance", "Will be full tessellation amount applied when camera is closer than this distance");
			public static GUIContent maxDistText = new GUIContent ("Max camera disance", "Will be no tessellation applied whean camera is farther than this distance");
			public static GUIContent _TessEdgeLengthLimitText = new GUIContent ("Edge length limit", "Additional edge length limit (in screenspace - pixels)");
			
			// snow
			public static GUIContent _SnowText = new GUIContent ("Dynamic Snow", "");
            public static GUIContent _RippleMapSnowText = new GUIContent ("Bumps tex", "");
			public static GUIContent _RippleMapSnowSharedText = new GUIContent ("Bumps tex (shared)", "(shared with wet ripples)");
			public static GUIContent _SnowColorAndCoverageText = new GUIContent ("Color (RGB), Level (A)", "");
			public static GUIContent _FrostText = new GUIContent ("Frost", "Frost amount - means diffuse scatter and color taken from snow, but w/o actually snow present. When we've got wetness enabled - frost will appear only on areas influenced by wet/water.");
			public static GUIContent _SnowSpecGlossText = new GUIContent ("Specular & Gloss", "Specular (RGB) Gloss (A)");
			public static GUIContent _SnowSlopeDampText = new GUIContent ("Slope damp", "");
			
			public static GUIContent _SnowDiffuseScatteringColorText = new GUIContent ("Diffuse scattering", "Albedo x (RGB) x 4\n\n(A) - lerp to (RGB) x 4");
			public static GUIContent _SnowDiffuseScatteringExponentText = new GUIContent ("Exponent", "Snow diffuse scattering exponent");
			public static GUIContent _SnowDiffuseScatteringOffsetText = new GUIContent ("Offset", "Snow diffuse scattering offset");
			
			public static GUIContent _SnowDeepSmoothenText = new GUIContent ("Deep smoothening", "");
			public static GUIContent _SparkleMapSnowWithGlitterText = new GUIContent("Micro surface (shared)", "R - glitter mask\nG - dissolve mask\nBA - microsurface normal\n\ntexture shared with glitter mask");
			public static GUIContent _SparkleMapSnowText = new GUIContent("Micro surface", "G - dissolve mask\nBA - microsurface normal");
			public static GUIContent _SnowEmissionTransparencyText = new GUIContent("Emission tint", "How snow passes the underlying emission components (when present)");
			
			public static GUIContent _SnowMicroTilingText = new GUIContent("Tiling", "(relative to detail tiling)");
			public static GUIContent _SnowBumpMicroText = new GUIContent("  Bumps micro", "");
            public static GUIContent _SnowBumpMicro2UsedText = new GUIContent("2nd micro", "(relative to detail tiling)");
            public static GUIContent _SnowMicroTiling2Text = new GUIContent("Tiling", "(relative to detail tiling)");
            public static GUIContent _SnowBumpMicro2Text = new GUIContent("Strength", "");
            public static GUIContent _SnowMacroTilingText = new GUIContent("Tiling", "(relative to detail tiling)");
			public static GUIContent _SnowBumpMacroText = new GUIContent("  Bumps macro", "");
			
			public static GUIContent _SnowWorldMappingText = new GUIContent("World mapping", "Map snow in world space - top planar. Might introduce stretching on slopes, but snow will be continuous across all objects mapped this way.\n\nNote that you can switch it off (refer to UBER_StandardConfig.cginc) to gain a bit of performance.");
			
			public static GUIContent _SnowDissolveText = new GUIContent("Dissolve masking", "");
			public static GUIContent _SnowDissolveMaskOcclusionText = new GUIContent("Mask occlusion", "Occlusion taken from dissolve mask");
			
			public static GUIContent _SnowTranslucencyColorText = new GUIContent("Translucency color", "(RGB) - forward\n\n(A) - deferred realtime lights");
			public static GUIContent _SnowGlitterColorText = new GUIContent("Glitter color", "(RGB) - spec color gain\n(A) - smoothness gain");
			
			public static GUIContent _SnowHeightThresholdText = new GUIContent("Damp by height", "Snow will be present over this threshold\n(world Y) + Damp transition");
			public static GUIContent _SnowHeightThresholdTransitionText = new GUIContent("Damp transition", "Gradual transition of snow coverage threshold");
			
			public static GUIContent _SnowLevelFromGlobalText = new GUIContent ("Level", "Whether snow level can be controlled globally by script");
			public static GUIContent _FrostFromGlobalText = new GUIContent ("Frost", "Whether frost can be controlled globally by script");
			public static GUIContent _SnowBumpMicroFromGlobalText = new GUIContent ("Micro bumps", "Whether micro bumps are controlled globally by script");
			public static GUIContent _SnowDissolveFromGlobalText = new GUIContent ("Dissolve", "Whether dissolve value is controlled globally by script");
			public static GUIContent _SnowSpecGlossFromGlobalText = new GUIContent ("Spec & Gloss", "Whether spec color (RGB)/smoothness(A) are controlled globally by script");
			public static GUIContent _SnowGlitterColorFromGlobalText = new GUIContent ("Glitter", "Whether glitter color is controlled globally by script");
			
			// glitter
			public static GUIContent _GlitterText = new GUIContent ("Glitter", "");
			public static GUIContent _SparkleMapGlitterText = new GUIContent("Glitter mask", "(R) - glitter mask");
			public static GUIContent _SparkleMapGlitterSharedText = new GUIContent("Glitter mask (shared)", "(R) - glitter mask, texture shared\nwith snow micro surface texture");
			public static GUIContent _GlitterTilingText = new GUIContent("Tiling", "(relative to detail tiling)");
			
			public static GUIContent _GlitterColorText = new GUIContent("Glitter color", "(RGB) - spec color gain\n(A) - smoothness gain");
			public static GUIContent _GlitterColorizationText = new GUIContent("Random colorization", "");
			public static GUIContent _GlitterDensityText = new GUIContent("Density", "");
			public static GUIContent _GlitterAnimationFrequencyText = new GUIContent ("Animation frequency", "");
			public static GUIContent _GlitterFilterText = new GUIContent ("Filtering", "Bias for MIP selection of glitter mask");
			public static GUIContent _GlitterMaskText = new GUIContent ("Occlusion mask", "Glitter value can depend on one of the occlusion texture channel (configurable in shader defines section - GLITTER_OPACITY_CHANNEL)");
			
            public static string advancedText = "Unity's Advanced Options";

            // preset collection
            public static GUIContent presetCollectionSectionText = new GUIContent ("Presets", "");
			public static GUIContent presetCollectionText = new GUIContent ("Preset collection", "");
			public static GUIContent presetInCollectionText = new GUIContent ("Preset selected", "Presets available in the collection choosen");
			public static GUIContent currentPresetNameText = new GUIContent ("Current preset name", "Current preset selected. Type any name to save new preset.");
			public static readonly string[] PresetParamSectionNames = Enum.GetNames (typeof (UBER_PresetParamSection));
			
			// RTP - geom blend
			public static GUIContent _TERRAIN_HeightMapText = new GUIContent("Heightmaps", "Detail heightmaps combined (underlying terrain)");
			public static GUIContent _TERRAIN_ControlText = new GUIContent("Control texture", "Control texture (underlying terrain)");
			public static GUIContent _TERRAIN_PosSizeText = new GUIContent("Terrain pos/size", "XY - position on XZ world plane, ZW - extents in world XZ axis");
			public static GUIContent _TERRAIN_TilingText = new GUIContent("Terrain tiling", "XY - tiling, ZW - offset");
			public static string geomBlendSectionText = "RTP - Height blending";
			
			// triplanar
			public static GUIContent triplanarSectionText = new GUIContent("Triplanar", "");
			public static GUIContent _TriplanarWorldMappingText = new GUIContent("World mapping", "When checked shader becomes considerably cheaper, but this option is useful for static objects only");
			public static GUIContent _MainTexAverageColorText = new GUIContent("Albedo average color", "Albedo color used on the blending edges\n(A - blending color transparency)");
			public static GUIContent _MainTex2AverageColorText = new GUIContent ("Albedo 2 average color", "Albedo color used on the blending edges\n(A - blending color transparency)");
			public static GUIContent _TriplanarBlendSharpnessText = new GUIContent("Blending sharpness", "");
			public static GUIContent _TriplanarNormalBlendSharpnessText = new GUIContent ("Sharpen normals", "Additional sharpness for normals");
			public static GUIContent _TriplanarHeightmapBlendingValueText = new GUIContent("Edge by heightmap", "Level of heightmap influence over blending");
			public static GUIContent _TriplanarBlendAmbientOcclusionText = new GUIContent("Ambient Occlusion", "Ambient occlusion on the blending edges");
		}
		
		public static Texture2D UBER_icon;
		//public static float fadeGroupPOM=1.0f;
		
		// preset functionality
		public static UBER_MaterialPresetCollection presetCollection;
		public static bool currentPresetInitFlag;
		public static string curPresetCollectionPath="Assets/UBER_presets.asset";
		
		MaterialProperty blendMode = null;
		
		MaterialProperty albedoMap = null;
		MaterialProperty albedoMap2 = null;
		
		MaterialProperty albedoColor = null;
		MaterialProperty albedoColor2 = null;
		
		MaterialProperty alphaCutoff = null;
		
		MaterialProperty specularMap = null;
		MaterialProperty specularMap2 = null;
        MaterialProperty Smoothness_from_albedo_alpha = null;

        MaterialProperty specularColor = null;
		MaterialProperty specularColor2 = null;
		
		MaterialProperty metallicMap = null;
		MaterialProperty metallicMap2 = null;
		
		MaterialProperty metallic = null;
		MaterialProperty metallic2 = null;
		
		MaterialProperty smoothness = null;
		MaterialProperty smoothness2 = null;
		
		MaterialProperty bumpScale = null;
		MaterialProperty bumpScale2 = null;
		
		MaterialProperty bumpMap = null;
		MaterialProperty bumpMap2 = null;
		
		MaterialProperty occlusionStrength = null;
		MaterialProperty occlusionStrength2 = null;
		MaterialProperty _SecOcclusionStrength = null;
		
		MaterialProperty occlusionMap = null;
		
		MaterialProperty heigtMapScale = null;
		MaterialProperty heigtMapScale2 = null;
		
		MaterialProperty heightMap = null;
		MaterialProperty heightMap2 = null;
		
		MaterialProperty emissionColorForRendering = null;
		MaterialProperty emissionMap = null;
		MaterialProperty detailMask = null;
		MaterialProperty detailAlbedoMap = null;
		MaterialProperty detailNormalMapScale = null;
		MaterialProperty detailNormalMap = null;
		MaterialProperty uvSetSecondary = null;
		MaterialProperty _UVSecOcclusion = null;
		MaterialProperty _UVSecOcclusionLightmapPacked = null;
		
		
		// UBER
		MaterialProperty _CutoffEdgeGlow = null;
		
		MaterialProperty _ShadowCull = null;
		MaterialProperty _MainShown = null;
		MaterialProperty _SecondaryShown = null;
		MaterialProperty _PresetsShown = null;
		
		MaterialProperty _DiffuseScatter = null; // switch to turn on/off diffuse scattering
		MaterialProperty _DiffuseScatteringColor = null;
		MaterialProperty _DiffuseScatteringColor2 = null;
		MaterialProperty _DiffuseScatteringExponent = null;
		MaterialProperty _DiffuseScatteringOffset = null;
		
		MaterialProperty _GlossMin = null;
		MaterialProperty _GlossMax = null;
		
		// bend normals
		MaterialProperty _BendNormalsFreq = null;
		MaterialProperty _BendNormalsStrength = null;
		
		// detail
		MaterialProperty _DetailUVMult = null;
		MaterialProperty _DetailNormalLerp = null;
		
		MaterialProperty _DetailColor = null;
		MaterialProperty _DetailEmissiveness = null;
		MaterialProperty _SpecularRGBGlossADetail = null;
		MaterialProperty _DetailSpecGloss = null;
		MaterialProperty _DetailSpecLerp = null;
		// detail metallic setup
		MaterialProperty _MetallicGlossMapDetail = null;
		MaterialProperty _DetailMetalness = null;
		MaterialProperty _DetailGloss = null;
		
		// emission (animated)
		MaterialProperty _PanEmissionMask = null;
		MaterialProperty _PanUSpeed = null;
		MaterialProperty _PanVSpeed = null;
		MaterialProperty _PulsateEmission = null;
		MaterialProperty _EmissionPulsateSpeed = null;
		MaterialProperty _MinPulseBrightness = null;
		
		// translucency
		MaterialProperty _Translucency = null;
		MaterialProperty _TranslucencyShown = null;
		MaterialProperty _TranslucencyColor = null;
		MaterialProperty _TranslucencyColor2 = null;
		MaterialProperty _TranslucencyStrength = null;
		MaterialProperty _TranslucencyPointLightDirectionality = null;
		MaterialProperty _TranslucencyConstant = null;
		MaterialProperty _TranslucencyNormalOffset = null;
		MaterialProperty _TranslucencyExponent = null;
		MaterialProperty _TranslucencyOcclusion = null;
		MaterialProperty _TranslucencySuppressRealtimeShadows = null;
		MaterialProperty _TranslucencyNDotL = null;
		MaterialProperty _TranslucencyDeferredLightIndex = null;
		
		// POM
		MaterialProperty _POM = null;
		MaterialProperty _POMShown = null;
		MaterialProperty _DepthWrite = null;
		MaterialProperty _Depth = null;
		MaterialProperty _DistSteps = null;
		MaterialProperty _ReliefMIPbias = null;
		MaterialProperty _ObjectNormalsTex = null;
		MaterialProperty _ObjectTangentsTex = null;
		
		// UI props that affects property used by shader - _CurvatureMultOffset & _Tan2ObjectMultOffset
		MaterialProperty _POMPrecomputedFlag = null;
		MaterialProperty _POMCurvatureType = null;
		MaterialProperty _DepthReductionDistance = null;
		MaterialProperty _CurvatureCustomU = null;
		MaterialProperty _CurvatureCustomV = null;
		MaterialProperty _CurvatureMultU = null;
		MaterialProperty _CurvatureMultV = null;
		MaterialProperty _Tan2ObjCustomU = null;
		MaterialProperty _Tan2ObjCustomV = null;
		//MaterialProperty _Tan2ObjMultU = null;
		//MaterialProperty _Tan2ObjMultV = null;
		
		MaterialProperty _UV_Clip = null;
		MaterialProperty _UV_Clip_Borders = null;
		MaterialProperty _POM_BottomCut = null;
		MaterialProperty _POM_MeshIsVolume = null;
		MaterialProperty _POM_ExtrudeVolume = null;
		
		MaterialProperty _POMShadows = null;
		
		MaterialProperty _ShadowStrength = null;
		MaterialProperty _DistStepsShadows = null;
		MaterialProperty _ShadowMIPbias = null;
		MaterialProperty _Softness = null;
		MaterialProperty _SoftnessFade = null;
		// refraction
		MaterialProperty _Refraction = null;
		MaterialProperty _RefractionChromaticAberration = null;
		MaterialProperty _RefractionBumpScale=null;
		
		// wetness
		MaterialProperty _Wetness = null;
		MaterialProperty _WetnessShown = null;
		MaterialProperty _WetnessLevel = null;
		MaterialProperty _WetnessConst = null;
		MaterialProperty _WetnessColor = null;
		MaterialProperty _WetnessDarkening = null;
		MaterialProperty _WetnessSpecGloss = null;
		MaterialProperty _WetnessEmissiveness = null;
		MaterialProperty _WetnessEmissivenessWrap = null;
		MaterialProperty _WetnessNormalInfluence = null;
		MaterialProperty _WetnessUVMult = null;
		
		MaterialProperty _WetnessLevelFromGlobal = null;
		MaterialProperty _WetnessConstFromGlobal = null;
		MaterialProperty _RippleStrengthFromGlobal = null;
		MaterialProperty _WetnessFlowGlobalTime = null;
		MaterialProperty _WetnessMergeWithSnowPerMaterial = null;
		
		// texture shared between wet and snow
		MaterialProperty _RippleMap = null;
		
		MaterialProperty _WetRipples = null;
		MaterialProperty _RippleMapWet = null;
		MaterialProperty _RippleStrength = null;
		MaterialProperty _RippleTiling = null;
		MaterialProperty _RippleSpecFilter = null;
		MaterialProperty _RippleAnimSpeed = null;
		MaterialProperty _FlowCycleScale = null;
		
		MaterialProperty _RippleRefraction = null;
		
		MaterialProperty _WetnessNormMIP = null;
		MaterialProperty _WetnessNormStrength = null;
		
		
		MaterialProperty _WetDroplets = null;
		MaterialProperty _DropletsMap = null;
		MaterialProperty _RainIntensity = null;
		MaterialProperty _DropletsTiling = null;
		MaterialProperty _DropletsAnimSpeed = null;
		MaterialProperty _RainIntensityFromGlobal = null;
		
		// tessellation
		MaterialProperty _TessDepth = null;
		MaterialProperty _TessOffset = null;
		MaterialProperty _Tess = null;
		MaterialProperty _TessEdgeLengthLimit = null;
		MaterialProperty minDist = null;
		MaterialProperty maxDist = null;
		MaterialProperty _Phong = null;
		
		// snow
		MaterialProperty _Snow = null;
		MaterialProperty _SnowShown = null;
		MaterialProperty _RippleMapSnow = null;
        MaterialProperty _SnowColorAndCoverage = null;
		MaterialProperty _Frost = null;
		MaterialProperty _SnowSpecGloss = null;
		MaterialProperty _SnowSlopeDamp = null;
		MaterialProperty _SnowDiffuseScatteringColor = null;
		MaterialProperty _SnowDiffuseScatteringExponent = null;
		MaterialProperty _SnowDiffuseScatteringOffset = null;
		MaterialProperty _SnowDeepSmoothen = null;
		MaterialProperty _SparkleMapSnow = null; // UI prop
		MaterialProperty _SnowEmissionTransparency = null;
		
		MaterialProperty _SnowMicroTiling = null;
		MaterialProperty _SnowBumpMicro = null;
		MaterialProperty _SnowMacroTiling = null;
		MaterialProperty _SnowBumpMacro = null;

        MaterialProperty _SnowMicroTiling2 = null;
        MaterialProperty _SnowBumpMicro2 = null;
        MaterialProperty _SnowBumpMicro2Used = null;

        MaterialProperty _SnowWorldMapping = null;
		
		MaterialProperty _SnowDissolve = null;
		MaterialProperty _SnowDissolveMaskOcclusion = null;
		MaterialProperty _SnowTranslucencyColor = null;
		MaterialProperty _SnowGlitterColor = null;
		
		MaterialProperty _SnowHeightThreshold = null;
		MaterialProperty _SnowHeightThresholdTransition = null;
		
		MaterialProperty _SnowLevelFromGlobal = null;
		MaterialProperty _FrostFromGlobal = null;
		MaterialProperty _SnowBumpMicroFromGlobal = null;
		MaterialProperty _SnowDissolveFromGlobal = null;
		MaterialProperty _SnowSpecGlossFromGlobal = null;
		MaterialProperty _SnowGlitterColorFromGlobal = null;
		
		// glitter
		MaterialProperty _Glitter = null;
		MaterialProperty _GlitterShown = null;
		MaterialProperty _SparkleMapGlitter = null; // UI prop
		MaterialProperty _SparkleMap = null; // prop used in shader
		
		MaterialProperty _GlitterColor = null;
		MaterialProperty _GlitterColor2 = null;
		MaterialProperty _GlitterColorization = null;
		MaterialProperty _GlitterDensity = null;
		MaterialProperty _GlitterTiling = null;
		//MaterialProperty _GlitterAnimationFrequency = null;
		MaterialProperty _GlitterFilter = null;
		MaterialProperty _GlitterMask = null;
		
		//
		// RTP - geom blend
		MaterialProperty _TERRAIN_HeightMap;
		MaterialProperty _TERRAIN_Control;
		MaterialProperty _TERRAIN_PosSize;
		MaterialProperty _TERRAIN_Tiling;
		MaterialProperty _GeomBlendShown;
		
		// triplanar
		MaterialProperty _TriplanarShown;
		MaterialProperty _TriplanarWorldMapping;
		MaterialProperty _MainTexAverageColor;
		MaterialProperty _MainTex2AverageColor;
		MaterialProperty _TriplanarBlendSharpness;
		MaterialProperty _TriplanarNormalBlendSharpness;
		MaterialProperty _TriplanarHeightmapBlendingValue;
		MaterialProperty _TriplanarBlendAmbientOcclusion;

        /*
        // decal
        MaterialProperty _DecalMask;
        MaterialProperty _DecalMaskGUI;
        MaterialProperty _DecalMaskForSnow;
        MaterialProperty _DecalMaskForSnowGUI;
        MaterialProperty _DecalMaskForSnowThreshold;
        MaterialProperty _Pierceable;
        //MaterialProperty _PiercingThreshold;
        */

        // coloring
        MaterialProperty diffuseTintArray0;
        MaterialProperty diffuseTintArray1;
        MaterialProperty diffuseTintArray2;
        MaterialProperty diffuseTintArray3;
        MaterialProperty diffuseTintArray4;
        MaterialProperty diffuseTintArray5;
        MaterialProperty diffuseTintArray6;
        MaterialProperty diffuseTintArray7;
        MaterialProperty diffuseTintArray8;
        MaterialProperty diffuseTintArray9;
        MaterialProperty diffuseTintArray10;
        MaterialProperty diffuseTintArray11;
        MaterialProperty diffuseTintArray12;
        MaterialProperty diffuseTintArray13;
        MaterialProperty diffuseTintArray14;
        MaterialProperty diffuseTintArray15;

        MaterialProperty diffuseTintArrayB0;
        MaterialProperty diffuseTintArrayB1;
        MaterialProperty diffuseTintArrayB2;
        MaterialProperty diffuseTintArrayB3;
        MaterialProperty diffuseTintArrayB4;
        MaterialProperty diffuseTintArrayB5;
        MaterialProperty diffuseTintArrayB6;
        MaterialProperty diffuseTintArrayB7;
        MaterialProperty diffuseTintArrayB8;
        MaterialProperty diffuseTintArrayB9;
        MaterialProperty diffuseTintArrayB10;
        MaterialProperty diffuseTintArrayB11;
        MaterialProperty diffuseTintArrayB12;
        MaterialProperty diffuseTintArrayB13;
        MaterialProperty diffuseTintArrayB14;
        MaterialProperty diffuseTintArrayB15;


        MaterialEditor m_MaterialEditor;
		WorkflowMode m_WorkflowMode = WorkflowMode.Specular;
#if !UNITY_2018_1_OR_NEWER
		ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1/99f, 3f);
#endif
		
		bool m_FirstTimeApply = true;
		
		static bool alt_shader_inited=false;
		static string alt_shader = ""; // tessellation/regular shader
		
		public void FindProperties (MaterialProperty[] props)
		{
			blendMode = FindProperty ("_Mode", props);
			
			albedoMap = FindProperty ("_MainTex", props);
			albedoMap2 = FindProperty ("_MainTex2", props);
            Smoothness_from_albedo_alpha = FindProperty("_Smoothness_from_albedo_alpha", props, false);

            albedoColor = FindProperty ("_Color", props);
			albedoColor2 = FindProperty ("_Color2", props);
			
			alphaCutoff = FindProperty ("_Cutoff", props);
			
			specularMap = FindProperty ("_SpecGlossMap", props, false);
			specularMap2 = FindProperty ("_SpecGlossMap2", props, false);

            specularColor = FindProperty ("_SpecColor", props, false);
			specularColor2 = FindProperty ("_SpecColor2", props, false);
			
			metallicMap = FindProperty ("_MetallicGlossMap", props, false);
			metallicMap2 = FindProperty ("_MetallicGlossMap2", props, false);
			
			metallic = FindProperty ("_Metallic", props, false);
			metallic2 = FindProperty ("_Metallic2", props, false);
			
			if (specularMap != null && specularColor != null)
				m_WorkflowMode = WorkflowMode.Specular;
			else if (metallicMap != null && metallic != null)
				m_WorkflowMode = WorkflowMode.Metallic;
			else
				m_WorkflowMode = WorkflowMode.Dielectric;
			
			smoothness = FindProperty ("_Glossiness", props);
			smoothness2 = FindProperty ("_Glossiness2", props);
			
			bumpScale = FindProperty ("_BumpScale", props);
			bumpScale2 = FindProperty ("_BumpScale2", props);
			
			bumpMap = FindProperty ("_BumpMap", props);
			bumpMap2 = FindProperty ("_BumpMap2", props);
			
			heigtMapScale = FindProperty ("_Parallax", props);
			heigtMapScale2 = FindProperty ("_Parallax2", props);
			
			heightMap = FindProperty("_ParallaxMap", props);
			heightMap2 = FindProperty("_ParallaxMap2", props);
			
			if (m_MaterialEditor.targets.Length == 1) {
				FixSubstanceTexelSizeBug(props);
			}
			
			occlusionStrength = FindProperty ("_OcclusionStrength", props);
			occlusionStrength2 = FindProperty ("_OcclusionStrength2", props);
			_SecOcclusionStrength = FindProperty ("_SecOcclusionStrength", props);
			
			occlusionMap = FindProperty ("_OcclusionMap", props);
			
			emissionColorForRendering = FindProperty ("_EmissionColor", props);
			emissionMap = FindProperty ("_EmissionMap", props);
			detailMask = FindProperty ("_DetailMask", props);
			detailAlbedoMap = FindProperty ("_DetailAlbedoMap", props);
			detailNormalMapScale = FindProperty ("_DetailNormalMapScale", props);
			detailNormalMap = FindProperty ("_DetailNormalMap", props);
			uvSetSecondary = FindProperty ("_UVSec", props);
			_UVSecOcclusion = FindProperty ("_UVSecOcclusion", props);
			_UVSecOcclusionLightmapPacked = FindProperty ("_UVSecOcclusionLightmapPacked", props);
			
			
			// UBER
			_CutoffEdgeGlow = FindProperty ("_CutoffEdgeGlow", props);
			
			_ShadowCull = FindProperty ("_ShadowCull", props);
			_MainShown = FindProperty ("_MainShown", props);
			_SecondaryShown = FindProperty ("_SecondaryShown", props);
			_PresetsShown = FindProperty ("_PresetsShown", props);
			
			_DiffuseScatter = FindProperty ("_DiffuseScatter", props);
			_DiffuseScatteringColor = FindProperty ("_DiffuseScatteringColor", props);
			_DiffuseScatteringColor2 = FindProperty ("_DiffuseScatteringColor2", props);
			_DiffuseScatteringExponent = FindProperty ("_DiffuseScatteringExponent", props);
			_DiffuseScatteringOffset = FindProperty ("_DiffuseScatteringOffset", props);
			_GlossMin = FindProperty ("_GlossMin", props);
			_GlossMax = FindProperty ("_GlossMax", props);
			// bend normals
			_BendNormalsFreq = FindProperty ("_BendNormalsFreq", props);
			_BendNormalsStrength = FindProperty ("_BendNormalsStrength", props);
			// detail
			_DetailUVMult = FindProperty ("_DetailUVMult", props);
			_DetailNormalLerp = FindProperty ("_DetailNormalLerp", props);
			
			_DetailColor = FindProperty ("_DetailColor", props);
			_DetailEmissiveness = FindProperty ("_DetailEmissiveness", props);
			if (m_WorkflowMode == WorkflowMode.Specular) {
				_SpecularRGBGlossADetail = FindProperty ("_SpecularRGBGlossADetail", props);
				_DetailSpecGloss = FindProperty ("_DetailSpecGloss", props);
			} else if (m_WorkflowMode == WorkflowMode.Metallic) {
				// metalic setup
				_MetallicGlossMapDetail = FindProperty ("_MetallicGlossMapDetail", props);
				_DetailMetalness = FindProperty ("_DetailMetalness", props);
				_DetailGloss = FindProperty ("_DetailGloss", props);
			}
			_DetailSpecLerp = FindProperty ("_DetailSpecLerp", props);
			
			// emission (animated)
			_PanEmissionMask = FindProperty ("_PanEmissionMask", props);
			_PanUSpeed = FindProperty ("_PanUSpeed", props);
			_PanVSpeed = FindProperty ("_PanVSpeed", props);
			_PulsateEmission = FindProperty ("_PulsateEmission", props);
			_EmissionPulsateSpeed = FindProperty ("_EmissionPulsateSpeed", props);
			_MinPulseBrightness = FindProperty ("_MinPulseBrightness", props);
			
			// translucency
			_Translucency = FindProperty ("_Translucency", props);
			_TranslucencyShown = FindProperty ("_TranslucencyShown", props);
			_TranslucencyColor = FindProperty ("_TranslucencyColor", props);
			_TranslucencyColor2 = FindProperty ("_TranslucencyColor2", props);
			_TranslucencyStrength = FindProperty ("_TranslucencyStrength", props);
			_TranslucencyPointLightDirectionality = FindProperty ("_TranslucencyPointLightDirectionality", props);
			_TranslucencyConstant = FindProperty ("_TranslucencyConstant", props);
			_TranslucencyNormalOffset = FindProperty ("_TranslucencyNormalOffset", props);
			_TranslucencyExponent = FindProperty ("_TranslucencyExponent", props);
			_TranslucencyOcclusion = FindProperty ("_TranslucencyOcclusion", props);
			_TranslucencySuppressRealtimeShadows = FindProperty ("_TranslucencySuppressRealtimeShadows", props);
			_TranslucencyNDotL = FindProperty ("_TranslucencyNDotL", props);
			_TranslucencyDeferredLightIndex = FindProperty ("_TranslucencyDeferredLightIndex", props);
			
			// POM
			_POM = FindProperty ("_POM", props);
			_POMShown = FindProperty ("_POMShown", props);
			_DepthWrite = FindProperty ("_DepthWrite", props);
			_Depth = FindProperty ("_Depth", props);
			_DistSteps = FindProperty ("_DistSteps", props);
			_ReliefMIPbias = FindProperty ("_ReliefMIPbias", props);
			_ObjectNormalsTex = FindProperty ("_ObjectNormalsTex", props);
			_ObjectTangentsTex = FindProperty ("_ObjectTangentsTex", props);
			
			_POMPrecomputedFlag = FindProperty ("_POMPrecomputedFlag", props);
			_POMCurvatureType = FindProperty ("_POMCurvatureType", props);
			_DepthReductionDistance = FindProperty ("_DepthReductionDistance", props);
			_CurvatureCustomU = FindProperty ("_CurvatureCustomU", props);
			_CurvatureCustomV = FindProperty ("_CurvatureCustomV", props);
			_CurvatureMultU = FindProperty ("_CurvatureMultU", props);
			_CurvatureMultV = FindProperty ("_CurvatureMultV", props);
			_Tan2ObjCustomU = FindProperty ("_Tan2ObjCustomU", props);
			_Tan2ObjCustomV = FindProperty ("_Tan2ObjCustomV", props);
			//_Tan2ObjMultU = FindProperty ("_Tan2ObjMultU", props);
			//_Tan2ObjMultV = FindProperty ("_Tan2ObjMultV", props);
			
			_UV_Clip = FindProperty ("_UV_Clip", props);
			_UV_Clip_Borders = FindProperty ("_UV_Clip_Borders", props);
			_POM_BottomCut = FindProperty ("_POM_BottomCut", props);
			_POM_MeshIsVolume = FindProperty ("_POM_MeshIsVolume", props);
			_POM_ExtrudeVolume = FindProperty ("_POM_ExtrudeVolume", props);
			
			_POMShadows = FindProperty ("_POMShadows", props);
			_ShadowStrength = FindProperty ("_ShadowStrength", props);
			_DistStepsShadows = FindProperty ("_DistStepsShadows", props);
			_ShadowMIPbias = FindProperty ("_ShadowMIPbias", props);
			_Softness = FindProperty ("_Softness", props);
			_SoftnessFade = FindProperty ("_SoftnessFade", props);
			// refraction
			_Refraction = FindProperty ("_Refraction", props);
			_RefractionBumpScale = FindProperty ("_RefractionBumpScale", props);
			_RefractionChromaticAberration = FindProperty ("_RefractionChromaticAberration", props);
			// wetness
			_Wetness = FindProperty ("_Wetness", props);
			_WetnessShown = FindProperty ("_WetnessShown", props);
			_WetnessLevel = FindProperty ("_WetnessLevel", props);
			_WetnessConst = FindProperty ("_WetnessConst", props);
			_WetnessColor = FindProperty ("_WetnessColor", props);
			_WetnessDarkening = FindProperty ("_WetnessDarkening", props);
			
			_WetnessSpecGloss = FindProperty ("_WetnessSpecGloss", props);
			_WetnessEmissiveness = FindProperty ("_WetnessEmissiveness", props);
			_WetnessEmissivenessWrap = FindProperty ("_WetnessEmissivenessWrap", props);
			_WetnessNormalInfluence = FindProperty ("_WetnessNormalInfluence", props);
			_WetnessUVMult = FindProperty ("_WetnessUVMult", props);
			
			_WetnessLevelFromGlobal = FindProperty ("_WetnessLevelFromGlobal", props);
			_WetnessConstFromGlobal = FindProperty ("_WetnessConstFromGlobal", props);
			_RippleStrengthFromGlobal = FindProperty ("_RippleStrengthFromGlobal", props);
			
			_WetnessFlowGlobalTime = FindProperty ("_WetnessFlowGlobalTime", props);
			_WetnessMergeWithSnowPerMaterial = FindProperty ("_WetnessMergeWithSnowPerMaterial", props);
			
			// shared texture - wet ripples, snow macro bumps
			_RippleMap = FindProperty ("_RippleMap", props);
			
			_WetRipples = FindProperty ("_WetRipples", props);
			_RippleMapWet = FindProperty ("_RippleMapWet", props);
			_RippleStrength = FindProperty ("_RippleStrength", props);
			_RippleTiling = FindProperty ("_RippleTiling", props);
			_RippleSpecFilter = FindProperty ("_RippleSpecFilter", props);
			_RippleAnimSpeed = FindProperty ("_RippleAnimSpeed", props);
			_FlowCycleScale = FindProperty ("_FlowCycleScale", props);
			_RippleRefraction = FindProperty ("_RippleRefraction", props);
			_WetnessNormMIP = FindProperty ("_WetnessNormMIP", props);
			_WetnessNormStrength = FindProperty ("_WetnessNormStrength", props);
			
			_WetDroplets = FindProperty ("_WetDroplets", props);
			_DropletsMap = FindProperty ("_DropletsMap", props);
			_RainIntensity = FindProperty ("_RainIntensity", props);
			_DropletsTiling = FindProperty ("_DropletsTiling", props);
			_DropletsAnimSpeed = FindProperty ("_DropletsAnimSpeed", props);
			_RainIntensityFromGlobal = FindProperty ("_RainIntensityFromGlobal", props);
			
			// tessellation
			_TessDepth = FindProperty ("_TessDepth", props);
			_TessOffset = FindProperty ("_TessOffset", props);
			_Tess = FindProperty ("_Tess", props);
			_TessEdgeLengthLimit = FindProperty ("_TessEdgeLengthLimit", props);
			minDist = FindProperty ("minDist", props);
			maxDist = FindProperty ("maxDist", props);
			_Phong = FindProperty ("_Phong", props);
			
			// snow
			_Snow = FindProperty ("_Snow", props);
			_SnowShown = FindProperty ("_SnowShown", props);
            _RippleMapSnow = FindProperty("_RippleMapSnow", props);
            _SnowColorAndCoverage = FindProperty ("_SnowColorAndCoverage", props);
			_Frost = FindProperty ("_Frost", props);
			_SnowSpecGloss = FindProperty ("_SnowSpecGloss", props);
			_SnowSlopeDamp = FindProperty ("_SnowSlopeDamp", props);
			_SnowDiffuseScatteringColor = FindProperty ("_SnowDiffuseScatteringColor", props);
			_SnowDiffuseScatteringExponent = FindProperty ("_SnowDiffuseScatteringExponent", props);
			_SnowDiffuseScatteringOffset = FindProperty ("_SnowDiffuseScatteringOffset", props);
			_SnowDeepSmoothen = FindProperty ("_SnowDeepSmoothen", props);
			_SparkleMapSnow = FindProperty ("_SparkleMapSnow", props);
			_SnowEmissionTransparency = FindProperty ("_SnowEmissionTransparency", props);
			
			_SnowMicroTiling = FindProperty ("_SnowMicroTiling", props);
			_SnowBumpMicro = FindProperty ("_SnowBumpMicro", props);
			_SnowMacroTiling = FindProperty ("_SnowMacroTiling", props);
			_SnowBumpMacro = FindProperty ("_SnowBumpMacro", props);

            _SnowBumpMicro2Used = FindProperty("_SnowBumpMicro2Used", props, false);
            _SnowMicroTiling2 = FindProperty("_SnowMicroTiling2", props, false);
            _SnowBumpMicro2 = FindProperty("_SnowBumpMicro2", props, false);

            _SnowWorldMapping = FindProperty ("_SnowWorldMapping", props);
			
			_SnowDissolve = FindProperty ("_SnowDissolve", props);
			_SnowDissolveMaskOcclusion = FindProperty ("_SnowDissolveMaskOcclusion", props);
			_SnowTranslucencyColor = FindProperty ("_SnowTranslucencyColor", props);
			_SnowGlitterColor = FindProperty ("_SnowGlitterColor", props);
			
			_SnowHeightThreshold = FindProperty ("_SnowHeightThreshold", props);
			_SnowHeightThresholdTransition = FindProperty ("_SnowHeightThresholdTransition", props);
			
			_SnowLevelFromGlobal = FindProperty ("_SnowLevelFromGlobal", props);
			_FrostFromGlobal = FindProperty ("_FrostFromGlobal", props);
			_SnowBumpMicroFromGlobal = FindProperty ("_SnowBumpMicroFromGlobal", props);
			_SnowDissolveFromGlobal = FindProperty ("_SnowDissolveFromGlobal", props);
			_SnowSpecGlossFromGlobal = FindProperty ("_SnowSpecGlossFromGlobal", props);
			_SnowGlitterColorFromGlobal = FindProperty ("_SnowGlitterColorFromGlobal", props);
			
			// glitter
			_Glitter = FindProperty ("_Glitter", props);
			_GlitterShown = FindProperty ("_GlitterShown", props);
			_SparkleMapGlitter = FindProperty ("_SparkleMapGlitter", props);
			_SparkleMap = FindProperty ("_SparkleMap", props);
			
			_GlitterColor = FindProperty ("_GlitterColor", props);
			_GlitterColor2 = FindProperty ("_GlitterColor2", props);
			_GlitterColorization = FindProperty ("_GlitterColorization", props);
			_GlitterDensity = FindProperty ("_GlitterDensity", props);
			_GlitterTiling = FindProperty ("_GlitterTiling", props);
			//_GlitterAnimationFrequency = FindProperty ("_GlitterAnimationFrequency", props);
			_GlitterFilter = FindProperty ("_GlitterFilter", props);
			_GlitterMask = FindProperty ("_GlitterMask", props);
			
			// RTP - geom blend
			_TERRAIN_HeightMap = FindProperty ("_TERRAIN_HeightMap", props, false);
			_TERRAIN_Control = FindProperty ("_TERRAIN_Control", props, false);
			_TERRAIN_PosSize = FindProperty ("_TERRAIN_PosSize", props, false);
			_TERRAIN_Tiling = FindProperty ("_TERRAIN_Tiling", props, false);
			_GeomBlendShown = FindProperty ("_GeomBlendShown", props, false);
			
			// triplanar
			_TriplanarShown = FindProperty ("_TriplanarShown", props, false);
			_TriplanarWorldMapping = FindProperty ("_TriplanarWorldMapping", props, false);
			_MainTexAverageColor = FindProperty ("_MainTexAverageColor", props, false);
			_MainTex2AverageColor = FindProperty ("_MainTex2AverageColor", props, false);
			_TriplanarBlendSharpness = FindProperty ("_TriplanarBlendSharpness", props, false);
			_TriplanarNormalBlendSharpness = FindProperty ("_TriplanarNormalBlendSharpness", props, false);
			_TriplanarHeightmapBlendingValue = FindProperty ("_TriplanarHeightmapBlendingValue", props, false);
			_TriplanarBlendAmbientOcclusion = FindProperty ("_TriplanarBlendAmbientOcclusion", props, false);

            /*
            // decal
            _DecalMask = FindProperty("_DecalMask", props, false);
            _DecalMaskGUI = FindProperty("_DecalMaskGUI", props, false);
            _DecalMaskForSnow = FindProperty("_DecalMaskForSnow", props, false);
            _DecalMaskForSnowGUI = FindProperty("_DecalMaskForSnowGUI", props, false);
            _DecalMaskForSnowThreshold = FindProperty("_DecalMaskForSnowThreshold", props, false);
            _Pierceable = FindProperty("_Pierceable", props, false);
            //_PiercingThreshold = FindProperty("_PiercingThreshold", props, false);
            */

            // coloring
            diffuseTintArray0 = FindProperty("diffuseTintArray0", props, false);
            diffuseTintArray1 = FindProperty("diffuseTintArray1", props, false);
            diffuseTintArray2 = FindProperty("diffuseTintArray2", props, false);
            diffuseTintArray3 = FindProperty("diffuseTintArray3", props, false);
            diffuseTintArray4 = FindProperty("diffuseTintArray4", props, false);
            diffuseTintArray5 = FindProperty("diffuseTintArray5", props, false);
            diffuseTintArray6 = FindProperty("diffuseTintArray6", props, false);
            diffuseTintArray7 = FindProperty("diffuseTintArray7", props, false);
            diffuseTintArray8 = FindProperty("diffuseTintArray8", props, false);
            diffuseTintArray9 = FindProperty("diffuseTintArray9", props, false);
            diffuseTintArray10 = FindProperty("diffuseTintArray10", props, false);
            diffuseTintArray11 = FindProperty("diffuseTintArray11", props, false);
            diffuseTintArray12 = FindProperty("diffuseTintArray12", props, false);
            diffuseTintArray13 = FindProperty("diffuseTintArray13", props, false);
            diffuseTintArray14 = FindProperty("diffuseTintArray14", props, false);
            diffuseTintArray15 = FindProperty("diffuseTintArray15", props, false);

            diffuseTintArrayB0 = FindProperty("diffuseTintArrayB0", props, false);
            diffuseTintArrayB1 = FindProperty("diffuseTintArrayB1", props, false);
            diffuseTintArrayB2 = FindProperty("diffuseTintArrayB2", props, false);
            diffuseTintArrayB3 = FindProperty("diffuseTintArrayB3", props, false);
            diffuseTintArrayB4 = FindProperty("diffuseTintArrayB4", props, false);
            diffuseTintArrayB5 = FindProperty("diffuseTintArrayB5", props, false);
            diffuseTintArrayB6 = FindProperty("diffuseTintArrayB6", props, false);
            diffuseTintArrayB7 = FindProperty("diffuseTintArrayB7", props, false);
            diffuseTintArrayB8 = FindProperty("diffuseTintArrayB8", props, false);
            diffuseTintArrayB9 = FindProperty("diffuseTintArrayB9", props, false);
            diffuseTintArrayB10 = FindProperty("diffuseTintArrayB10", props, false);
            diffuseTintArrayB11 = FindProperty("diffuseTintArrayB11", props, false);
            diffuseTintArrayB12 = FindProperty("diffuseTintArrayB12", props, false);
            diffuseTintArrayB13 = FindProperty("diffuseTintArrayB13", props, false);
            diffuseTintArrayB14 = FindProperty("diffuseTintArrayB14", props, false);
            diffuseTintArrayB15 = FindProperty("diffuseTintArrayB15", props, false);

        }

        public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
		{
			if (UBER_icon==null) {
				string[] icons = AssetDatabase.FindAssets("UBERLogo128x64 t:Texture2D", null);
				if (icons.Length>0) {
					UBER_icon=AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(icons[0]));
				}
			}
			
			m_MaterialEditor = materialEditor;
			Material material = materialEditor.target as Material;
			
			FindProperties (props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
			
			ShaderPropertiesGUI (material, props);
			
			// Make sure that needed keywords are set up if we're switching some existing
			// material to a standard shader.
			if (m_FirstTimeApply)
			{
				SetMaterialKeywords (material, m_WorkflowMode);
				m_FirstTimeApply = false;
			}
		}
		
		private void CheckForAltShader(Material material, bool _Tessellation_Used) {
			alt_shader = ""; // tessellation/regular shader
			if (_Tessellation_Used) {
				alt_shader=material.shader.name.Replace("Tessellation/", "");
				if (!Shader.Find(alt_shader)) alt_shader="";
			} else {
				alt_shader=material.shader.name.Replace("Setup/", "Setup/Tessellation/");
				if (!Shader.Find(alt_shader)) alt_shader="";
			}
		} 
		
		private void SaveCollectionReferenceInThisScene() {
			GameObject go=GameObject.Find("UBER_PresetReference");
			if (go) {
				(go.GetComponent<UBER_MaterialPresetCollectionReference>() as UBER_MaterialPresetCollectionReference).collectionReference=presetCollection;
			} else {
				go=new GameObject("UBER_PresetReference");
				go.hideFlags=HideFlags.HideInHierarchy;// | HideFlags.DontSaveInBuild;
				go.AddComponent<UBER_MaterialPresetCollectionReference>();
				(go.GetComponent<UBER_MaterialPresetCollectionReference>() as UBER_MaterialPresetCollectionReference).collectionReference=presetCollection;
			}
		}
		
		public void ShaderPropertiesGUI (Material material, MaterialProperty[] props)
		{
			//
			// static switches
			//
			// animated emission
			bool _Animated_Emission = _PulsateEmission.floatValue != 0 || _PanEmissionMask.floatValue != 0;
			
			bool _Refraction_Used=false, _Refraction_hasMixedValue=false, _Refraction_hasAllOff=false;
			GetStaticProp(material, "REFR", ref _Refraction_Used, ref _Refraction_hasMixedValue, ref _Refraction_hasAllOff);
			
			bool _Tessellation_Used=false, _Tessellation_hasMixedValue=false, _Tessellation_hasAllOff=false;
			GetStaticProp(material, "TESS", ref _Tessellation_Used, ref _Tessellation_hasMixedValue, ref _Tessellation_hasAllOff);
			
			if (!alt_shader_inited) {
				CheckForAltShader(material, _Tessellation_Used);
				alt_shader_inited=true;
			}
			
			bool _BendNormals_Used=false, _BendNormals_hasMixedValue=false, _BendNormals_hasAllOff=false;
			GetStaticProp(material, "BEND_NORM", ref _BendNormals_Used, ref _BendNormals_hasMixedValue, ref _BendNormals_hasAllOff);
			
			bool _TwoLayers_Used=false, _TwoLayers_hasMixedValue=false, _TwoLayers_hasAllOff=false;
			GetStaticProp(material, "TWO_LAYERS", ref _TwoLayers_Used, ref _TwoLayers_hasMixedValue, ref _TwoLayers_hasAllOff);
			
			bool _Silhouettes_Used=false, _Silhouettes_hasMixedValue=false, _Silhouettes_hasAllOff=false;
			GetStaticProp(material, "SILHOUETTE_CURVATURE", ref _Silhouettes_Used, ref _Silhouettes_hasMixedValue, ref _Silhouettes_hasAllOff);
			
			bool _DistanceMap_Used=false, _DistanceMap_hasMixedValue=false, _DistanceMap_hasAllOff=false;
			GetStaticProp(material, "POM_DISTANCE_MAP", ref _DistanceMap_Used, ref _DistanceMap_hasMixedValue, ref _DistanceMap_hasAllOff);
			
			bool _ExtrusionMap_Used=false, _ExtrusionMap_hasMixedValue=false, _ExtrusionMap_hasAllOff=false;
			GetStaticProp(material, "POM_EXTRUSION_MAP", ref _ExtrusionMap_Used, ref _ExtrusionMap_hasMixedValue, ref _ExtrusionMap_hasAllOff);
			
			bool _GeomBlend_Used=false, _GeomBlend_hasMixedValue=false, _GeomBlend_hasAllOff=false;
			GetStaticProp(material, "GEOM_BLEND", ref _GeomBlend_Used, ref _GeomBlend_hasMixedValue, ref _GeomBlend_hasAllOff);
			
			bool _Triplanar_Used=false, _Triplanar_hasMixedValue=false, _Triplanar_hasAllOff=false;
			GetStaticProp(material, "TRIPLANAR", ref _Triplanar_Used, ref _Triplanar_hasMixedValue, ref _Triplanar_hasAllOff);
			
			bool _NONORMAL_Used=false, _NONORMAL_hasMixedValue=false, _NONORMAL_hasAllOff=false;
			GetStaticProp(material, "NONORMAL", ref _NONORMAL_Used, ref _NONORMAL_hasMixedValue, ref _NONORMAL_hasAllOff);

            bool _FORCE_DIFFAO_Used = false, _FORCE_DIFFAO_hasMixedValue = false, _FORCE_DIFFAO_hasAllOff = false;
            GetStaticProp(material, "FORCE_DIFFAO", ref _FORCE_DIFFAO_Used, ref _FORCE_DIFFAO_hasMixedValue, ref _FORCE_DIFFAO_hasAllOff);

            bool _2ndColor_Used = false, _2ndColor_hasMixedValue = false, _2ndColor_hasAllOff = false;
            GetStaticProp(material, "2ND_COLOR", ref _2ndColor_Used, ref _2ndColor_hasMixedValue, ref _2ndColor_hasAllOff);

            // occlusion texture not present - all occlusion data taken from a channel of albedo texture (unusable for transparent shaders though)
            // occlusion texture present - occlusion taken from occlusion texture (so it's usable for transparent shaders)
            // when occlusion texture is present and _UVSecOcclusion checkbox is set, UV1 coords are taken for reading from occlusion texture and such occlusion is superimposed with primary occlusion taken from albedo A
            bool _AlbedoOcclusion_Used = ((BlendMode)material.GetFloat("_Mode") == BlendMode.Opaque) && (occlusionMap.textureValue == null || _UVSecOcclusion.floatValue==1 || _FORCE_DIFFAO_Used);
			
			if (UBER_Global.initPresetCollection) {
				// reinit presetCollection between loaded scenes and scripts got recompiled
				presetCollection = null;
				currentPresetInitFlag=false;
			}
			if (presetCollection == null) {
				if (!currentPresetInitFlag) {
					currentPresetInitFlag=true;
					// try to init from hidden game object - will be stored per scene then
					// reference to our presetCollection asset is stored in hidden UBER_PresetReference game object per scene
					GameObject go=GameObject.Find("UBER_PresetReference");
					if (go) {
						presetCollection=(go.GetComponent<UBER_MaterialPresetCollectionReference>() as UBER_MaterialPresetCollectionReference).collectionReference;
					}
				}
			}  
			
			// Use default labelWidth
			EditorGUIUtility.labelWidth = 0f;
			
			// Detect any changes to the material
			EditorGUI.BeginChangeCheck();
			{
				EditorGUI.BeginDisabledGroup (_Refraction_Used || _GeomBlend_Used);
				EditorGUILayout.BeginVertical(GUILayout.MaxWidth(250));
				BlendModePopup ();
				EditorGUI.EndDisabledGroup ();
				
				if (UBER_icon != null) {
					Rect iconRect = GUILayoutUtility.GetLastRect ();
					iconRect.y -= 5;
					iconRect.height = UBER_icon.height;//64;
					iconRect.width = UBER_icon.width;//*(iconRect.height/UBER_icon.height);
					iconRect.x = EditorGUIUtility.currentViewWidth - iconRect.width - 15;
					iconRect.x = GUILayoutUtility.GetLastRect ().xMax > iconRect.x ? GUILayoutUtility.GetLastRect ().xMax : iconRect.x;
					//					iconRect.x+=boxRect.width-8;
					//					iconRect.y-=3;
					GUI.DrawTexture (iconRect, UBER_icon, ScaleMode.StretchToFill);
				}
				ShadowCullingPopup ();
				EditorGUILayout.EndVertical();
				if (GUILayout.Button ("Open texture channel mixer", GUILayout.MaxWidth (250))) {
					UBER_TextureChannelMixer window = EditorWindow.GetWindow (typeof(UBER_TextureChannelMixer)) as UBER_TextureChannelMixer;
					window.target_mat = material;
#if UNITY_5_0
					window.title = "Texture mixer";
#else
					window.titleContent.text = "Texture mixer";
#endif
					window.minSize = new Vector2 (450, 766);
					window.maxSize = new Vector2 (460, 768);
				}
//				if (GUILayout.Button ("Purge junk keywords", GUILayout.MaxWidth (250))) {
//					string[] keywords=new string[] { "DIRECTIONAL", "DIRECTIONAL_COOKIE", "DIRLIGHTMAP_COMBINED", "DIRLIGHTMAP_OFF", "DIRLIGHTMAP_SEPARATE", "DYNAMICLIGHTMAP_OFF", "DYNAMICLIGHTMAP_ON", "ENABLE_LOD_FADE", "FOG_EXP", "FOG_EXP2", "FOG_LINEAR", "HDR_LIGHT_PREPASS_OFF", "HDR_LIGHT_PREPASS_ON", "LIGHTMAP_OFF", "LIGHTMAP_ON", "LOD_FADE_CROSSFADE", "LOD_FADE_PERCENTAGE", "POINT", "POINT_COOKIE", "SHADOWS_CUBE", "SHADOWS_DEPTH", "SHADOWS_NATIVE", "SHADOWS_NONATIVE", "SHADOWS_OFF", "SHADOWS_SCREEN", "SHADOWS_SINGLE_CASCADE", "SHADOWS_SOFT", "SHADOWS_SPLIT_SPHERES", "SILHOUETTE_CURVATURE_BASIC", "SILHOUETTE_CURVATURE_MAPPED", "SOFTPARTICLES_OFF", "SOFTPARTICLES_ON", "SPOT", "UNITY_COLORSPACE_GAMMA", "UNITY_HDR_ON", "VERTEXLIGHT_ON", "V_WIRE_IBL_OFF", "V_WIRE_LIGHT_OFF", "_ALPHABLEND_ON", "_ALPHAPREMULTIPLY_ON", "_ALPHATEST_ON", "_CHROMATIC_ABERRATION", "_CURVATUREPRECOMPUTEDFLAG_ON", "_DEPTHWRITE_ON", "_DETAIL_MULX2", "_DETAIL_SIMPLE", "_DETAIL_TEXTURED", "_DETAIL_TEXTURED_WITH_SPEC_GLOSS", "_DIFFUSE_SCATTER", "_EMISSION", "_EMISSION_ANIMATED", "_EMISSION_SIMPLE", "_EMISSION_TEXTURED", "_FROSTFROMGLOBAL_ON", "_GLITTER", "_GLITTER_ON", "_LIGHTMAPPING_DYNAMIC_LIGHTMAPS", "_METALLICGLOSSMAP", "_NORMALMAP", "_OCCLUSION_FROM_ALBEDO_ALPHA", "_PANEMISSIONMASK_ON", "_PARALLAXMAP", "_PARALLAXMAP", "_PARALLAXMAP_2MAPS", "_PARALLAX_POM", "_PARALLAX_POM_SHADOWS", "_PARALLAX_POM_SHADOWS_ZWRITE", "_PARALLAX_POM_ZWRITE", "_POMPRECOMPUTEDFLAG_ON", "_POMSHADOWS_ON", "_POM_DISTANCE_MAP", "_POM_DISTANCE_MAP_SHADOWS", "_POM_DISTANCE_MAP_ZWRITE", "_POM_EXTRUDEVOLUME_ON", "_POM_EXTRUSION_MAP", "_POM_EXTRUSION_MAP_SHADOWS", "_POM_EXTRUSION_MAP_ZWRITE", "_POM_MESHISVOLUME_ON", "_POM_ON", "_PULSATEEMISSION_ON", "_RAININTENSITYFROMGLOBAL_ON", "_RIPPLESTRENGTHFROMGLOBAL_ON", "_SNOW", "_SNOWBUMPMICROFROMGLOBAL_ON", "_SNOWDISSOLVEFROMGLOBAL_ON", "_SNOWGLITTERCOLORFROMGLOBAL_ON", "_SNOWLEVELFROMGLOBAL_ON", "_SNOWSPECGLOSSFROMGLOBAL_ON", "_SNOWWORLDMAPPING_ON", "_SNOW_ON", "_SPECGLOSSMAP", "_TESSELLATION_DISPLACEMENT", "_TEST10_ON", "_TEST11_ON", "_TEST12_ON", "_TEST13_ON", "_TEST14_ON", "_TEST15_ON", "_TEST16_ON", "_TEST1_ON", "_TEST2_ON", "_TEST3_ON", "_TEST4_ON", "_TEST5_ON", "_TEST6_ON", "_TEST7_ON", "_TEST8_ON", "_TEST9_ON", "_TESTING1_ON", "_TESTING2_ON", "_TESTING3_ON", "_TRANSLUCENCY", "_TRANSLUCENCY_ON", "_TRIPLANAR_WORLD_MAPPING", "_UVSEC_UV1", "_UV_CLIP_ON", "_WETDROPLETS_ON", "_WETNESSCONSTFROMGLOBAL_ON", "_WETNESSEMISSIVENESSWRAP_ON", "_WETNESSFLOWGLOBALTIME_ON", "_WETNESSLEVELFROMGLOBAL_ON", "_WETNESS_DROPLETS", "_WETNESS_FULL", "_WETNESS_NONE", "_WETNESS_ON", "_WETNESS_RIPPLES", "_WETNESS_SIMPLE", "_WETRIPPLES_ON" , "_WETNESS_NONE", "_WETNESS_SIMPLE", "_WETNESS_RIPPLES", "_WETNESS_DROPLETS", "_WETNESS_FULL", "_ALPHATEST_ON", "_ALPHABLEND_ON", "_ALPHAPREMULTIPLY_ON", "_SPECGLOSSMAP", "_DETAIL_SIMPLE", "_DETAIL_TEXTURED", "_DETAIL_TEXTURED_WITH_SPEC_GLOSS", "_PARALLAXMAP", "_PARALLAX_POM", "_PARALLAX_POM_ZWRITE", "_PARALLAX_POM_SHADOWS", "_PARALLAXMAP", "_PARALLAXMAP_2MAPS", "_SNOW", "_TRANSLUCENCY", "_DIFFUSE_SCATTER", "_GLITTER", "_TESSELLATION_DISPLACEMENT"};
//					foreach(string key in keywords) {
//						material.DisableKeyword(key);
//						//Debug.Log (key+" "+material.IsKeywordEnabled(key));
//					}
//				}
				GUILayout.Space (15);

                Color bCol = GUI.backgroundColor;

                ////////////////////////////////////////
                //
                // preset functionality
                //
                GUI.backgroundColor = new Color (0.9f, 1.0f, 0.9f, 0.9f);
				EditorGUILayout.BeginVertical ("Button");
				GUI.backgroundColor = bCol;
				
				GUILayout.Label (Styles.presetCollectionSectionText.text, EditorStyles.boldLabel);
				
				if (true) {//_PresetsShown.floatValue==1) {
					Color col = GUI.color;
					GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
					Rect rect = GUILayoutUtility.GetLastRect ();
					rect.x += EditorGUIUtility.currentViewWidth - 47;
					//rect.height-=EditorGUIUtility.singleLineHeight;
					
					EditorGUI.BeginChangeCheck ();
					float nval = EditorGUI.Foldout (rect, _PresetsShown.floatValue == 1, "") ? 1 : 0;
					if (EditorGUI.EndChangeCheck ()) {
						_PresetsShown.floatValue = nval;
					}
					
					GUI.color = col;
				}
				if (_PresetsShown.floatValue == 1 || _PresetsShown.hasMixedValue) {
					
					UBER_MaterialPresetCollection newPresetCollection = (UBER_MaterialPresetCollection)EditorGUILayout.ObjectField (Styles.presetCollectionText, presetCollection, typeof(UBER_MaterialPresetCollection), false);
					if (newPresetCollection != presetCollection) {
						if (newPresetCollection != null) {
							curPresetCollectionPath = AssetDatabase.GetAssetPath (newPresetCollection);
						}
						presetCollection = newPresetCollection;
						SaveCollectionReferenceInThisScene ();
					}
					GUI.backgroundColor = new Color (0.7f, 0.8f, 1.0f, 1.0f);
					if (GUILayout.Button ("Create New Preset Collection")) {
						string path = EditorUtility.SaveFilePanel ("Save Preset Collection", System.IO.Path.GetDirectoryName (curPresetCollectionPath), System.IO.Path.GetFileNameWithoutExtension (curPresetCollectionPath), System.IO.Path.GetExtension (curPresetCollectionPath).Substring (1));
						if (!string.IsNullOrEmpty (path)) {
							curPresetCollectionPath = "Assets" + path.Substring (Application.dataPath.Length);
							presetCollection = ScriptableObject.CreateInstance<UBER_MaterialPresetCollection> ();
							if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (curPresetCollectionPath) != null) {
								AssetDatabase.DeleteAsset (curPresetCollectionPath); // overwrite and create new empty preset collection
							}
							AssetDatabase.CreateAsset (presetCollection, curPresetCollectionPath);
							AssetDatabase.SaveAssets ();
							AssetDatabase.Refresh ();
							SaveCollectionReferenceInThisScene ();
						}
						
					}
					GUI.backgroundColor = bCol;
					
					EditorGUILayout.Space ();
					
					if (presetCollection != null) {
						if (presetCollection.currentPresetName == null || presetCollection.currentPresetName == "") {
							presetCollection.currentPresetName = material.name;
						}
						
						int currentPresetIndex = presetCollection.PresetIndex (presetCollection.currentPresetName);
						if (presetCollection.matPresets != null && presetCollection.matPresets.Length > 0) {
							int newPresetIndex = EditorGUILayout.Popup (Styles.presetInCollectionText.text, currentPresetIndex, presetCollection.names);
							if (newPresetIndex >= 0) {
								currentPresetIndex = newPresetIndex;
								presetCollection.currentPresetName = presetCollection.matPresets [currentPresetIndex].name;
							}
						} else {
							//EditorGUILayout.Popup(-1, new string[] {""});
						}
						
						EditorGUI.BeginChangeCheck ();
						presetCollection.currentPresetName = EditorGUILayout.TextField (Styles.currentPresetNameText, presetCollection.currentPresetName);
						if (EditorGUI.EndChangeCheck ()) {
							currentPresetIndex = presetCollection.PresetIndex (presetCollection.currentPresetName);
						}
						
						EditorGUILayout.BeginHorizontal ();
						GUI.backgroundColor = currentPresetIndex == -1 ? new Color (0.8f, 0.9f, 1.0f, 1.0f) : new Color (1.0f, 0.9f, 0.6f, 1.0f);
						if (GUILayout.Button (currentPresetIndex == -1 ? new GUIContent ("Save", "Save current material state into preset") : new GUIContent ("Update", "Overwrite preset with current material.\nShift+click to skip notification dialogs."), GUILayout.Height (25))) {
							bool saveFlag = false;
							if (currentPresetIndex != -1) {
								if (Event.current.shift || EditorUtility.DisplayDialog ("UBER Notification", "Overwrite existing preset ?", "OK", "Cancel")) {
									saveFlag = true;
								}
							} else {
								saveFlag = true;
							}
							if (saveFlag) {
								presetCollection.AddPreset (material, presetCollection.currentPresetName);
								EditorUtility.SetDirty(presetCollection);
								AssetDatabase.SaveAssets ();
								AssetDatabase.Refresh ();
							}
						}
						GUI.backgroundColor = bCol;
						
						EditorGUI.BeginDisabledGroup (currentPresetIndex == -1);
						GUI.backgroundColor = new Color (1.0f, 0.5f, 0.4f, 1.0f);
						if (GUILayout.Button (new GUIContent ("Delete", "Delete preset.\nShift+click to skip notification dialogs."), GUILayout.Height (25))) {
							if (Event.current.shift || EditorUtility.DisplayDialog ("UBER Notification", "Delete preset from the collection ?", "OK", "Cancel")) {
								presetCollection.RemovePreset (currentPresetIndex);
							}
						}
						GUI.backgroundColor = bCol;
						
						EditorGUILayout.BeginHorizontal ("Box");
						GUI.backgroundColor = new Color (0.7f, 1.0f, 0.8f, 1.0f);
						if (GUILayout.Button (new GUIContent ("Restore", "Restore material state to from preset.\nShift+click to skip notification dialogs."))) {
							if (Event.current.shift || EditorUtility.DisplayDialog ("UBER Notification", "Restore material props ?", "OK", "Cancel")) {
								presetCollection.matPresets [currentPresetIndex].RestoreProps (material, presetCollection.whatToRestore);
								// TODO - name of shader doesn't change in material header... don't know how to force it
								// (below doesn't work)
								//EditorUtility.SetDirty(material);
							}
						}
						GUI.backgroundColor = bCol;
						presetCollection.whatToRestore = (UBER_PresetParamSection)EditorGUILayout.Popup ("", (int)presetCollection.whatToRestore, Styles.PresetParamSectionNames, GUILayout.MaxWidth (100));
						EditorGUILayout.EndHorizontal ();
						
						EditorGUI.EndDisabledGroup ();
						EditorGUILayout.EndHorizontal (); 
					}
					
				}
				
				EditorGUILayout.EndVertical ();
				//
				// presets
				//
				//////////////////////////////////////////////
				/// 
				EditorGUILayout.Space ();

                /*
                //
                // decal mask
                //
                if (_DecalMaskGUI != null)
                {
                    bCol = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 1.0f, 0.7f);
                    EditorGUILayout.BeginVertical("Button");
                    GUI.backgroundColor = bCol;
                    EditorGUILayout.LabelField("Decal properties", EditorStyles.boldLabel);
                    EditorGUI.showMixedValue = _DecalMaskGUI.hasMixedValue;
                    EditorGUI.BeginChangeCheck();
                    ShaderPropertyTooltip(_DecalMaskGUI, Styles.decalMaskText, 0);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _DecalMask.floatValue = _DecalMaskGUI.floatValue / 3.0f;
                    }
                    EditorGUI.showMixedValue = false;
                    if (_Snow.floatValue>0)
                    {
                        EditorGUI.showMixedValue = _DecalMaskGUI.hasMixedValue;
                        EditorGUI.BeginChangeCheck();
                        // ShaderPropertyTooltip(_DecalMaskForSnowGUI, Styles.decalMaskForSnowText, 0);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _DecalMaskForSnow.floatValue = _DecalMaskForSnowGUI.floatValue / 3.0f;
                        }
                        EditorGUI.showMixedValue = false;
                        ShaderPropertyTooltip(_DecalMaskForSnowThreshold, Styles.decalMaskForSnowThresholdText, 1);
                    }
                    if (_Pierceable!=null) {
                        if (_Pierceable.floatValue > 0)
                        {
                            EditorGUILayout.HelpBox("When set remember to use with companion of PierceableObject component!", MessageType.Warning);
                        }
                        ShaderPropertyTooltip(_Pierceable, Styles._PierceableText, 0);
                        //if (_PiercingThreshold != null)
                        //{
                        //    ShaderPropertyTooltip(_PiercingThreshold, Styles._PiercingThresholdText, 1);
                        //}
                    }
                    EditorGUILayout.EndVertical();
                }
                */
				
				// Refraction
				if (_Refraction_Used || _Refraction_hasMixedValue) {// && (BlendMode)blendMode.floatValue==BlendMode.Transparent) {
					GUI.backgroundColor = new Color (1.0f, 1.0f, 1.0f, 0.6f);
					EditorGUILayout.BeginVertical ("Button");
					GUILayout.Space (5);
					GUI.backgroundColor = bCol;
					ShaderPropertyTooltip (_Refraction, Styles._RefractionText, 0);
					ShaderPropertyTooltip (_RefractionBumpScale, Styles._RefractionBumpScaleText, 0);
					ShaderPropertyTooltip (_RefractionChromaticAberration, Styles._RefractionChromaticAberrationText, 0);
					GUILayout.Space (3);
					EditorGUILayout.EndVertical ();
				}
				
				// Triplanar
				if (_Triplanar_Used || _Triplanar_hasMixedValue) {
					GUI.backgroundColor = new Color (0.95f, 0.95f, 0.5f, 0.7f);
					EditorGUILayout.BeginVertical ("Button");
					GUI.backgroundColor = bCol;
					
					GUILayout.Label (Styles.triplanarSectionText, EditorStyles.boldLabel);
					// show/hide block
					if (true) {//_TriplanarShown.floatValue==1) {
						Color col = GUI.color;
						GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
						Rect rect = GUILayoutUtility.GetLastRect ();
						rect.x += EditorGUIUtility.currentViewWidth - 47;
						//rect.height-=EditorGUIUtility.singleLineHeight;
						
						EditorGUI.BeginChangeCheck ();
						float nval = EditorGUI.Foldout (rect, _TriplanarShown.floatValue == 1, "") ? 1 : 0;
						if (EditorGUI.EndChangeCheck ()) {
							_TriplanarShown.floatValue = nval;
						}
						
						GUI.color = col;
					}
					if (_TriplanarShown.floatValue == 1 || _TriplanarShown.hasMixedValue) {
						ShaderPropertyTooltip (_TriplanarWorldMapping, Styles._TriplanarWorldMappingText, 1);
						ShaderPropertyTooltip (_MainTexAverageColor, Styles._MainTexAverageColorText, 1);
						if (_TwoLayers_Used || _TwoLayers_hasMixedValue) {
							ShaderPropertyTooltip (_MainTex2AverageColor, Styles._MainTex2AverageColorText, 1);
						}
						ShaderPropertyTooltip (_TriplanarBlendSharpness, Styles._TriplanarBlendSharpnessText, 1);
						ShaderPropertyTooltip (_TriplanarNormalBlendSharpness, Styles._TriplanarNormalBlendSharpnessText, 2);
						EditorGUI.BeginDisabledGroup (heightMap.textureValue == null);
						ShaderPropertyTooltip (_TriplanarHeightmapBlendingValue, Styles._TriplanarHeightmapBlendingValueText, 1);
						EditorGUI.EndDisabledGroup ();
						ShaderPropertyTooltip (_TriplanarBlendAmbientOcclusion, Styles._TriplanarBlendAmbientOcclusionText, 1);
					}
					EditorGUILayout.EndVertical ();
				}
				
				// Primary properties
				GUI.backgroundColor = new Color (0.95f, 0.95f, 1.0f, 0.7f);
				EditorGUILayout.BeginVertical ("Button");
				GUI.backgroundColor = bCol;
				
				GUILayout.Label (Styles.primaryMapsText, EditorStyles.boldLabel);
				// show/hide block
				if (true) {//_MainShown.floatValue==1) {
					Color col = GUI.color;
					GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
					Rect rect = GUILayoutUtility.GetLastRect ();
					rect.x += EditorGUIUtility.currentViewWidth - 47;
					//rect.height-=EditorGUIUtility.singleLineHeight;
					
					EditorGUI.BeginChangeCheck ();
					float nval = EditorGUI.Foldout (rect, _MainShown.floatValue == 1, "") ? 1 : 0;
					if (EditorGUI.EndChangeCheck ()) {
						_MainShown.floatValue = nval;
					}
					
					GUI.color = col;
				}
				if (_MainShown.floatValue == 1 || _MainShown.hasMixedValue) {
                    bool _AlbedoSmoothnessAvailable = true;
                    if (m_WorkflowMode == WorkflowMode.Specular)
                    {
                        if (specularMap.textureValue || (_TwoLayers_Used && specularMap2.textureValue) || _SpecularRGBGlossADetail.textureValue)
                        {
                            _AlbedoSmoothnessAvailable = false;
                        }
                    }
                    else if (m_WorkflowMode == WorkflowMode.Metallic)
                    {
                        if (metallicMap.textureValue || (_TwoLayers_Used && metallicMap2.textureValue) || _MetallicGlossMapDetail.textureValue)
                        {
                            _AlbedoSmoothnessAvailable = false;
                        }
                    }
                    _AlbedoSmoothnessAvailable = _AlbedoSmoothnessAvailable && ((BlendMode)material.GetFloat("_Mode") == BlendMode.Opaque);
                    _AlbedoSmoothnessAvailable = _AlbedoSmoothnessAvailable && (Smoothness_from_albedo_alpha != null);

                    DoAlbedoArea (material, _AlbedoOcclusion_Used, _TwoLayers_Used, _AlbedoSmoothnessAvailable);
                    if (_FORCE_DIFFAO_Used)
                    {
                        ShaderPropertyTooltip(diffuseTintArray0, new GUIContent("diffuseTint ColorA 0",""), 1);
                        ShaderPropertyTooltip(diffuseTintArray1, new GUIContent("diffuseTint ColorA 1", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray2, new GUIContent("diffuseTint ColorA 2", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray3, new GUIContent("diffuseTint ColorA 3", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray4, new GUIContent("diffuseTint ColorA 4", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray5, new GUIContent("diffuseTint ColorA 5", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray6, new GUIContent("diffuseTint ColorA 6", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray7, new GUIContent("diffuseTint ColorA 7", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray8, new GUIContent("diffuseTint ColorA 8", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray9, new GUIContent("diffuseTint ColorA 9", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray10, new GUIContent("diffuseTint ColorA 10", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray11, new GUIContent("diffuseTint ColorA 11", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray12, new GUIContent("diffuseTint ColorA 12", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray13, new GUIContent("diffuseTint ColorA 13", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray14, new GUIContent("diffuseTint ColorA 14", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArray15, new GUIContent("diffuseTint ColorA 15", ""), 1);
                    }
                    if (_2ndColor_Used)
                    {
                        EditorGUILayout.Space();
                        ShaderPropertyTooltip(diffuseTintArrayB0, new GUIContent("diffuseTint ColorB 0", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB1, new GUIContent("diffuseTint ColorB 1", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB2, new GUIContent("diffuseTint ColorB 2", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB3, new GUIContent("diffuseTint ColorB 3", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB4, new GUIContent("diffuseTint ColorB 4", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB5, new GUIContent("diffuseTint ColorB 5", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB6, new GUIContent("diffuseTint ColorB 6", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB7, new GUIContent("diffuseTint ColorB 7", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB8, new GUIContent("diffuseTint ColorB 8", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB9, new GUIContent("diffuseTint ColorB 9", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB10, new GUIContent("diffuseTint ColorB 10", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB11, new GUIContent("diffuseTint ColorB 11", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB12, new GUIContent("diffuseTint ColorB 12", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB13, new GUIContent("diffuseTint ColorB 13", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB14, new GUIContent("diffuseTint ColorB 14", ""), 1);
                        ShaderPropertyTooltip(diffuseTintArrayB15, new GUIContent("diffuseTint ColorB 15", ""), 1);
                    }
                    DoSpecularMetallicArea (_TwoLayers_Used);
					
					EditorGUI.BeginChangeCheck ();
					if (!_NONORMAL_Used) {
						m_MaterialEditor.TexturePropertySingleLine (Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
					}
					if (_TwoLayers_Used) {
						m_MaterialEditor.TexturePropertySingleLine (new GUIContent (Styles.normalMapText.text + " 2", Styles.normalMapText.tooltip), bumpMap2, bumpMap2.textureValue != null ? bumpScale2 : null);
					}
					if (EditorGUI.EndChangeCheck ()) {
						FixSubstanceTexelSizeBug(props);
					}
					
					if (_BendNormals_Used) {
						GUI.backgroundColor = new Color (0.5f, 0.5f, 1.0f, 1);
						EditorGUILayout.BeginVertical ("Box");
						GUI.backgroundColor = bCol;
						ShaderPropertyTooltip (_BendNormalsFreq, Styles._BendNormalsFreqText, 1);
						ShaderPropertyTooltip (_BendNormalsStrength, Styles._BendNormalsStrengthText, 1);
						EditorGUILayout.EndVertical ();
					}
					
					
					EditorGUI.BeginChangeCheck ();
					if (_ExtrusionMap_Used) {
						m_MaterialEditor.TexturePropertySingleLine (Styles.extrusionMapText, heightMap, null);
					} else if (_DistanceMap_Used) {
						m_MaterialEditor.TexturePropertySingleLine (Styles.distanceMapText, heightMap, null);
					} else {
						m_MaterialEditor.TexturePropertySingleLine (_TwoLayers_Used ? Styles.heightMap2LText : Styles.heightMapText, heightMap, ((heightMap.textureValue != null && (_POM.floatValue == 0 || _Triplanar_Used)) || _TwoLayers_Used) && !_Tessellation_Used ? heigtMapScale : null);
						if (_TwoLayers_Used) {
							m_MaterialEditor.TexturePropertySingleLine (new GUIContent (Styles.heightMapText.text + " 2", _TwoLayers_Used ? Styles.heightMap2LText.tooltip : Styles.heightMapText.tooltip), heightMap2, ((heightMap2.textureValue != null && (_POM.floatValue == 0 || _Triplanar_Used)) || _TwoLayers_Used) && !_Tessellation_Used ? heigtMapScale2 : null);
						}
					}
					if (EditorGUI.EndChangeCheck ()) {
						FixSubstanceTexelSizeBug(props);
					}
					
					//EditorGUILayout.Space();
					if (_AlbedoOcclusion_Used && (!_AlbedoSmoothnessAvailable || Smoothness_from_albedo_alpha.floatValue==0)) {
                        if (!_FORCE_DIFFAO_Used) {
                            EditorGUILayout.HelpBox("When Occlusion map is not specified it's taken from Albedo (A) channel. Otherwise when select UV1 we get both occlusions superimposed. Does not apply to transparent modes.", MessageType.Info);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("With forced AO from diffuse (checked in shader tag) occlusion map acts only as coloring mask", MessageType.Warning);
                        }
                    }
                    if (_TwoLayers_Used) {
						m_MaterialEditor.TexturePropertySingleLine (_UVSecOcclusion.floatValue == 0 ? (_FORCE_DIFFAO_Used ? Styles.maskingText2 : Styles.occlusionText2) : (_FORCE_DIFFAO_Used ? Styles.maskingText : Styles.occlusionText), occlusionMap, occlusionMap.textureValue != null && _UVSecOcclusion.floatValue == 0 && !_FORCE_DIFFAO_Used ? occlusionStrength : null, null);
						if (occlusionMap.textureValue != null && _UVSecOcclusion.floatValue == 0 && !_FORCE_DIFFAO_Used) {
							ShaderPropertyTooltip (occlusionStrength2, Styles._SecOcclusionStrengthText, 2);
						}
					} else {
						m_MaterialEditor.TexturePropertySingleLine ((_FORCE_DIFFAO_Used ? Styles.maskingText : Styles.occlusionText), occlusionMap, occlusionMap.textureValue != null && _UVSecOcclusion.floatValue == 0 && !_FORCE_DIFFAO_Used ? occlusionStrength : null);
					}
					if (occlusionMap.textureValue != null && !_FORCE_DIFFAO_Used) {
						ShaderPropertyTooltip (_UVSecOcclusion, Styles._UVSecOcclusionText, 2);
						if (_UVSecOcclusion.floatValue == 1) {
							// secondary occlusion
							ShaderPropertyTooltip (_SecOcclusionStrength, Styles._SecOcclusionStrengthText, 3);
							ShaderPropertyTooltip (_UVSecOcclusionLightmapPacked, Styles._UVSecOcclusionLightmapPackedText, 3);
						}
					} else {
						_UVSecOcclusion.floatValue = 0;
					}
					
					DoEmissionArea (material, _Animated_Emission);
					
					EditorGUI.BeginChangeCheck ();
					m_MaterialEditor.TextureScaleOffsetProperty (albedoMap);
					if (EditorGUI.EndChangeCheck ()) {
						emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake
					}
				}
				EditorGUILayout.EndVertical ();
				
				// Secondary properties
				GUI.backgroundColor = new Color (0.95f, 1.0f, 0.98f, 0.4f);
				EditorGUILayout.BeginVertical ("Button");
				GUI.backgroundColor = bCol;
				
				GUILayout.Label (Styles.secondaryMapsText, EditorStyles.boldLabel);
				// show/hide block
				if (true) {//_SecondaryShown.floatValue==1) {
					Color col = GUI.color;
					GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
					Rect rect = GUILayoutUtility.GetLastRect ();
					rect.x += EditorGUIUtility.currentViewWidth - 47;
					//rect.height-=EditorGUIUtility.singleLineHeight;
					
					EditorGUI.BeginChangeCheck ();
					float nval = EditorGUI.Foldout (rect, _SecondaryShown.floatValue == 1, "") ? 1 : 0;
					if (EditorGUI.EndChangeCheck ()) {
						_SecondaryShown.floatValue = nval;
					}
					
					GUI.color = col;
				}
				if (_SecondaryShown.floatValue == 1 || _SecondaryShown.hasMixedValue) {
					m_MaterialEditor.TexturePropertySingleLine (Styles.detailMaskText, detailMask);
					if (!_TwoLayers_Used) {
						
						EditorGUI.BeginDisabledGroup (detailMask.textureValue == null);
						ShaderPropertyTooltip (_DetailUVMult, Styles._DetailUVMultText, 0);
						EditorGUI.EndDisabledGroup ();
						
						if (m_WorkflowMode == WorkflowMode.Specular) {
							m_MaterialEditor.TexturePropertySingleLine (Styles.detailAlbedoText, detailAlbedoMap, _DetailColor);
						} else {
							m_MaterialEditor.TexturePropertySingleLine (Styles.detailAlbedoTextMetal, detailAlbedoMap, _DetailColor);
						}
						
						ShaderPropertyTooltip (_DetailEmissiveness, Styles._DetailEmissivenessText, 3);
						DoSpecularMetallicDetailArea ();
						
						//
						m_MaterialEditor.TexturePropertySingleLine (Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
						
						if (detailNormalMap.textureValue != null)
							ShaderPropertyTooltip (_DetailNormalLerp, Styles._DetailNormalLerpText, 3);
						//
					}
					m_MaterialEditor.TextureScaleOffsetProperty (detailAlbedoMap);
					EditorGUI.BeginDisabledGroup (_Triplanar_Used && !_Triplanar_hasMixedValue);
					ShaderPropertyTooltip (uvSetSecondary, Styles.uvSetLabel, 0);
					EditorGUI.EndDisabledGroup();
				}
				EditorGUILayout.EndVertical ();
				
				//
				EditorGUILayout.Space ();
				
				if ((_DistanceMap_Used || _ExtrusionMap_Used || _Tessellation_Used || heightMap.textureValue != null) || (heightMap2.textureValue != null && _TwoLayers_Used)) {
					//EditorGUILayout.Space();
					GUI.backgroundColor = new Color (1.0f, 0.98f, 0.96f, 0.9f);
					EditorGUILayout.BeginVertical ("Button");
					GUI.backgroundColor = bCol;
					if (_ExtrusionMap_Used || _DistanceMap_Used) {
						if (_DistanceMap_Used) {
							EditorGUILayout.LabelField ("Distance mapped POM", EditorStyles.boldLabel);
						} else {
							EditorGUILayout.LabelField ("Extrusion mapped POM", EditorStyles.boldLabel);
						}
						{
							Color col = GUI.color;
							GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
							Rect rect = GUILayoutUtility.GetLastRect ();
							rect.x += EditorGUIUtility.currentViewWidth - 47;
							
							EditorGUI.BeginChangeCheck ();
							float nval = EditorGUI.Foldout (rect, _POMShown.floatValue == 1, "") ? 1 : 0;
							if (EditorGUI.EndChangeCheck ()) {
								_POMShown.floatValue = nval;
							}
							
							GUI.color = col;
						}
						if (_POMShown.floatValue == 1 || _POMShown.hasMixedValue) {
							GUILayout.Space (4);
							
							ShaderPropertyTooltip (_Depth, Styles._DepthText, 1);
							if (_ExtrusionMap_Used) {
								ShaderPropertyTooltip (_DistSteps, Styles._DistStepsText, 1);
								ShaderPropertyTooltip (_ReliefMIPbias, Styles._ReliefMIPbiasText, 1);
							}
							ShaderPropertyTooltip (_DepthReductionDistance, Styles._DepthReductionDistanceText, 1);
							
							ShaderPropertyTooltip (_POMPrecomputedFlag, Styles._POMPrecomputedFlagText, 1);
							
							if (_POMPrecomputedFlag.floatValue == 0) {
								ShaderPropertyTooltip (_Tan2ObjCustomU, Styles._Tan2ObjCustomUText, 2);
								ShaderPropertyTooltip (_Tan2ObjCustomV, Styles._Tan2ObjCustomVText, 2);
							}
							
							ShaderPropertyTooltip (_UV_Clip, Styles._UV_ClipText, 1);
							if (_UV_Clip.floatValue > 0) {
								EditorGUILayout.BeginHorizontal ();
								GUILayout.Space (34);
								GUI_UV_Borders (_UV_Clip_Borders);
								EditorGUILayout.EndHorizontal ();
							}
							ShaderPropertyTooltip (_POM_BottomCut, Styles._POM_BottomCutText, 2);
							ShaderPropertyTooltip (_POM_MeshIsVolume, Styles._POM_MeshIsVolumeText, 1);
							ShaderPropertyTooltip (_POM_ExtrudeVolume, Styles._POM_ExtrudeVolumeText, 1);
							
							EditorGUILayout.Space ();
							
							//ShaderPropertyTooltip(_POMShadows, Styles._POMShadowsText, 1);
							EditorGUILayout.BeginHorizontal ();
							GUILayout.Space (17);
							
							EditorGUI.showMixedValue = _POMShadows.hasMixedValue;
							EditorGUI.BeginChangeCheck ();
							float nval = EditorGUILayout.ToggleLeft (Styles._POMShadowsText, _POMShadows.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (150)) ? 1 : 0;
							if (_POMShadows.floatValue == 0 && nval == 1) {
								_DepthWrite.floatValue = 0;
							}
							if (EditorGUI.EndChangeCheck ()) {
								_POMShadows.floatValue = nval;
							}
							EditorGUI.showMixedValue = false;
							
							EditorGUI.showMixedValue = _DepthWrite.hasMixedValue;
							EditorGUI.BeginChangeCheck ();
							nval = EditorGUILayout.ToggleLeft (Styles._DepthWriteText, _DepthWrite.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (150)) ? 1 : 0;
							if (_DepthWrite.floatValue == 0 && nval == 1) {
								_POMShadows.floatValue = 0;
							}
							if (EditorGUI.EndChangeCheck ()) {
								_DepthWrite.floatValue = nval;
							}
							EditorGUI.showMixedValue = false;
							EditorGUILayout.EndHorizontal ();
							
							if (_POMShadows.floatValue != 0) {
								ShaderPropertyTooltip (_ShadowStrength, Styles._ShadowStrengthText, 2);
								ShaderPropertyTooltip (_Softness, Styles._SoftnessText, 2);
							}
							
						}
					} else if (_Tessellation_Used) {
						EditorGUILayout.LabelField ("Tessellation", EditorStyles.boldLabel);
						{
							Color col = GUI.color;
							GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
							Rect rect = GUILayoutUtility.GetLastRect ();
							rect.x += EditorGUIUtility.currentViewWidth - 47;
							
							EditorGUI.BeginChangeCheck ();
							float nval = EditorGUI.Foldout (rect, _POMShown.floatValue == 1, "") ? 1 : 0;
							if (EditorGUI.EndChangeCheck ()) {
								_POMShown.floatValue = nval;
							}
							
							GUI.color = col;
						}
						{
							Rect rect = GUILayoutUtility.GetLastRect ();
							rect.x += EditorGUIUtility.currentViewWidth - 170;
							rect.width = 104;
							if (alt_shader_inited && alt_shader != "" && GUI.Button (rect, "switch to Parallax", EditorStyles.miniButton)) {
								material.shader = Shader.Find (alt_shader);
								alt_shader_inited = false;
								//return;
							}
						}
						if (_POMShown.floatValue == 1 || _POMShown.hasMixedValue) {
							GUILayout.Space (4);
							
							//EditorGUILayout.BeginVertical("Box");
							ShaderPropertyTooltip (_Phong, Styles._PhongText, 1);
							//EditorGUILayout.EndVertical();
							
							GUILayout.Space (6);
							
							ShaderPropertyTooltip (_TessDepth, Styles._TessDepthText, 1);
							EditorGUI.BeginDisabledGroup(_TessDepth.floatValue==0);
							ShaderPropertyTooltip (_TessOffset, Styles._TessOffsetText, 2);
							EditorGUI.EndDisabledGroup();
							
							GUILayout.Space (8);
							
							ShaderPropertyTooltip (_Tess, Styles._TessText, 1);
							ShaderPropertyTooltip (minDist, Styles.minDistText, 1);
							ShaderPropertyTooltip (maxDist, Styles.maxDistText, 1);
							ShaderPropertyTooltip (_TessEdgeLengthLimit, Styles._TessEdgeLengthLimitText, 1);
						}
					} else {
						EditorGUI.BeginDisabledGroup (_TwoLayers_Used || _Triplanar_Used);
						if (_POM.floatValue == 1 && !_TwoLayers_Used && !_Triplanar_Used) {
							
							EditorGUI.showMixedValue = _POM.hasMixedValue;
							EditorGUI.BeginChangeCheck ();
							float nval = EditorGUILayout.ToggleLeft (Styles._POMText, _POM.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (195)) ? 1 : 0;
							if (EditorGUI.EndChangeCheck ()) {
								_POM.floatValue = nval;
							}
							EditorGUI.showMixedValue = false;
							
						} else {
							if (_TwoLayers_Used || _Triplanar_Used) {
								EditorGUILayout.ToggleLeft (Styles._POMText, false, EditorStyles.boldLabel);
							} else {
								EditorGUI.showMixedValue = _POM.hasMixedValue;
								EditorGUI.BeginChangeCheck ();
								float nval = EditorGUILayout.ToggleLeft (Styles._POMText, _POM.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (195)) ? 1 : 0;
								if (EditorGUI.EndChangeCheck ()) {
									_POM.floatValue = nval;
								}
								EditorGUI.showMixedValue = false;
							}
						}
						EditorGUI.EndDisabledGroup ();
						// show/hide block
						if (_POM.floatValue == 1 && !_TwoLayers_Used && !_Triplanar_Used) {
							Color col = GUI.color;
							GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
							Rect rect = GUILayoutUtility.GetLastRect ();
							rect.x += EditorGUIUtility.currentViewWidth - 47;
							
							EditorGUI.BeginChangeCheck ();
							float nval = EditorGUI.Foldout (rect, _POMShown.floatValue == 1, "") ? 1 : 0;
							if (EditorGUI.EndChangeCheck ()) {
								_POMShown.floatValue = nval;
							}
							
							GUI.color = col;
						}
						if (alt_shader_inited && alt_shader != "") {//_POM.floatValue == 1 || _TwoLayers_Used) {
							Rect rect = GUILayoutUtility.GetLastRect ();
							rect.x += EditorGUIUtility.currentViewWidth - 170 + (_TwoLayers_Used ? 22 : 0);
							rect.width = 104;
							if (alt_shader_inited && alt_shader != "" && GUI.Button (rect, "switch to Tess", EditorStyles.miniButton)) {
								material.shader = Shader.Find (alt_shader);
								alt_shader_inited = false;
								return;
							}
						}
						if ((_POMShown.floatValue == 1 || _POMShown.hasMixedValue) && !_TwoLayers_Used && !_Triplanar_Used) {
							GUILayout.Space (4);
						}
					}
					//					float fadeGroupPOM_Dest= _POMShown.floatValue;//(heightMap.textureValue!=null && _POM.floatValue!=0 && _POMShown.floatValue==1 && !_Tessellation_Used && !_TwoLayers_Used) ? 1:0;
					//					if (Mathf.Abs(facdeGroupPOM_Dest-fadeGroupPOM)>0.001f) {
					//						fadeGroupPOM+=(fadeGroupPOM_Dest-fadeGroupPOM)*0.5f;
					//						EditorUtility.SetDirty(material);
					//					} else {
					//						fadeGroupPOM=fadeGroupPOM_Dest;
					//					}
					//					EditorGUILayout.BeginFadeGroup(fadeGroupPOM);
					if (heightMap.textureValue != null && _POM.floatValue != 0 && (_POMShown.floatValue == 1 || _POMShown.hasMixedValue) && !_Tessellation_Used && !_DistanceMap_Used && !_ExtrusionMap_Used && !_TwoLayers_Used && !_Triplanar_Used) {
						ShaderPropertyTooltip (_Depth, Styles._DepthText, 1);
						ShaderPropertyTooltip (_DistSteps, Styles._DistStepsText, 1);
						ShaderPropertyTooltip (_ReliefMIPbias, Styles._ReliefMIPbiasText, 1);
						ShaderPropertyTooltip (_DepthReductionDistance, Styles._DepthReductionDistanceText, 1);
						
						EditorGUI.BeginDisabledGroup (_Silhouettes_Used && _POMCurvatureType.floatValue == 1);
						ShaderPropertyTooltip (_POMPrecomputedFlag, Styles._POMPrecomputedFlagText, 1);
						EditorGUI.EndDisabledGroup ();
						if (_Silhouettes_Used)
							ShaderPropertyTooltip (_POMCurvatureType, Styles._POMCurvatureTypeText, 1);
						if (_Silhouettes_Used && _POMCurvatureType.floatValue == 0) {
							if (_POMPrecomputedFlag.floatValue > 0) {
								//ShaderPropertyTooltip(_Tan2ObjMultU, Styles._Tan2ObjMultUText, 2);
								//ShaderPropertyTooltip(_Tan2ObjMultV, Styles._Tan2ObjMultVText, 2);
								ShaderPropertyTooltip (_CurvatureMultU, Styles._CurvatureMultUText, 2);
								ShaderPropertyTooltip (_CurvatureMultV, Styles._CurvatureMultVText, 2);
							} else {
								ShaderPropertyTooltip (_Tan2ObjCustomU, Styles._Tan2ObjCustomUText, 2);
								ShaderPropertyTooltip (_Tan2ObjCustomV, Styles._Tan2ObjCustomVText, 2);
								ShaderPropertyTooltip (_CurvatureCustomU, Styles._CurvatureCustomUText, 2);
								ShaderPropertyTooltip (_CurvatureCustomV, Styles._CurvatureCustomVText, 2);
							}
						} else if (_Silhouettes_Used && _POMCurvatureType.floatValue == 1) {
							_POMPrecomputedFlag.floatValue = 1; // mapped curvature w/o prebaked scales makes not much sense...
							EditorGUILayout.BeginHorizontal ();
							GUILayout.Space (32);
							m_MaterialEditor.TexturePropertySingleLine (Styles._ObjectNormalsTexText, _ObjectNormalsTex);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							GUILayout.Space (32);
							m_MaterialEditor.TexturePropertySingleLine (Styles._ObjectTangentsTexText, _ObjectTangentsTex);
							EditorGUILayout.EndHorizontal ();
						} else {
							if (_POMPrecomputedFlag.floatValue == 0) {
								ShaderPropertyTooltip (_Tan2ObjCustomU, Styles._Tan2ObjCustomUText, 2);
								ShaderPropertyTooltip (_Tan2ObjCustomV, Styles._Tan2ObjCustomVText, 2);
							}
						}
						
						if (_Silhouettes_Used) {
							ShaderPropertyTooltip (_UV_Clip, Styles._UV_ClipText, 1);
							if (_UV_Clip.floatValue > 0) {
								EditorGUILayout.BeginHorizontal ();
								GUILayout.Space (34);
								GUI_UV_Borders (_UV_Clip_Borders);
								EditorGUILayout.EndHorizontal ();
							}
							ShaderPropertyTooltip (_POM_BottomCut, Styles._POM_BottomCutText, 2);
						}
						ShaderPropertyTooltip (_POM_MeshIsVolume, Styles._POM_MeshIsVolumeText, 1);
						
						EditorGUILayout.Space ();
						
						//ShaderPropertyTooltip(_POMShadows, Styles._POMShadowsText, 1);
						EditorGUILayout.BeginHorizontal ();
						GUILayout.Space (17);
						
						EditorGUI.showMixedValue = _POM.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = EditorGUILayout.ToggleLeft (Styles._POMShadowsText, _POMShadows.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (150)) ? 1 : 0;
						if (_POMShadows.floatValue == 0 && nval == 1) {
							_DepthWrite.floatValue = 0;
						}
						if (EditorGUI.EndChangeCheck ()) {
							_POMShadows.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
						
						EditorGUI.showMixedValue = _DepthWrite.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						nval = EditorGUILayout.ToggleLeft (Styles._DepthWriteText, _DepthWrite.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (150)) ? 1 : 0;
						if (_DepthWrite.floatValue == 0 && nval == 1) {
							_POMShadows.floatValue = 0;
						}
						if (EditorGUI.EndChangeCheck ()) {
							_DepthWrite.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
						
						EditorGUILayout.EndHorizontal ();
						if (_POMShadows.floatValue != 0) {
							ShaderPropertyTooltip (_ShadowStrength, Styles._ShadowStrengthText, 2);
							ShaderPropertyTooltip (_DistStepsShadows, Styles._DistStepsShadowsText, 2);
							ShaderPropertyTooltip (_ShadowMIPbias, Styles._ShadowMIPbiasText, 2);
							ShaderPropertyTooltip (_Softness, Styles._SoftnessText, 2);
							ShaderPropertyTooltip (_SoftnessFade, Styles._SoftnessFadeText, 2);
						}
					}
					//					EditorGUILayout.EndFadeGroup();
					EditorGUILayout.EndVertical ();
				}
				
				//EditorGUILayout.Space();
				
				// wetness
				GUI.backgroundColor = new Color (0.7f, 0.7f, 1.0f, 0.8f);
				EditorGUILayout.BeginVertical ("Button");
				GUI.backgroundColor = bCol;
				
				{
					EditorGUI.showMixedValue = _Wetness.hasMixedValue;
					float nval;
					EditorGUI.BeginChangeCheck ();
					if (_Wetness.floatValue == 1) {
						nval = EditorGUILayout.ToggleLeft (Styles._WetnessText, _Wetness.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (EditorGUIUtility.currentViewWidth - 60)) ? 1 : 0;
					} else {
						nval = EditorGUILayout.ToggleLeft (Styles._WetnessText, _Wetness.floatValue == 1, EditorStyles.boldLabel) ? 1 : 0;
					}
					if (EditorGUI.EndChangeCheck ()) {
						_Wetness.floatValue = nval;
					}
					EditorGUI.showMixedValue = false;
				}
				
				// show/hide block
				if (_Wetness.floatValue == 1) {
					Color col = GUI.color;
					GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
					Rect rect = GUILayoutUtility.GetLastRect ();
					rect.x += EditorGUIUtility.currentViewWidth - 47;
					//rect.height-=EditorGUIUtility.singleLineHeight;
					
					EditorGUI.BeginChangeCheck ();
					float nval = EditorGUI.Foldout (rect, _WetnessShown.floatValue == 1, "") ? 1 : 0;
					if (EditorGUI.EndChangeCheck ()) {
						_WetnessShown.floatValue = nval;
					}
					
					GUI.color = col;
				}
				if (_Wetness.floatValue == 1 && (_WetnessShown.floatValue == 1 || _WetnessShown.hasMixedValue)) {
					EditorGUI.BeginDisabledGroup (detailMask.textureValue == null);
					ShaderPropertyTooltip (_WetnessUVMult, Styles._WetnessUVMultText, 0);
					EditorGUI.EndDisabledGroup ();
					
					ShaderPropertyTooltip (_WetnessLevel, Styles._WetnessLevelText, 0);
					ShaderPropertyTooltip (_WetnessConst, Styles._WetnessConstText, 0);
					EditorGUI.BeginDisabledGroup (_Snow.floatValue==0 || _SnowLevelFromGlobal.floatValue==0);
					ShaderPropertyTooltip (_WetnessMergeWithSnowPerMaterial, Styles._WetnessMergeWithSnowPerMaterialText, 0);
					EditorGUI.EndDisabledGroup ();
					
					EditorGUILayout.Space();
					
					ShaderPropertyTooltip (_WetnessColor, Styles._WetnessColorText, 0);
					ShaderPropertyTooltip (_WetnessDarkening, Styles._WetnessDarkeningText, 1);
					ShaderPropertyTooltip (_WetnessSpecGloss, Styles._WetnessSpecGlossText, 1);
					ShaderPropertyTooltip (_WetnessEmissiveness, Styles._WetnessEmissivenessText, 1);
					if (_WetnessEmissiveness.floatValue>0) {
						ShaderPropertyTooltip (_WetnessEmissivenessWrap, Styles._WetnessEmissivenessWrapText, 2);
						GUILayout.Space(7);
					} else {
						GUILayout.Space(3);
					}
					
					ShaderPropertyTooltip (_WetnessNormalInfluence, Styles._WetnessNormalInfluenceText, 1);

                    //EditorGUILayout.BeginHorizontal();
                    //GUILayout.Space(17);
                    
                    EditorGUI.BeginChangeCheck ();
					m_MaterialEditor.TexturePropertySingleLine (_RippleMapWet.textureValue == null || _Snow.floatValue == 0 ? Styles._RippleMapWetText : Styles._RippleMapWetSharedText, _RippleMapWet, _RippleMapWet.textureValue == null ? null : _RippleStrength);
					if (EditorGUI.EndChangeCheck ()) {
						_RippleMap.textureValue = _RippleMapWet.textureValue != null ? _RippleMapWet.textureValue : _RippleMapSnow.textureValue;
						_RippleMapSnow.textureValue = _RippleMapSnow.textureValue!=null && _RippleMapWet.textureValue != null ? _RippleMapWet.textureValue : _RippleMapSnow.textureValue;
					}
					//EditorGUILayout.EndHorizontal();
					_WetRipples.floatValue = (_RippleMapWet.textureValue != null) ? 1 : 0;
					if (_WetRipples.floatValue == 1) {
						ShaderPropertyTooltip (_RippleTiling, Styles._RippleTilingText, 2);
						ShaderPropertyTooltip (_RippleSpecFilter, Styles._RippleSpecFilterText, 1);
						ShaderPropertyTooltip (_RippleAnimSpeed, Styles._RippleAnimSpeedText, 1);
						ShaderPropertyTooltip (_WetnessFlowGlobalTime, Styles._WetnessFlowGlobalTimeText, 2);
						ShaderPropertyTooltip (_FlowCycleScale, Styles._FlowCycleScaleText, 1);
						ShaderPropertyTooltip (_WetnessNormMIP, Styles._WetnessNormMIPText, 1);
						ShaderPropertyTooltip (_WetnessNormStrength, Styles._WetnessNormStrengthText, 1);
					}
					
					//EditorGUILayout.BeginHorizontal();
					//GUILayout.Space(17);
					m_MaterialEditor.TexturePropertySingleLine (Styles._DropletsMapText, _DropletsMap);
					//EditorGUILayout.EndHorizontal();
					_WetDroplets.floatValue = (_DropletsMap.textureValue != null) ? 1 : 0;
					if (_WetDroplets.floatValue == 1) {
						ShaderPropertyTooltip (_DropletsTiling, Styles._DropletsTilingText, 2);
						ShaderPropertyTooltip (_RainIntensity, Styles._RainIntensityText, 1);
						ShaderPropertyTooltip (_DropletsAnimSpeed, Styles._DropletsAnimSpeedText, 1);
						ShaderPropertyTooltip (_RippleRefraction, Styles._RippleRefractionText, 1);
					}
					
					// global toggles
					EditorGUILayout.BeginVertical("Box");
					
					GlobalControlBox();
					
					EditorGUILayout.BeginHorizontal ();
					// level
					{
						EditorGUI.showMixedValue = _WetnessLevelFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_WetnessLevelFromGlobal.floatValue==1 ? true : false), Styles._WetnessLevelFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_WetnessLevelFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					// const
					{
						EditorGUI.showMixedValue = _WetnessConstFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_WetnessConstFromGlobal.floatValue==1 ? true : false), Styles._WetnessConstFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_WetnessConstFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					
					// ripple strength
					{
						EditorGUI.showMixedValue = _RippleStrengthFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_RippleStrengthFromGlobal.floatValue==1 ? true : false), Styles._RippleStrengthFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_RippleStrengthFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					// rain strength
					{
						EditorGUI.showMixedValue = _RainIntensityFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_RainIntensityFromGlobal.floatValue==1 ? true : false), Styles._RainIntensityFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_RainIntensityFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.EndVertical ();
					
				}
				EditorGUILayout.EndVertical ();
				
				//EditorGUILayout.Space();
				
				// translucency
				GUI.backgroundColor = new Color (1.0f, 0.7f, 0.7f, 0.7f);
				EditorGUILayout.BeginVertical ("Button");
				GUI.backgroundColor = bCol;
				
				{
					EditorGUI.showMixedValue = _Translucency.hasMixedValue;
					float nval;
					EditorGUI.BeginChangeCheck ();
					if (_Translucency.floatValue == 1) {
						nval = EditorGUILayout.ToggleLeft (Styles._TranslucencyText, _Translucency.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (EditorGUIUtility.currentViewWidth - 60)) ? 1 : 0;
					} else {
						nval = EditorGUILayout.ToggleLeft (Styles._TranslucencyText, _Translucency.floatValue == 1, EditorStyles.boldLabel) ? 1 : 0;
					}
					if (EditorGUI.EndChangeCheck ()) {
						_Translucency.floatValue = nval;
					}
					EditorGUI.showMixedValue = false;
				}
				
				// show/hide block
				if (_Translucency.floatValue == 1) {
					Color col = GUI.color;
					GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
					Rect rect = GUILayoutUtility.GetLastRect ();
					rect.x += EditorGUIUtility.currentViewWidth - 47;
					//rect.height-=EditorGUIUtility.singleLineHeight;
					
					EditorGUI.BeginChangeCheck ();
					float nval = EditorGUI.Foldout (rect, _TranslucencyShown.floatValue == 1, "") ? 1 : 0;
					if (EditorGUI.EndChangeCheck ()) {
						_TranslucencyShown.floatValue = nval;
					}
					
					GUI.color = col;
				}
				if (_Translucency.floatValue == 1 && (_TranslucencyShown.floatValue == 1 || _TranslucencyShown.hasMixedValue)) {
                    bool deferred_flag = Camera.main && (Camera.main.actualRenderingPath != RenderingPath.DeferredShading);
                    if (deferred_flag)
                    {
                        ShaderPropertyTooltip(_TranslucencyColor, Styles._TranslucencyColorText, 0);
                        if (_TwoLayers_Used)
                        {
                            ShaderPropertyTooltip(_TranslucencyColor2, new GUIContent(Styles._TranslucencyColorText.text + " 2", Styles._TranslucencyColorText.tooltip), 0);
                        }
                    }
                    if (!deferred_flag)
                    {
                        ShaderPropertyTooltip(_TranslucencyDeferredLightIndex, Styles._TranslucencyDeferredLightIndexText, 0);
                    }
					ShaderPropertyTooltip (_TranslucencyStrength, Styles._TranslucencyStrengthText, 0);
					EditorGUILayout.Space ();

                    if (deferred_flag)
                    {
                        ShaderPropertyTooltip(_TranslucencyConstant, Styles._TranslucencyConstantText, 1);
                    }
					ShaderPropertyTooltip (_TranslucencyOcclusion, Styles._TranslucencyOcclusionText, 1);
                    if (deferred_flag)
                    {
                        ShaderPropertyTooltip(_TranslucencySuppressRealtimeShadows, Styles._TranslucencySuppressRealtimeShadowsText, 1);
                        ShaderPropertyTooltip(_TranslucencyNDotL, Styles._TranslucencyNDotLText, 1);
                        EditorGUILayout.Space();

                        ShaderPropertyTooltip(_TranslucencyExponent, Styles._TranslucencyExponentText, 1);
                        ShaderPropertyTooltip(_TranslucencyNormalOffset, Styles._TranslucencyNormalOffsetText, 1);
                        ShaderPropertyTooltip(_TranslucencyPointLightDirectionality, Styles._TranslucencyPointLightDirectionalityText, 1);
                    }
				}
				EditorGUILayout.EndVertical ();
				// (translucency)
				
				//EditorGUILayout.Space();
				
				
				// glitter
				GUI.backgroundColor = new Color (0.7f, 1.0f, 0.7f, 0.75f);
				EditorGUILayout.BeginVertical ("Button");
				GUI.backgroundColor = bCol;
				
				{
					EditorGUI.showMixedValue = _Glitter.hasMixedValue;
					float nval;
					EditorGUI.BeginChangeCheck ();
					if (_Glitter.floatValue == 1) {
						nval = EditorGUILayout.ToggleLeft (Styles._GlitterText, _Glitter.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (EditorGUIUtility.currentViewWidth - 60)) ? 1 : 0;
					} else {
						nval = EditorGUILayout.ToggleLeft (Styles._GlitterText, _Glitter.floatValue == 1, EditorStyles.boldLabel) ? 1 : 0;
					}
					if (EditorGUI.EndChangeCheck ()) {
						_Glitter.floatValue = nval;
					}
					EditorGUI.showMixedValue = false;
				}
				
				// show/hide block
				if (_Glitter.floatValue == 1) {
					Color col = GUI.color;
					GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
					Rect rect = GUILayoutUtility.GetLastRect ();
					rect.x += EditorGUIUtility.currentViewWidth - 47;
					//rect.height-=EditorGUIUtility.singleLineHeight;
					
					EditorGUI.BeginChangeCheck ();
					float nval = EditorGUI.Foldout (rect, _GlitterShown.floatValue == 1, "") ? 1 : 0;
					if (EditorGUI.EndChangeCheck ()) {
						_GlitterShown.floatValue = nval;
					}
					
					GUI.color = col;
				}
				if (_Glitter.floatValue == 1 && (_GlitterShown.floatValue == 1 || _GlitterShown.hasMixedValue)) {
					
					EditorGUI.BeginChangeCheck ();
					m_MaterialEditor.TexturePropertySingleLine (_Snow.floatValue == 0 ? Styles._SparkleMapGlitterText : Styles._SparkleMapGlitterSharedText, _SparkleMapGlitter, null);
					if (EditorGUI.EndChangeCheck ()) {
						// we need this texture in glitter - always
						_SparkleMap.textureValue = _SparkleMapGlitter.textureValue != null ? _SparkleMapGlitter.textureValue : _SparkleMap.textureValue;
						// dont allow null (take back _SparkleMap used in shader)
						_SparkleMapGlitter.textureValue = _SparkleMap.textureValue;
						// copy to snow - we've got it shared
						_SparkleMapSnow.textureValue = _SparkleMapGlitter.textureValue;
					}
					ShaderPropertyTooltip (_GlitterTiling, Styles._GlitterTilingText, 2);
					
					ShaderPropertyTooltip (_GlitterColor, Styles._GlitterColorText, 0);
					if (_TwoLayers_Used) {
						ShaderPropertyTooltip (_GlitterColor2, new GUIContent (Styles._GlitterColorText.text + " 2", Styles._GlitterColorText.tooltip), 0);
					}
					ShaderPropertyTooltip (_GlitterDensity, Styles._GlitterDensityText, 0);
					ShaderPropertyTooltip (_GlitterColorization, Styles._GlitterColorizationText, 1);
					//EditorGUILayout.Space();
					
					//ShaderPropertyTooltip(_GlitterAnimationFrequency, Styles._GlitterAnimationFrequencyText, 1);
					ShaderPropertyTooltip (_GlitterFilter, Styles._GlitterFilterText, 1);
					ShaderPropertyTooltip (_GlitterMask, Styles._GlitterMaskText, 1);
				}
				EditorGUILayout.EndVertical ();
				// (glitter)
				
				//EditorGUILayout.Space();
				
				// snow
				GUI.backgroundColor = new Color (0.95f, 1.0f, 1.0f, 1.0f);
				EditorGUILayout.BeginVertical ("Button");
				GUI.backgroundColor = bCol;
				
				{
					EditorGUI.showMixedValue = _Snow.hasMixedValue;
					float nval;
					EditorGUI.BeginChangeCheck ();
					if (_Snow.floatValue == 1) {
						nval = EditorGUILayout.ToggleLeft (Styles._SnowText, _Snow.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width (EditorGUIUtility.currentViewWidth - 60)) ? 1 : 0;
					} else {
						nval = EditorGUILayout.ToggleLeft (Styles._SnowText, _Snow.floatValue == 1, EditorStyles.boldLabel) ? 1 : 0;
					}
					if (EditorGUI.EndChangeCheck ()) {
                        _Snow.floatValue = nval;
                    }
                    EditorGUI.showMixedValue = false;
				}
				
				// show/hide block
				if (_Snow.floatValue == 1) {
					Color col = GUI.color;
					GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
					Rect rect = GUILayoutUtility.GetLastRect ();
					rect.x += EditorGUIUtility.currentViewWidth - 47;
					//rect.height-=EditorGUIUtility.singleLineHeight;
					
					EditorGUI.BeginChangeCheck ();
					float nval = EditorGUI.Foldout (rect, _SnowShown.floatValue == 1, "") ? 1 : 0;
					if (EditorGUI.EndChangeCheck ()) {
						_SnowShown.floatValue = nval;
					}
					
					GUI.color = col;
				}
				if (_Snow.floatValue == 1 && (_SnowShown.floatValue == 1 || _SnowShown.hasMixedValue)) {
					
					EditorGUILayout.BeginHorizontal ();
					ShaderPropertyTooltip (_SnowColorAndCoverage, Styles._SnowColorAndCoverageText, 0);
					
					EditorGUI.showMixedValue = _SnowColorAndCoverage.hasMixedValue;
					EditorGUI.BeginChangeCheck ();
					float slevel = _SnowColorAndCoverage.colorValue.a;
					float nslevel = EditorGUILayout.Slider (slevel, 0, 1);
					if (EditorGUI.EndChangeCheck ()) {
						_SnowColorAndCoverage.colorValue = new Color (_SnowColorAndCoverage.colorValue.r, _SnowColorAndCoverage.colorValue.g, _SnowColorAndCoverage.colorValue.b, nslevel);
					}
					EditorGUI.showMixedValue = false;
					
					EditorGUILayout.EndHorizontal ();
					ShaderPropertyTooltip (_Frost, Styles._FrostText, 0);
					
					ShaderPropertyTooltip (_SnowDiffuseScatteringColor, Styles._SnowDiffuseScatteringColorText, 1);
					EditorGUI.BeginDisabledGroup (_SnowDiffuseScatteringColor.colorValue.r == 0 && _SnowDiffuseScatteringColor.colorValue.g == 0 && _SnowDiffuseScatteringColor.colorValue.b == 0);
					ShaderPropertyTooltip (_SnowDiffuseScatteringExponent, Styles._SnowDiffuseScatteringExponentText, 2);
					ShaderPropertyTooltip (_SnowDiffuseScatteringOffset, Styles._SnowDiffuseScatteringOffsetText, 2);
					EditorGUI.EndDisabledGroup ();
					EditorGUI.BeginDisabledGroup (_Translucency.floatValue != 1);
					ShaderPropertyTooltip (_SnowTranslucencyColor, Styles._SnowTranslucencyColorText, 1);
					EditorGUI.EndDisabledGroup ();
					
					EditorGUI.BeginDisabledGroup(_Glitter.floatValue != 1 || _SnowGlitterColorFromGlobal.floatValue==1);
					ShaderPropertyTooltip (_SnowGlitterColor, Styles._SnowGlitterColorText, 1);
					EditorGUI.EndDisabledGroup();
					
					EditorGUI.BeginDisabledGroup(_SnowDissolveFromGlobal.floatValue==1);
					ShaderPropertyTooltip (_SnowDissolve, Styles._SnowDissolveText, 0);
					EditorGUI.EndDisabledGroup();
					ShaderPropertyTooltip (_SnowSlopeDamp, Styles._SnowSlopeDampText, 0);
					ShaderPropertyTooltip (_SnowHeightThreshold, Styles._SnowHeightThresholdText, 1);
					ShaderPropertyTooltip (_SnowHeightThresholdTransition, Styles._SnowHeightThresholdTransitionText, 1);
					ShaderPropertyTooltip (_SnowDeepSmoothen, Styles._SnowDeepSmoothenText, 0);
					
					EditorGUILayout.Space ();

                    EditorGUI.BeginChangeCheck ();
					m_MaterialEditor.TexturePropertySingleLine (_RippleMapWet.textureValue == null || _Wetness.floatValue==0 || _WetRipples.floatValue == 0 ? Styles._RippleMapSnowText : Styles._RippleMapSnowSharedText, _RippleMapSnow, _SnowBumpMacro);
					if (EditorGUI.EndChangeCheck ()) {
						_RippleMap.textureValue = _RippleMapSnow.textureValue != null ? _RippleMapSnow.textureValue : _RippleMapWet.textureValue;
						_RippleMapWet.textureValue = _RippleMapSnow.textureValue != null && _RippleMapWet.textureValue != null ? _RippleMapSnow.textureValue : _RippleMapWet.textureValue;
						_RippleMapSnow.textureValue = _RippleMapSnow.textureValue != null ? _RippleMapSnow.textureValue : _RippleMap.textureValue;
					}
					ShaderPropertyTooltip (_SnowMacroTiling, Styles._SnowMacroTilingText, 2);
					
					EditorGUI.BeginChangeCheck ();
					m_MaterialEditor.TexturePropertySingleLine (_Glitter.floatValue == 1 ? Styles._SparkleMapSnowWithGlitterText : Styles._SparkleMapSnowText, _SparkleMapSnow, _SnowBumpMicroFromGlobal.floatValue==1 ? null : _SnowBumpMicro);
					if (EditorGUI.EndChangeCheck ()) {
						// we need this texture in snow - always
						_SparkleMap.textureValue = _SparkleMapSnow.textureValue != null ? _SparkleMapSnow.textureValue : _SparkleMap.textureValue;
						// dont allow null (take back _SparkleMap used in shader)
						_SparkleMapSnow.textureValue = _SparkleMap.textureValue;
						// copy to glitter map - we've got it shared
						_SparkleMapGlitter.textureValue = _SparkleMapSnow.textureValue;
					}
					
					ShaderPropertyTooltip (_SnowMicroTiling, Styles._SnowMicroTilingText, 2);
                    if (_SnowBumpMicro2Used!= null)
                    {
                        ShaderPropertyTooltip(_SnowBumpMicro2Used, Styles._SnowBumpMicro2UsedText, 2);
                        if (_SnowBumpMicro2Used.floatValue>0)
                        {
                            ShaderPropertyTooltip(_SnowMicroTiling2, Styles._SnowMicroTiling2Text, 3);
                            ShaderPropertyTooltip(_SnowBumpMicro2, Styles._SnowBumpMicro2Text, 3);
                        }
                    }

                    EditorGUILayout.Space ();
					
					ShaderPropertyTooltip (_SnowWorldMapping, Styles._SnowWorldMappingText, 1);
					EditorGUI.BeginDisabledGroup(_SnowSpecGlossFromGlobal.floatValue==1);
					ShaderPropertyTooltip (_SnowSpecGloss, Styles._SnowSpecGlossText, 1);
					EditorGUI.EndDisabledGroup();
					ShaderPropertyTooltip (_SnowEmissionTransparency, Styles._SnowEmissionTransparencyText, 1);
					ShaderPropertyTooltip (_SnowDissolveMaskOcclusion, Styles._SnowDissolveMaskOcclusionText, 1);
					
					EditorGUILayout.Space ();
					
					// global toggles
					EditorGUILayout.BeginVertical("Box");
					
					GlobalControlBox();
					
					EditorGUILayout.BeginHorizontal ();
					// level
					{
						EditorGUI.showMixedValue = _SnowLevelFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_SnowLevelFromGlobal.floatValue==1 ? true : false), Styles._SnowLevelFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_SnowLevelFromGlobal.floatValue = nval;
							if (nval==0) {
								_WetnessMergeWithSnowPerMaterial.floatValue=0;
							}
						}
						EditorGUI.showMixedValue = false;
					}
					// frost
					{
						EditorGUI.showMixedValue = _FrostFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_FrostFromGlobal.floatValue==1 ? true : false), Styles._FrostFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_FrostFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					// glitter
					{
						EditorGUI.showMixedValue = _SnowGlitterColorFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_SnowGlitterColorFromGlobal.floatValue==1 ? true : false), Styles._SnowGlitterColorFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_SnowGlitterColorFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					// dissolve
					{
						EditorGUI.showMixedValue = _SnowDissolveFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_SnowDissolveFromGlobal.floatValue==1 ? true : false), Styles._SnowDissolveFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_SnowDissolveFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					// micro bump
					{
						EditorGUI.showMixedValue = _SnowBumpMicroFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_SnowBumpMicroFromGlobal.floatValue==1 ? true : false), Styles._SnowBumpMicroFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_SnowBumpMicroFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					// spec/gloss
					{
						EditorGUI.showMixedValue = _SnowSpecGlossFromGlobal.hasMixedValue;
						EditorGUI.BeginChangeCheck ();
						float nval = GUILayout.Toggle( (_SnowSpecGlossFromGlobal.floatValue==1 ? true : false), Styles._SnowSpecGlossFromGlobalText, EditorStyles.miniButton )  ? 1:0;
						if (EditorGUI.EndChangeCheck ()) {
							_SnowSpecGlossFromGlobal.floatValue = nval;
						}
						EditorGUI.showMixedValue = false;
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.EndVertical ();
				}
				EditorGUILayout.EndVertical ();
				
				//
				// RTP geom blend
				//
				if (_GeomBlend_Used || _GeomBlend_hasMixedValue) {
					EditorGUILayout.Space();
					
					// Secondary properties
					GUI.backgroundColor = new Color (1.0f, 1.0f, 1.0f, 0.9f);
					EditorGUILayout.BeginVertical ("Button");
					GUI.backgroundColor = bCol;
					
					GUILayout.Label (Styles.geomBlendSectionText, EditorStyles.boldLabel);
					// show/hide block
					if (true) {//_GeomBlendShown.floatValue==1) {
						Color col = GUI.color;
						GUI.color = new Color (1.0f, 1.0f, 0f, 1f);
						Rect rect = GUILayoutUtility.GetLastRect ();
						rect.x += EditorGUIUtility.currentViewWidth - 47;
						//rect.height-=EditorGUIUtility.singleLineHeight;
						
						EditorGUI.BeginChangeCheck ();
						float nval = EditorGUI.Foldout (rect, _GeomBlendShown.floatValue == 1, "") ? 1 : 0;
						if (EditorGUI.EndChangeCheck ()) {
							_GeomBlendShown.floatValue = nval;
						}
						
						GUI.color = col;
					}
					if (_GeomBlendShown.floatValue == 1 || _GeomBlendShown.hasMixedValue) {
						m_MaterialEditor.TexturePropertySingleLine (Styles._TERRAIN_HeightMapText, _TERRAIN_HeightMap);
						m_MaterialEditor.TexturePropertySingleLine (Styles._TERRAIN_ControlText, _TERRAIN_Control);
						ShaderPropertyTooltip (_TERRAIN_PosSize, Styles._TERRAIN_PosSizeText, 1);
						ShaderPropertyTooltip (_TERRAIN_Tiling, Styles._TERRAIN_TilingText, 1);
					}
					EditorGUILayout.EndVertical ();
				}				
				
			}
			if (EditorGUI.EndChangeCheck())
			{
				foreach (var obj in blendMode.targets)
					MaterialChanged((Material)obj, m_WorkflowMode);
			}

            EditorGUILayout.Space();

            GUILayout.Label(Styles.advancedText, EditorStyles.boldLabel);
            m_MaterialEditor.RenderQueueField();
            m_MaterialEditor.EnableInstancingField();
        }
		
		void GetStaticProp(Material material, string tag, ref bool _switch, ref bool _hasMixedValue, ref bool _hasAllOff) {
			_switch = material.GetTag(tag, false)=="On";
			_hasMixedValue=false;
			if (m_MaterialEditor!=null && m_MaterialEditor.targets!=null) {
				foreach (Material mat in m_MaterialEditor.targets) {
					if (material.GetTag (tag, false) != mat.GetTag (tag, false)) {
						_hasMixedValue = true;
						break;
					}
				}
			}
			_hasAllOff=true;
			if (m_MaterialEditor!=null && m_MaterialEditor.targets!=null) {
				foreach (Material mat in m_MaterialEditor.targets) {
					if (mat.GetTag (tag, false) == "On") {
						_hasAllOff = false;
						break;
					}
				} 
			}
		}
		
		internal void DetermineWorkflow(MaterialProperty[] props)
		{
			if (FindProperty("_SpecGlossMap", props, false) != null && FindProperty("_SpecColor", props, false) != null)
				m_WorkflowMode = WorkflowMode.Specular;
			else if (FindProperty("_MetallicGlossMap", props, false) != null && FindProperty("_Metallic", props, false) != null)
				m_WorkflowMode = WorkflowMode.Metallic;
			else
				m_WorkflowMode = WorkflowMode.Dielectric;
		}
		
		public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
		{
			alt_shader_inited = false;
			
			base.AssignNewShaderToMaterial(material, oldShader, newShader);
			
			BlendMode blendMode;
			
			bool _Refraction_Used=false, _Refraction_hasMixedValue=false, _Refraction_hasAllOff=false;
			GetStaticProp(material, "REFR", ref _Refraction_Used, ref _Refraction_hasMixedValue, ref _Refraction_hasAllOff);
			if (_Refraction_Used) {
				// force transparency in refraction mode
				blendMode = BlendMode.Transparent;
				material.SetFloat("_Mode", (float)blendMode);
				DetermineWorkflow( MaterialEditor.GetMaterialProperties(new Material[] { material }) );
				MaterialChanged(material, m_WorkflowMode);
			}
			
			bool _GeomBlend_Used=false, _GeomBlend_hasMixedValue=false, _GeomBlend_hasAllOff=false;
			GetStaticProp(material, "GEOM_BLEND", ref _GeomBlend_Used, ref _GeomBlend_hasMixedValue, ref _GeomBlend_hasAllOff);
			if (_GeomBlend_Used) {
				// force fade transparency in refraction mode
				blendMode = BlendMode.Fade;
				material.SetFloat("_Mode", (float)blendMode);
				DetermineWorkflow( MaterialEditor.GetMaterialProperties(new Material[] { material }) );
				MaterialChanged(material, m_WorkflowMode);
			}
			
			bool _Triplanar_Used=false, _Triplanar_hasMixedValue=false, _Triplanar_hasAllOff=false;
			GetStaticProp(material, "TRIPLANAR", ref _Triplanar_Used, ref _Triplanar_hasMixedValue, ref _Triplanar_hasAllOff);
			if (_Triplanar_Used && material.HasProperty("_POM")) {
				// turn off POM in triplanar
				material.SetFloat("_POM",0);
				DetermineWorkflow( MaterialEditor.GetMaterialProperties(new Material[] { material }) );
				MaterialChanged(material, m_WorkflowMode);
			}
			if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
				return;
			
			blendMode = BlendMode.Opaque;
			if (oldShader.name.Contains("/Transparent/Cutout/"))
			{
				blendMode = BlendMode.Cutout;
			}
			else if (oldShader.name.Contains("/Transparent/"))
			{
				// NOTE: legacy shaders did not provide physically based transparency
				// therefore Fade mode
				blendMode = BlendMode.Fade;
			}
			
			material.SetFloat("_Mode", (float)blendMode);
			
			DetermineWorkflow( MaterialEditor.GetMaterialProperties(new Material[] { material }) );
			MaterialChanged(material, m_WorkflowMode);
		}
		
		void BlendModePopup()
		{
			EditorGUI.showMixedValue = blendMode.hasMixedValue;
			var mode = (BlendMode)blendMode.floatValue;
			
			EditorGUI.BeginChangeCheck();
			mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
			if (EditorGUI.EndChangeCheck())
			{
				m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
				blendMode.floatValue = (float)mode;
			}
			
			EditorGUI.showMixedValue = false;
		}
		
		void ShadowCullingPopup() {
			EditorGUI.showMixedValue = _ShadowCull.hasMixedValue;
			var mode = (CullMode)_ShadowCull.floatValue;
			
			EditorGUI.BeginChangeCheck();
			mode = (CullMode)EditorGUILayout.Popup(Styles.cullingMode, (int)mode, Styles.cullingNames);
			if (EditorGUI.EndChangeCheck())
			{
				m_MaterialEditor.RegisterPropertyChangeUndo("Shadow Culling Mode");
				_ShadowCull.floatValue = (float)mode;
			}
			
			EditorGUI.showMixedValue = false;
			
		}
		
		void DoAlbedoArea(Material material, bool _AlbedoOcclusion_Used, bool _TwoLayers_Used, bool _AlbedoSmoothnessAvailable)
		{
            m_MaterialEditor.TexturePropertySingleLine(((_AlbedoSmoothnessAvailable && Smoothness_from_albedo_alpha.floatValue != 0) ? Styles.albedoSmthText : (_AlbedoOcclusion_Used ? Styles.albedoOccText : Styles.albedoText)), albedoMap, albedoColor);
            if (!_TwoLayers_Used && _AlbedoSmoothnessAvailable)
            {
                // (show it below albedo for single layer mode
                ShaderPropertyTooltip(Smoothness_from_albedo_alpha, Styles.Smoothness_from_albedo_alphaText, 2);
            }
            if (albedoMap.textureValue != null && _AlbedoOcclusion_Used) {
                if (!_AlbedoSmoothnessAvailable || (_AlbedoSmoothnessAvailable && Smoothness_from_albedo_alpha.floatValue == 0))
                {
                    ShaderPropertyTooltip(occlusionStrength, Styles.occlusionStrengthText, 2);
                }
            }
			if (_TwoLayers_Used) {
				m_MaterialEditor.TexturePropertySingleLine ((_AlbedoSmoothnessAvailable && Smoothness_from_albedo_alpha.floatValue != 0) ? new GUIContent(Styles.albedoSmthText.text + " 2", Styles.albedoSmthText.tooltip): (_AlbedoOcclusion_Used ? new GUIContent(Styles.albedoOccText.text+" 2", Styles.albedoOccText.tooltip) : new GUIContent(Styles.albedoText.text+" 2", Styles.albedoText.tooltip)), albedoMap2, albedoColor2);
                if (_AlbedoSmoothnessAvailable)
                {
                    // (show it below 2nd albedo for 2layers mode
                    ShaderPropertyTooltip(Smoothness_from_albedo_alpha, Styles.Smoothness_from_albedo_alphaText, 2);
                }
                if (albedoMap2.textureValue != null && _AlbedoOcclusion_Used) {
                    if (!_AlbedoSmoothnessAvailable || (_AlbedoSmoothnessAvailable && Smoothness_from_albedo_alpha.floatValue == 0))
                    {
                        ShaderPropertyTooltip(occlusionStrength2, Styles.occlusionStrengthText, 2);
                    }
				}
			}

            if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
			{
				ShaderPropertyTooltip(alphaCutoff, Styles.alphaCutoffText, MaterialEditor.kMiniTextureFieldLabelIndentLevel);
				ShaderPropertyTooltip(_CutoffEdgeGlow, Styles._CutoffEdgeGlowText, MaterialEditor.kMiniTextureFieldLabelIndentLevel);
			}
			
			EditorGUI.BeginChangeCheck();
			ShaderPropertyTooltip(_DiffuseScatteringColor, Styles._DiffuseScatteringColorText,2);
			if (_TwoLayers_Used) {
				ShaderPropertyTooltip(_DiffuseScatteringColor2, new GUIContent(Styles._DiffuseScatteringColorText.text+" 2", Styles._DiffuseScatteringColorText.tooltip),2);
				EditorGUI.BeginDisabledGroup(_DiffuseScatteringColor.colorValue.r==0 && _DiffuseScatteringColor.colorValue.g==0 && _DiffuseScatteringColor.colorValue.b==0 && _DiffuseScatteringColor2.colorValue.r==0 && _DiffuseScatteringColor2.colorValue.g==0 && _DiffuseScatteringColor2.colorValue.b==0);
			} else {
				EditorGUI.BeginDisabledGroup(_DiffuseScatteringColor.colorValue.r==0 && _DiffuseScatteringColor.colorValue.g==0 && _DiffuseScatteringColor.colorValue.b==0);
			}
			if (EditorGUI.EndChangeCheck())	{
				if (_TwoLayers_Used) {
					_DiffuseScatter.floatValue = ((_DiffuseScatteringColor.colorValue.grayscale==0) && (_DiffuseScatteringColor2.colorValue.grayscale==0)) ? 0 : 1;
				} else {
					_DiffuseScatter.floatValue = (_DiffuseScatteringColor.colorValue.grayscale==0) ? 0 : 1;
				}
			}
			ShaderPropertyTooltip(_DiffuseScatteringExponent, Styles._DiffuseScatteringExponentText, 3);
			ShaderPropertyTooltip(_DiffuseScatteringOffset, Styles._DiffuseScatteringOffsetText, 3);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Space ();
		}
		
		void DoEmissionArea(Material material, bool _Animated_Emission)
		{
			float brightness = emissionColorForRendering.colorValue.maxColorComponent;
			bool showHelpBox = !HasValidEmissiveKeyword(material);
			bool showEmissionColorAndGIControls = brightness > 0.0f;
			
			bool hadEmissionTexture = emissionMap.textureValue != null;
			
			Color bCol = GUI.backgroundColor;
			if (showEmissionColorAndGIControls) {
				GUI.backgroundColor = new Color (1f, 1f, 1.0f, 1f);
				EditorGUILayout.BeginVertical("Box");
				GUI.backgroundColor = bCol;
			}
			
			// Texture and HDR color controls
			m_MaterialEditor.TexturePropertyWithHDRColor(_Animated_Emission ? Styles.emissionAnimatedText : Styles.emissionText, emissionMap, emissionColorForRendering,
#if !UNITY_2018_1_OR_NEWER
                m_ColorPickerHDRConfig,
#endif
                false);

            // If texture was assigned and color was black set color to white
            if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
				emissionColorForRendering.colorValue = Color.white;
			
			// Dynamic Lightmapping mode
			if (showEmissionColorAndGIControls)
			{
				bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled(emissionColorForRendering.colorValue);
				EditorGUI.BeginDisabledGroup(!shouldEmissionBeEnabled);
				
				m_MaterialEditor.LightmapEmissionProperty (MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
				
				EditorGUI.EndDisabledGroup();
				
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(17);
				
				{
					EditorGUI.showMixedValue = _PanEmissionMask.hasMixedValue;
					EditorGUI.BeginChangeCheck();
					float nval=EditorGUILayout.ToggleLeft(Styles._PanEmissionMaskText, _PanEmissionMask.floatValue==1, EditorStyles.boldLabel) ? 1:0;
					if (EditorGUI.EndChangeCheck())	{
						_PanEmissionMask.floatValue=nval;
					}
					EditorGUI.showMixedValue = false;
				}
				
				EditorGUILayout.EndHorizontal();
				//ShaderPropertyTooltip(_PanEmissionMask, Styles._PanEmissionMaskText, 1);
				if (_PanEmissionMask.floatValue!=0) {
					ShaderPropertyTooltip(_PanUSpeed, Styles._PanUSpeedText, 2);
					ShaderPropertyTooltip(_PanVSpeed, Styles._PanVSpeedText, 2);
				}
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(17);
				
				{
					EditorGUI.showMixedValue = _PulsateEmission.hasMixedValue;
					EditorGUI.BeginChangeCheck();
					float nval=EditorGUILayout.ToggleLeft(Styles._PulsateEmissionText, _PulsateEmission.floatValue==1, EditorStyles.boldLabel) ? 1:0;
					if (EditorGUI.EndChangeCheck())	{
						_PulsateEmission.floatValue=nval;
					}
					EditorGUI.showMixedValue = false;
				}
				
				EditorGUILayout.EndHorizontal();
				//ShaderPropertyTooltip(_PulsateEmission, Styles._PulsateEmissionText, 1);
				if (_PulsateEmission.floatValue!=0) { 
					ShaderPropertyTooltip(_EmissionPulsateSpeed, Styles._EmissionPulsateSpeedText, 2);
					ShaderPropertyTooltip(_MinPulseBrightness, Styles._MinPulseBrightnessText, 2);
				}
				EditorGUILayout.EndVertical();
				
			}
			
			if (showHelpBox)
			{
				EditorGUILayout.HelpBox(Styles.emissiveWarning.text, MessageType.Warning);
			}
			
		}
		
		void DoSpecularMetallicArea(bool _TwoLayers_Used)
		{
			if (m_WorkflowMode == WorkflowMode.Specular) {
				if ((Smoothness_from_albedo_alpha==null || Smoothness_from_albedo_alpha.floatValue==0) && specularMap.textureValue == null && (specularMap2.textureValue == null || !_TwoLayers_Used) && _SpecularRGBGlossADetail.textureValue == null) { // (gloss ranged also for detail spec map)
					EditorGUI.BeginChangeCheck();
					float prevSmoothAlpha=specularColor.colorValue.a;
					m_MaterialEditor.TexturePropertyTwoLines (Styles.specularMapText, specularMap, specularColor, Styles.smoothnessText, smoothness);
					if (EditorGUI.EndChangeCheck())	{
						if (prevSmoothAlpha!=specularColor.colorValue.a) {
							smoothness.floatValue=specularColor.colorValue.a;
						} else {
							specularColor.colorValue=new Color(specularColor.colorValue.r, specularColor.colorValue.g, specularColor.colorValue.b, smoothness.floatValue);
						}
					}
					if (_TwoLayers_Used) {
						EditorGUI.BeginChangeCheck();
						float prevSmoothAlpha2=specularColor2.colorValue.a;
						m_MaterialEditor.TexturePropertyTwoLines (new GUIContent(Styles.specularMapText.text+" 2", Styles.specularMapText.tooltip), specularMap2, specularColor2, Styles.smoothnessText, smoothness2);
						if (EditorGUI.EndChangeCheck())	{
							if (prevSmoothAlpha2!=specularColor2.colorValue.a) {
								smoothness2.floatValue=specularColor2.colorValue.a;
							} else {
								specularColor2.colorValue=new Color(specularColor2.colorValue.r, specularColor2.colorValue.g, specularColor2.colorValue.b, smoothness2.floatValue);
							}
						}
					}
					_GlossMin.floatValue = 0; // explicit gloss (neither from main nor detail texture)
					_GlossMax.floatValue = 1;
				} else {
					// UBER: GlossMin, GlossMax added 
					m_MaterialEditor.TexturePropertySingleLine (Styles.specularMapText, specularMap, specularColor);
					if (_TwoLayers_Used) {
						m_MaterialEditor.TexturePropertySingleLine (new GUIContent(Styles.specularMapText.text+" 2", Styles.specularMapText.tooltip), specularMap2, specularColor2);
					}
					ShaderPropertyTooltip (_GlossMin, Styles._GlossMinText, 2);
					ShaderPropertyTooltip (_GlossMax, Styles._GlossMaxText, 2);
				}
			} else if (m_WorkflowMode == WorkflowMode.Metallic) {
				// UBER - we're multiplying value from metal texture by metallic and smoothness
				m_MaterialEditor.TexturePropertyTwoLines (Styles.metallicMapText, metallicMap, metallic, Styles.smoothnessText, smoothness);
				if (_TwoLayers_Used) {
					// UBER - we're multiplying value from metal texture by metallic and smoothness
					m_MaterialEditor.TexturePropertyTwoLines (new GUIContent(Styles.metallicMapText.text+" 2", Styles.metallicMapText.tooltip), metallicMap2, metallic2, new GUIContent(Styles.smoothnessText.text+" 2", Styles.smoothnessText.tooltip), smoothness2);
				}
				if ((Smoothness_from_albedo_alpha == null || Smoothness_from_albedo_alpha.floatValue == 0) && metallicMap.textureValue == null && (metallicMap2.textureValue == null || !_TwoLayers_Used) && _MetallicGlossMapDetail.textureValue == null) {
					_GlossMin.floatValue=0; // explicit gloss (neither from main nor detail texture)
					_GlossMax.floatValue=1;
				} else {
					ShaderPropertyTooltip(_GlossMin, Styles._GlossMinText,2);
					ShaderPropertyTooltip(_GlossMax, Styles._GlossMaxText,2);
				}
			}
		}
		
		
		void DoSpecularMetallicDetailArea()
		{
			if (m_WorkflowMode == WorkflowMode.Specular)
			{
				if (_SpecularRGBGlossADetail.textureValue==null && detailAlbedoMap.textureValue!=null) {
					EditorGUILayout.HelpBox("When Specular map not specified, gloss is taken from Detail Albedo (A).", MessageType.Info);
				}
				m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, _SpecularRGBGlossADetail, _DetailSpecGloss);
			}
			else if (m_WorkflowMode == WorkflowMode.Metallic)
			{
				if (_MetallicGlossMapDetail.textureValue==null && detailAlbedoMap.textureValue!=null) {
					EditorGUILayout.HelpBox("When Metallic map not specified, gloss is taken from Detail Albedo (A).", MessageType.Info);
				}
				if (_MetallicGlossMapDetail.textureValue==null) {
					m_MaterialEditor.TexturePropertyTwoLines(Styles.metallicMapText, _MetallicGlossMapDetail, _DetailMetalness, Styles._DetailGlossText, _DetailGloss);
				} else {
					m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, _MetallicGlossMapDetail);
					_DetailMetalness.floatValue=1; // we don't "tint" detail metallic/gloss values taken from texture
					_DetailGloss.floatValue=1;
				}
			}
			ShaderPropertyTooltip(_DetailSpecLerp, Styles._DetailSpecLerpText, 3);
		}
		
		public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
		{
			switch (blendMode)
			{
			case BlendMode.Opaque:
				material.SetOverrideTag("RenderType", "");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = -1; 
				break;
			case BlendMode.Cutout:
				material.SetOverrideTag("RenderType", "TransparentCutout");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2451;
				break;
			case BlendMode.Fade:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
			case BlendMode.Transparent:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
			}
		}
		
		public static void SetupMaterialWithShadowCullMode(Material material, CullMode cullMode)
		{
			switch (cullMode)
			{
			case CullMode.Back:
				material.SetInt("_ShadowCull", (int)UnityEngine.Rendering.CullMode.Back);
				break;
			case CullMode.Front:
				material.SetInt("_ShadowCull", (int)UnityEngine.Rendering.CullMode.Front);
				break;
			case CullMode.Off:
				material.SetInt("_ShadowCull", (int)UnityEngine.Rendering.CullMode.Off);
				break;
			}
		}
		
		static bool ShouldEmissionBeEnabled(Color color)
		{
			return color.maxColorComponent > (0.1f / 255.0f);
		}
		
		static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
		{
			// Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
			// (MaterialProperty value might come from renderer material property block)
			
			bool _Silhouettes_Used = material.GetTag ("SILHOUETTE_CURVATURE", false) == "On";
			if (_Silhouettes_Used) {
				float _POMCurvatureType = material.GetFloat ("_POMCurvatureType");
				SetKeyword (material, "SILHOUETTE_CURVATURE_BASIC", _POMCurvatureType==0);
				SetKeyword (material, "SILHOUETTE_CURVATURE_MAPPED", _POMCurvatureType==1);
			}
			
			//			if (!_Silhouettes_Used) { // normalmap is turned on permanently for silhouettes
			//				SetKeyword (material, "_NORMALMAP", material.GetTexture ("_BumpMap") || material.GetTexture ("_DetailNormalMap"));
			//			}
			
			bool _TwoLayers_Used = material.GetTag("TWO_LAYERS", false)=="On";
			bool _Tessellation_Used = material.GetTag("TESS", false)=="On";
			
			bool _DistanceMap_Used = material.GetTag ("POM_DISTANCE_MAP", false) == "On";
			bool _ExtrusionMap_Used = material.GetTag ("POM_EXTRUSION_MAP", false) == "On";
			
			bool _Triplanar_Used = material.GetTag ("TRIPLANAR", false) == "On";

            //			if (workflowMode == WorkflowMode.Specular)
            //				SetKeyword (material, "_SPECGLOSSMAP", material.GetTexture ("_SpecGlossMap")!=null || (_TwoLayers_Used && material.GetTexture ("_SpecGlossMap2")!=null));
            //			else if (workflowMode == WorkflowMode.Metallic)
            //				SetKeyword (material, "_METALLICGLOSSMAP", material.GetTexture ("_MetallicGlossMap")!=null || (_TwoLayers_Used && material.GetTexture ("_MetallicGlossMap2")!=null));

            bool flag_POM_Shadows=false;
			bool flag_POM=false;
			bool flag_Parallax=false;
			bool flag_Parallax_2Maps=false;
			if (_TwoLayers_Used) {
				if (material.GetTexture ("_ParallaxMap") != null) {
					flag_Parallax = material.HasProperty("_Parallax") && material.GetFloat("_Parallax")>0;
				}
				if (material.GetTexture ("_ParallaxMap2") != null) {
					flag_Parallax = false;
					flag_Parallax_2Maps = true;
				}
			} else if (!_Tessellation_Used) {
				if (material.GetTexture ("_ParallaxMap") != null) {
					if (material.HasProperty ("_POM") && material.HasProperty ("_POMShadows")) {
						// shader applied
						if (material.GetFloat ("_POM") != 0 && !_TwoLayers_Used) {
							if (material.GetFloat ("_POMShadows") != 0) {
								flag_POM_Shadows = true;
							} else {
								flag_POM = true;
							}
						} else {
							flag_Parallax = material.HasProperty("_Parallax") && material.GetFloat("_Parallax")>0;
						}
					}
				}
			}
			//Debug.Log ("" + flag_POM_Shadows + " " + flag_POM + " " + flag_Parallax + " " + flag_Parallax_2Maps);
			bool ZWRITE=material.HasProperty("_DepthWrite") && (material.GetFloat("_DepthWrite")==1);
			if (!_DistanceMap_Used && !_ExtrusionMap_Used) {
				SetKeyword (material, "_PARALLAX_POM_SHADOWS", flag_POM_Shadows);
				SetKeyword (material, "_PARALLAX_POM", flag_POM && !ZWRITE);
				SetKeyword (material, "_PARALLAX_POM_ZWRITE", flag_POM && ZWRITE);
				SetKeyword (material, "_PARALLAXMAP", flag_Parallax);
				SetKeyword (material, "_PARALLAXMAP_2MAPS", flag_Parallax_2Maps);
			} else {
				flag_POM_Shadows = material.GetFloat ("_POMShadows") != 0;
				if (_DistanceMap_Used) {
					SetKeyword (material, "_POM_DISTANCE_MAP_SHADOWS", flag_POM_Shadows && !ZWRITE);
					SetKeyword (material, "_POM_DISTANCE_MAP", !flag_POM_Shadows && !ZWRITE);
					SetKeyword (material, "_POM_DISTANCE_MAP_ZWRITE", !flag_POM_Shadows && ZWRITE);
				} else if (_ExtrusionMap_Used) {
					SetKeyword (material, "_POM_EXTRUSION_MAP_SHADOWS", flag_POM_Shadows && !ZWRITE);
					SetKeyword (material, "_POM_EXTRUSION_MAP", !flag_POM_Shadows && !ZWRITE);
					SetKeyword (material, "_POM_EXTRUSION_MAP_ZWRITE", !flag_POM_Shadows && ZWRITE);
				}
			}
			
			bool _AlbedoOcclusion = (material.GetTexture("_OcclusionMap") == null || material.GetFloat("_UVSecOcclusion") == 1 || material.GetTag("FORCE_DIFFAO", false) == "On");
			SetKeyword (material, "_Occlusion_from_albedo_alpha", false ); // disable prev UBER1.1d usage
            material.SetFloat("_Occlusion_from_albedo_alpha", _AlbedoOcclusion ? 1:0); // for transparent modes we'll hardcode occlusion as 1 (in UBER_StandardInput.cginc/Occlusion() function)

            // disable _Smoothness_from_albedo_alpha feature where it's not applicable (any of spec/metallic maps used or we're in transparent mode)
            if (workflowMode == WorkflowMode.Specular)
            {
                if (((BlendMode)material.GetFloat("_Mode") != BlendMode.Opaque) || material.GetTexture("_SpecGlossMap") || (_TwoLayers_Used && material.GetTexture("_SpecGlossMap2")) || material.GetTexture("_SpecularRGBGlossADetail"))
                {
                    material.SetFloat("_Smoothness_from_albedo_alpha", 0);
                }
            }
            else if (workflowMode == WorkflowMode.Metallic)
            {
                if (((BlendMode)material.GetFloat("_Mode") != BlendMode.Opaque) || material.GetTexture("_MetallicGlossMap") || (_TwoLayers_Used && material.GetTexture("_MetallicGlossMap2")) || material.GetTexture("_MetallicGlossMapDetail"))
                {
                    material.SetFloat("_Smoothness_from_albedo_alpha", 0);
                }
            }

            // UBER
            if (workflowMode == WorkflowMode.Specular) {
				// simple detail (masked with mask texture w/o actual detail textures)
				SetKeyword (material, "_DETAIL_SIMPLE", material.GetTexture ("_DetailMask") && !(material.GetTexture ("_DetailAlbedoMap") || material.GetTexture ("_DetailNormalMap") || material.GetTexture ("_SpecularRGBGlossADetail")) ); // (detail glossmap)
				// advanced detail - textured
				SetKeyword (material, "_DETAIL_TEXTURED", (material.GetTexture ("_DetailAlbedoMap") || material.GetTexture ("_DetailNormalMap")) && !material.GetTexture ("_SpecularRGBGlossADetail"));
				SetKeyword (material, "_DETAIL_TEXTURED_WITH_SPEC_GLOSS", material.GetTexture ("_SpecularRGBGlossADetail"));
			} else if (workflowMode == WorkflowMode.Metallic) {
				// simple detail (masked with mask texture w/o actual detail textures)
				SetKeyword (material, "_DETAIL_SIMPLE", material.GetTexture ("_DetailMask") && !(material.GetTexture ("_DetailAlbedoMap") || material.GetTexture ("_DetailNormalMap") || material.GetTexture ("_MetallicGlossMapDetail")) );
				// advanced detail - textured
				SetKeyword (material, "_DETAIL_TEXTURED", (material.GetTexture ("_DetailAlbedoMap") || material.GetTexture ("_DetailNormalMap")) && !material.GetTexture ("_MetallicGlossMapDetail"));
				SetKeyword (material, "_DETAIL_TEXTURED_WITH_SPEC_GLOSS", material.GetTexture ("_MetallicGlossMapDetail"));
			}
			// wetness
			bool flag_Wetness = ( material.HasProperty ("_Wetness") && material.HasProperty ("_WetDroplets") && material.HasProperty ("_WetRipples") ) && material.GetFloat ("_Wetness") == 1;
			bool flag_Wetness_Simple = flag_Wetness && (material.GetFloat ("_WetDroplets") == 0) && (material.GetFloat ("_WetRipples") == 0);
			bool flag_Wetness_Droplets = flag_Wetness && (material.GetFloat ("_WetDroplets") == 1) && (material.GetFloat ("_WetRipples") == 0);
			bool flag_Wetness_Ripples = flag_Wetness && (material.GetFloat ("_WetDroplets") == 0) && (material.GetFloat ("_WetRipples") == 1);
			bool flag_Wetness_Full = flag_Wetness && (material.GetFloat ("_WetDroplets") == 1) && (material.GetFloat ("_WetRipples") == 1);
			SetKeyword (material, "_WETNESS_NONE", !flag_Wetness);
			SetKeyword (material, "_WETNESS_SIMPLE", flag_Wetness_Simple);
			SetKeyword (material, "_WETNESS_RIPPLES", flag_Wetness_Ripples);
			SetKeyword (material, "_WETNESS_DROPLETS", flag_Wetness_Droplets);
			SetKeyword (material, "_WETNESS_FULL", flag_Wetness_Full);
			//Debug.Log (flag_Wetness + " " + flag_Wetness_Simple + " " + flag_Wetness_Droplets + " " + flag_Wetness_Ripples + " " + flag_Wetness_Full);
			
			SetKeyword (material, "_SNOW", material.HasProperty ("_Snow") && material.GetFloat("_Snow")==1 );
			
			SetKeyword (material, "_TRANSLUCENCY", material.HasProperty ("_Translucency") && material.GetFloat("_Translucency")==1 );
			
			//			if (_TwoLayers_Used) {
			//				SetKeyword (material, "_DIFFUSE_SCATTER", material.HasProperty ("_DiffuseScatteringColor") && (material.GetColor("_DiffuseScatteringColor").r!=0 || material.GetColor("_DiffuseScatteringColor").g!=0 || material.GetColor("_DiffuseScatteringColor").b!=0) || (material.HasProperty ("_DiffuseScatteringColor2") && (material.GetColor("_DiffuseScatteringColor2").r!=0 || material.GetColor("_DiffuseScatteringColor2").g!=0 || material.GetColor("_DiffuseScatteringColor2").b!=0)));
			//			} else {
			//				SetKeyword (material, "_DIFFUSE_SCATTER", material.HasProperty ("_DiffuseScatteringColor") && (material.GetColor("_DiffuseScatteringColor").r!=0 || material.GetColor("_DiffuseScatteringColor").g!=0 || material.GetColor("_DiffuseScatteringColor").b!=0) );
			//			}
			
			SetKeyword (material, "_GLITTER", material.HasProperty ("_Glitter") && material.GetFloat("_Glitter")==1 );
			
			if (_Triplanar_Used) {
				SetKeyword (material, "_TRIPLANAR_WORLD_MAPPING", material.HasProperty ("_TriplanarWorldMapping") && material.GetFloat("_TriplanarWorldMapping")==1 );
			}
			
			bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled (material.GetColor("_EmissionColor"));
			shouldEmissionBeEnabled = shouldEmissionBeEnabled || flag_Wetness;
			bool flag_Emission_Textured = material.HasProperty("_EmissionMap") && (material.GetTexture ("_EmissionMap") != null);
			bool flag_Emission_Animated = flag_Emission_Textured && (material.HasProperty("_PulsateEmission") && material.HasProperty("_PanEmissionMask") && (material.GetFloat("_PulsateEmission") != 0) || (material.GetFloat("_PanEmissionMask") != 0));
			flag_Emission_Textured = flag_Emission_Textured && !flag_Emission_Animated;
			//Debug.Log ("" +shouldEmissionBeEnabled+ " " +flag_Emission_Textured+ " " +flag_Emission_Animated);
			//SetKeyword (material, "_EMISSION_SIMPLE", !shouldEmissionBeEnabled || (!flag_Emission_Textured && !flag_Emission_Animated));
			SetKeyword (material, "_EMISSION_TEXTURED", shouldEmissionBeEnabled && flag_Emission_Textured);
			SetKeyword (material, "_EMISSION_ANIMATED", shouldEmissionBeEnabled && flag_Emission_Animated);
			
			if (material.GetTag ("REFR", false) == "On") {
				SetKeyword (material, "_CHROMATIC_ABERRATION", material.GetFloat ("_RefractionChromaticAberration") > 0 );
			}
			
			if (material.GetTag ("TESS", false) == "On") {
				SetKeyword (material, "_TESSELLATION_DISPLACEMENT", material.GetFloat ("_TessDepth") > 0 );
			}
			
			// Setup lightmap emissive flags
			MaterialGlobalIlluminationFlags flags = material.globalIlluminationFlags;
			if ((flags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != 0)
			{
				flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
				if (!shouldEmissionBeEnabled)
					flags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
				
				material.globalIlluminationFlags = flags;
			}
		}
		
		bool HasValidEmissiveKeyword (Material material)
		{
			// Material animation might be out of sync with the material keyword.
			// So if the emission support is disabled on the material, but the property blocks have a value that requires it, then we need to show a warning.
			// (note: (Renderer MaterialPropertyBlock applies its values to emissionColorForRendering))
			//bool hasEmissionKeyword = material.IsKeywordEnabled ("_EMISSION_SIMPLE") || material.IsKeywordEnabled ("_EMISSION_TEXTURED") || material.IsKeywordEnabled ("_EMISSION_ANIMATED");
			bool hasEmissionKeyword = material.IsKeywordEnabled ("_EMISSION_TEXTURED") || material.IsKeywordEnabled ("_EMISSION_ANIMATED");
			if (!hasEmissionKeyword && ShouldEmissionBeEnabled (emissionColorForRendering.colorValue))
				return false;
			else
				return true;
		}
		
		static void MaterialChanged(Material material, WorkflowMode workflowMode)
		{
			// Handle Blending modes
			SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
			
			if (material.GetTag ("GEOM_BLEND", false) == "On") {
				material.renderQueue = 2012; // geometry+12
			}
			
			//if (material.GetTag ("REFR", false) == "On") {
			//	material.renderQueue = 3002; // transparent stuff drawn on the top of everything else
			//}
			
			// shadow culling mode
			SetupMaterialWithShadowCullMode(material, (CullMode)material.GetFloat("_ShadowCull"));
			
			// curvature
			if (material.GetFloat ("_POMPrecomputedFlag") > 0) {
				material.SetVector ("_CurvatureMultOffset", new Vector4 (material.GetFloat ("_CurvatureMultU"), material.GetFloat ("_CurvatureMultV"), 0, 0));
				//material.SetVector ("_Tan2ObjectMultOffset", new Vector4 (material.GetFloat ("_Tan2ObjMultU"), material.GetFloat ("_Tan2ObjMultV"), 0, 0));
				material.SetVector ("_Tan2ObjectMultOffset", new Vector4 (1, 1, 0, 0));
			} else {
				material.SetVector("_CurvatureMultOffset", new Vector4(0,0,material.GetFloat("_CurvatureCustomU"), material.GetFloat("_CurvatureCustomV")));
				material.SetVector ("_Tan2ObjectMultOffset", new Vector4 (0,0,material.GetFloat ("_Tan2ObjCustomU"), material.GetFloat ("_Tan2ObjCustomV")));
			}
			
			SetMaterialKeywords(material, workflowMode);
		}
		
		static void SetKeyword(Material m, string keyword, bool state)
		{
			if (state)
				m.EnableKeyword (keyword);
			else
				m.DisableKeyword (keyword);
		}
		
		void ShaderPropertyTooltip(MaterialProperty prop, GUIContent content, int indent) {
            if (prop == null) return;
            m_MaterialEditor.ShaderProperty(prop, content.text, indent);
			Rect rect=GUILayoutUtility.GetLastRect(); 
			rect.width = Mathf.Min(rect.width, EditorGUIUtility.labelWidth);
			EditorGUI.LabelField (rect, new GUIContent ("", content.tooltip));
			//Debug.Log ((prop.targets[0] as Material).IsKeywordEnabled("_WETNESS"));
		}
		
		void GUI_UV_Borders(MaterialProperty aProp) {
			Vector4 vec;
			
			UnityEngine.Object[] objs=aProp.targets;
			bool difX = false;
			bool difY = false;
			bool difZ = false;
			bool difW = false;
			Material mat=objs[0] as Material;
			Vector4 propValue = mat.GetVector(aProp.name);
			for (int i=1; i<objs.Length; i++) {
				Material compare_mat=objs[i] as Material;
				Vector4 compare_propValue = compare_mat.GetVector(aProp.name);
				if (propValue.x!=compare_propValue.x) {
					difX=true;
				}
				if (propValue.y!=compare_propValue.y) {
					difY=true;
				}
				if (propValue.z!=compare_propValue.z) {
					difZ=true;
				}
				if (propValue.w!=compare_propValue.w) {
					difW=true;
				}
			}
			
			float wdth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 32;
			Rect rect;
			
			EditorGUI.BeginChangeCheck();
			
			EditorGUI.showMixedValue = difX;
			vec.x=EditorGUILayout.FloatField(" ", aProp.vectorValue.x);
			rect=GUILayoutUtility.GetLastRect(); 
			GUI.Label(rect, "Umin", EditorStyles.miniLabel);
			
			if (EditorGUI.EndChangeCheck()) {
				m_MaterialEditor.RegisterPropertyChangeUndo("Material UV borders cut edit");
				for (int i=0; i<objs.Length; i++) {
					mat=objs[i] as Material;
					Vector4 _vec=mat.GetVector(aProp.name);
					_vec.x=vec.x;
					mat.SetVector(aProp.name, _vec);
				}
			}
			
			EditorGUI.BeginChangeCheck();
			
			EditorGUI.showMixedValue = difY;
			vec.y=EditorGUILayout.FloatField(" ", aProp.vectorValue.y);
			rect=GUILayoutUtility.GetLastRect(); 
			GUI.Label(rect, "Vmin", EditorStyles.miniLabel);
			
			if (EditorGUI.EndChangeCheck()) {
				m_MaterialEditor.RegisterPropertyChangeUndo("Material UV borders cut edit");
				Undo.RecordObjects(objs, "");
				for (int i=0; i<objs.Length; i++) {
					mat=objs[i] as Material;
					Vector4 _vec=mat.GetVector(aProp.name);
					_vec.y=vec.y;
					mat.SetVector(aProp.name, _vec);
				}
			}
			
			
			EditorGUI.BeginChangeCheck();
			
			EditorGUI.showMixedValue = difZ;
			vec.z=EditorGUILayout.FloatField(" ", aProp.vectorValue.z);
			rect=GUILayoutUtility.GetLastRect(); 
			GUI.Label(rect, "Umax", EditorStyles.miniLabel);
			
			if (EditorGUI.EndChangeCheck()) {
				m_MaterialEditor.RegisterPropertyChangeUndo("Material UV borders cut edit");
				for (int i=0; i<objs.Length; i++) {
					mat=objs[i] as Material;
					Vector4 _vec=mat.GetVector(aProp.name);
					_vec.z=vec.z;
					mat.SetVector(aProp.name, _vec);
				}
			}
			
			EditorGUI.BeginChangeCheck();
			
			EditorGUI.showMixedValue = difW;
			vec.w=EditorGUILayout.FloatField(" ", aProp.vectorValue.w);
			rect=GUILayoutUtility.GetLastRect(); 
			GUI.Label(rect, "Vmax", EditorStyles.miniLabel);
			
			if (EditorGUI.EndChangeCheck()) {
				m_MaterialEditor.RegisterPropertyChangeUndo("Material UV borders cut edit");
				for (int i=0; i<objs.Length; i++) {
					mat=objs[i] as Material;
					Vector4 _vec=mat.GetVector(aProp.name);
					_vec.w=vec.w;
					mat.SetVector(aProp.name, _vec);
				}
			}
			
			EditorGUIUtility.labelWidth = wdth;
			
			
			EditorGUI.showMixedValue = false;
			
		}
		
		void GlobalControlBox() {
			Color bCol = GUI.backgroundColor;
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Global control over", EditorStyles.boldLabel);
			GUI.backgroundColor = new Color (0, 0.3f, 0.1f, 0.3f);
			if (GUILayout.Button ("> Controller", EditorStyles.miniButton, GUILayout.Width (80))) {
				UBER_GlobalParams gScript = GameObject.FindObjectOfType<UBER_GlobalParams> ();
				if (gScript) {
					Selection.activeObject = gScript;
				} else {
					if (EditorUtility.DisplayDialog ("UBER Notification", "No controller found...\nMake one and attach it to the camera ?", "OK", "Cancel")) {
						Camera[] cams = GameObject.FindObjectsOfType<Camera> ();
						Camera cam = null;
						if (cams.Length > 0) {
							foreach (Camera acam in cams) {
								if (acam.tag=="MainCamera") {
									cam = acam;
									break;
								}
							}
							if (cam==null) {
								foreach (Camera acam in cams) {
									if (cam == null || acam.cullingMask != (1<<(LayerMask.NameToLayer ("UI"))) ) {
										cam = acam;
									}
								}
							}
							if (cam!=null) {
								gScript = cam.gameObject.AddComponent<UBER_GlobalParams> ();
							}
							Selection.activeObject = gScript;
						}
					}
				}
			}
			GUI.backgroundColor = bCol;
			EditorGUILayout.EndHorizontal ();
		}
		
		//
		// fix substance bug - no correct output to TexelSize
		//
		void FixSubstanceTexelSizeBug(MaterialProperty[] props) {
			Vector4 substanceTexelSize=Vector4.one;
			
			Vector4 heightMapTexelSize=Vector4.one;
			Vector4 bumpMapTexelSize=Vector4.one;
			if (bumpMap.textureValue != null) {
#if !UNITY_2018_1_OR_NEWER
				if (bumpMap.textureValue.texelSize.x == 1 && bumpMap.textureValue.texelSize.x == 1) {
					// bug in substances...
					Material material = m_MaterialEditor.target as Material;
					if (material is ProceduralMaterial) {
						ProceduralMaterial pmaterial = material as ProceduralMaterial;
						Vector4 size = pmaterial.GetProceduralVector ("$outputsize");
						substanceTexelSize.z = Mathf.Pow (2, size.x);
						substanceTexelSize.w = Mathf.Pow (2, size.y);
						substanceTexelSize.x = 1.0f / substanceTexelSize.z;
						substanceTexelSize.y = 1.0f / substanceTexelSize.w;
						bumpMapTexelSize = substanceTexelSize;
					}
				}
                else
#endif
                {
					bumpMapTexelSize.x = bumpMap.textureValue.texelSize.x;
					bumpMapTexelSize.y = bumpMap.textureValue.texelSize.y;
					bumpMapTexelSize.z = 1.0f / bumpMap.textureValue.texelSize.x;
					bumpMapTexelSize.w = 1.0f / bumpMap.textureValue.texelSize.y;
				}
			}
			
			if (heightMap.textureValue != null) {
				if (heightMap.textureValue.texelSize.x == 1 && heightMap.textureValue.texelSize.x == 1) {
					// bug in substances...
					heightMapTexelSize = substanceTexelSize;
				} else {
					heightMapTexelSize.x = heightMap.textureValue.texelSize.x;
					heightMapTexelSize.y = heightMap.textureValue.texelSize.y;
					heightMapTexelSize.z = 1.0f / heightMap.textureValue.texelSize.x;
					heightMapTexelSize.w = 1.0f / heightMap.textureValue.texelSize.y;
				} 
			}
			
			Vector4 bumpMap2TexelSize=Vector4.one;
			if (bumpMap2.textureValue != null) {
				if (bumpMap2.textureValue.texelSize.x == 1 && bumpMap2.textureValue.texelSize.x == 1) {
					// bug in substances...
					bumpMap2TexelSize = substanceTexelSize;
				} else {
					bumpMap2TexelSize.x = bumpMap2.textureValue.texelSize.x;
					bumpMap2TexelSize.y = bumpMap2.textureValue.texelSize.y;
					bumpMap2TexelSize.z = 1.0f / bumpMap2.textureValue.texelSize.x;
					bumpMap2TexelSize.w = 1.0f / bumpMap2.textureValue.texelSize.y;
				}
			}
			
			//Debug.Log ("E"+heightMapTexelSize*1000000);
			//Debug.Log ((1.0f/heightMap.textureValue.texelSize.x)+" "+(1.0f/heightMap.textureValue.texelSize.y));
			MaterialProperty _heightMapTexelSize = FindProperty("heightMapTexelSize", props, false);
			if (_heightMapTexelSize!=null) _heightMapTexelSize.vectorValue=heightMapTexelSize;
			MaterialProperty _bumpMapTexelSize = FindProperty ("bumpMapTexelSize", props, false);
			if (_bumpMapTexelSize!=null) _bumpMapTexelSize.vectorValue=bumpMapTexelSize;
			MaterialProperty _bumpMap2TexelSize = FindProperty ("bumpMap2TexelSize", props, false);
			if (_bumpMap2TexelSize!=null) _bumpMap2TexelSize.vectorValue=bumpMap2TexelSize;
		}
	}
}