using UnityEngine;
using System.Collections;

[AddComponentMenu("UBER/Global Params")]
[ExecuteInEditMode]
public class UBER_GlobalParams : MonoBehaviour {

	// some constants for dynamic weather
	public const float DEFROST_RATE = 0.3f; // rate of flow speed increase with positive temperature
	public const float RAIN_DAMP_ON_FREEZE_RATE = 0.2f; // rate of rain intentisy decreasy with negative temperature

	public const float FROZEN_FLOW_BUMP_STRENGTH = 0.1f; // target flow bumpmap strength multiplier for fully frozen water
	public const float FROST_RATE = 0.3f; // rate of flow anim speed decrease with negative temperature
	public const float FROST_RATE_BUMP = 0.5f; // rate of flow bumps reduction due to negative temperature
	public const float RAIN_TO_WATER_LEVEL_RATE = 2.0f; // rate of water level (Water Level slider in material) increase due to fall intensity
	public const float RAIN_TO_WET_AMOUNT_RATE = 3.0f; // rate of wet amount (Wetness Const slider in material) increase due to fall intensity

	public const float WATER_EVAPORATION_RATE = 0.001f; // rate of water level decrease due to wind and temperature (Water Level slider in material)
	public const float WET_EVAPORATION_RATE = 0.0003f; // rate of const wet decrease due to wind and temperature (Wetness Const slider in material)

	public const float SNOW_FREEZE_RATE = 0.05f; // rate of snow properties (gloss, micro bump, dissolve, glitter) going to default values
	public const float SNOW_INCREASE_RATE = 0.01f; // rate of building snow coverage
	public const float SNOW_MELT_RATE = 0.03f; // rate of snow properties (gloss, micro bump, dissolve, glitter) going to melting values
	public const float SNOW_MELT_RATE_BY_RAIN = 0.05f; // additional snow melt when it rains
	public const float SNOW_DECREASE_RATE = 0.01f; // rate of decreasing snow coverage (snow melt)

	[Header("Global Water & Rain")]

	[Tooltip("You can control global water level (multiplied by material value)")]
	[Range(0.0f, 1.0f)] public float WaterLevel=1;
	[Tooltip("You can control global wetness (multiplied by material value)")]
	[Range(0.0f, 1.0f)] public float WetnessAmount=1;

	[Tooltip("Time scale for flow animation")]
	public float flowTimeScale=1.0f;
	[Tooltip("Multiplier of water flow ripple normalmap")]
	[Range(0.0f, 1.0f)] public float FlowBumpStrength=1f;
    
	[Tooltip("You can control global rain intensity")]
	[Range(0.0f, 1.0f)] public float RainIntensity=1;


	[Header("Global Snow")]

	[Tooltip("You can control global snow")]
	[Range(0.0f, 1.0f)] public float SnowLevel=1;

	[Tooltip("You can control global frost")]
	[Range(0.0f, 1.0f)] public float Frost=1;

	[Tooltip("Global snow dissolve value")]
	[Range(0.0f, 4.0f)] public float SnowDissolve=2;
	[Tooltip("Global snow dissolve value")]
	[Range(0.001f, 0.2f)] public float SnowBumpMicro=0.08f;
	[Tooltip("Global snow spec (RGB) & Gloss (A)")]
	public Color SnowSpecGloss = new Color (0.1f, 0.1f, 0.1f, 0.15f);
	[Tooltip("Global snow glitter color/spec boost")]
	public Color SnowGlitterColor = new Color (0.8f, 0.8f, 0.8f, 0.2f);

	// remove [HideInInspector] to tweak params
	[Header("Global Snow - cover state")]
	[HideInInspector][Range(0.0f, 4.0f)] public float SnowDissolveCover=2;
	[Tooltip("Global snow dissolve value")]
	[HideInInspector][Range(0.001f, 0.2f)] public float SnowBumpMicroCover=0.08f;
	[Tooltip("Global snow spec (RGB) & Gloss (A)")]
	[HideInInspector]public Color SnowSpecGlossCover = new Color (0.1f, 0.1f, 0.1f, 0.15f);
	[Tooltip("Global snow glitter color/spec boost")]
	[HideInInspector]public Color SnowGlitterColorCover = new Color (0.8f, 0.8f, 0.8f, 0.2f);
    
	// remove [HideInInspector] to tweak params
	[Header("Global Snow - melt state")]
	[HideInInspector][Range(0.0f, 4.0f)] public float SnowDissolveMelt=0.3f;
	[Tooltip("Global snow dissolve value")]
	[HideInInspector][Range(0.001f, 0.2f)] public float SnowBumpMicroMelt=0.02f;
	[Tooltip("Global snow spec (RGB) & Gloss (A)")]
	[HideInInspector]public Color SnowSpecGlossMelt = new Color (0.15f, 0.15f, 0.15f, 0.6f);
	[Tooltip("Global snow glitter color/spec boost")]
	[HideInInspector]public Color SnowGlitterColorMelt = new Color (0.1f, 0.1f, 0.1f, 0.03f);
    
	[Header("Rainfall/snowfall controller")]
	public bool Simulate = false;
	[Range(0f, 1f)] public float fallIntensity=0;
	[Tooltip("Temperature (influences melt/freeze/evaporation speed) - 0 means water freeze")]
	[Range(-50f, 50f)] public float temperature=20f;
	[Tooltip("Wind (1 means 4x faster evaporation and freeze rate)")]
	[Range(0f, 1f)] public float wind=0f;
    [Tooltip("Speed of surface state change due to the weather dynamics")]
	[Range(0f, 1f)] public float weatherTimeScale=1f;

	[Tooltip("We won't melt ice nor decrease water level while snow level is >5%")]
	public bool FreezeWetWhenSnowPresent = true;

	[Tooltip("Increase global Water level when snow appears")]
	public bool AddWetWhenSnowPresent = true;

	[Space(10)]
	[Tooltip("Set to show and adjust below particle systems")]
	public bool UseParticleSystem=true;
	[Tooltip("GameObject with particle system attached controlling rain")]
	public GameObject rainGameObject;
	[Tooltip("GameObject with particle system attached controlling snow")]
    public GameObject snowGameObject;

    private Vector4 __Time;
	private float lTime;
	private bool paricleSystemActive=false;

	private ParticleSystem psRain;
	private ParticleSystem psSnow;

	#if UNITY_EDITOR
		void OnDrawGizmos() {
			if (!Application.isPlaying) {
				float delta=Time.realtimeSinceStartup-lTime;
				lTime=Time.realtimeSinceStartup;
				AdvanceTime (delta);
			}
		}

		void Update() {
			SetupIt();
			if (Application.isPlaying) {
				AdvanceTime (Time.deltaTime);
			}
		}
	#else
		void Update() {
			AdvanceTime(Time.deltaTime);
		}
	#endif

	void Start() {
		SetupIt();
	}

	public void SetupIt() {
		Shader.SetGlobalFloat("_UBER_GlobalDry", 1-WaterLevel);
		Shader.SetGlobalFloat("_UBER_GlobalDryConst", 1-WetnessAmount);

		Shader.SetGlobalFloat("_UBER_GlobalRainDamp", 1-RainIntensity);
		Shader.SetGlobalFloat ("_UBER_RippleStrength", FlowBumpStrength);
        
		Shader.SetGlobalFloat("_UBER_GlobalSnowDamp", 1-SnowLevel);
		Shader.SetGlobalFloat("_UBER_Frost", 1-Frost);
		Shader.SetGlobalFloat ("_UBER_GlobalSnowDissolve", SnowDissolve);
		Shader.SetGlobalFloat ("_UBER_GlobalSnowBumpMicro", SnowBumpMicro);
		Shader.SetGlobalColor ("_UBER_GlobalSnowSpecGloss", SnowSpecGloss);
		Shader.SetGlobalColor ("_UBER_GlobalSnowGlitterColor", SnowGlitterColor);
	}

	public void AdvanceTime(float amount) {
		SimulateDynamicWeather(amount*weatherTimeScale);

		// time advance in shader (flow)
		amount *= flowTimeScale;
		__Time.x += amount / 20.0f;
		__Time.y += amount;
		__Time.z += amount*2;
		__Time.w += amount*3;
		Shader.SetGlobalVector("UBER_Time", __Time);
	}

	public void SimulateDynamicWeather(float dt) {
		if (dt == 0 || !Simulate)
			return;

		float prev_RainIntensity = RainIntensity;
		float prev_temperature = temperature;
		float prev_flowTimeScale = flowTimeScale;
		float prev_FlowBumpStrength = FlowBumpStrength;
		float prev_WaterLevel = WaterLevel;
		float prev_WetnessAmount = WetnessAmount;

		float prev_SnowLevel = SnowLevel;
		float prev_SnowDissolve = SnowDissolve;
		float prev_SnowBumpMicro = SnowBumpMicro;
		Color prev_SnowSpecGloss = SnowSpecGloss;
		Color prev_SnowGlitterColor = SnowGlitterColor;

		float windF = (wind * 4f + 1f);

		float snowPresentFct=FreezeWetWhenSnowPresent ? Mathf.Clamp01((0.05f-SnowLevel)/0.05f) : 1;

		if (temperature > 0) {
			float temperatureFct=temperature+10f;
			RainIntensity = fallIntensity*snowPresentFct;
			flowTimeScale += (dt*temperatureFct*DEFROST_RATE)*snowPresentFct; // defrost
			if (flowTimeScale>1) flowTimeScale=1;
			FlowBumpStrength += (dt*temperatureFct*DEFROST_RATE)*snowPresentFct;
			if (FlowBumpStrength>1) FlowBumpStrength=1;

			WaterLevel+=RainIntensity*dt*RAIN_TO_WATER_LEVEL_RATE*snowPresentFct;
			if (WaterLevel>1) WaterLevel=1;
			WetnessAmount+=RainIntensity*dt*RAIN_TO_WET_AMOUNT_RATE*snowPresentFct;
			if (WetnessAmount>1) WetnessAmount=1;

			float meltFct = Mathf.Abs(dt*temperatureFct*SNOW_MELT_RATE + dt*RainIntensity*SNOW_MELT_RATE_BY_RAIN);
			SnowDissolve = TargetValue(SnowDissolve, SnowDissolveMelt, meltFct*2f);
			SnowBumpMicro = TargetValue(SnowBumpMicro, SnowBumpMicroMelt, meltFct*0.1f);
			SnowSpecGloss.r = TargetValue(SnowSpecGloss.r, SnowSpecGlossMelt.r, meltFct);
			SnowSpecGloss.g = TargetValue(SnowSpecGloss.g, SnowSpecGlossMelt.g, meltFct);
			SnowSpecGloss.b = TargetValue(SnowSpecGloss.b, SnowSpecGlossMelt.b, meltFct);
			SnowSpecGloss.a = TargetValue(SnowSpecGloss.a, SnowSpecGlossMelt.a, meltFct);
			SnowGlitterColor.r = TargetValue(SnowGlitterColor.r, SnowGlitterColorMelt.r, meltFct);
			SnowGlitterColor.g = TargetValue(SnowGlitterColor.g, SnowGlitterColorMelt.g, meltFct);
			SnowGlitterColor.b = TargetValue(SnowGlitterColor.b, SnowGlitterColorMelt.b, meltFct);
			SnowGlitterColor.a = TargetValue(SnowGlitterColor.a, SnowGlitterColorMelt.a, meltFct);

			Frost -= (dt*temperatureFct*DEFROST_RATE)*snowPresentFct; // defrost
			if (Frost<0) Frost=0;
			
			SnowLevel -= (dt*temperatureFct*SNOW_DECREASE_RATE);
			if (SnowLevel<0) SnowLevel=0; 

		} else {
			float temperatureFct=temperature-10f;
			RainIntensity += (dt*temperatureFct*RAIN_DAMP_ON_FREEZE_RATE);
			if (RainIntensity<0) RainIntensity=0; 

			flowTimeScale += (dt*temperatureFct*FROST_RATE*windF); // frost
			if (flowTimeScale<0) flowTimeScale=0;
			if (FlowBumpStrength>FROZEN_FLOW_BUMP_STRENGTH) {
				FlowBumpStrength += (dt*temperatureFct*FROST_RATE_BUMP*flowTimeScale);
				if (FlowBumpStrength<FROZEN_FLOW_BUMP_STRENGTH) {
					FlowBumpStrength=FROZEN_FLOW_BUMP_STRENGTH;
				}
			}

			float freezeFct = Mathf.Abs(dt*temperatureFct*SNOW_FREEZE_RATE)*fallIntensity;
			SnowDissolve = TargetValue(SnowDissolve, SnowDissolveCover, freezeFct*2f);
			SnowBumpMicro = TargetValue(SnowBumpMicro, SnowBumpMicroCover, freezeFct*0.1f);
			SnowSpecGloss.r = TargetValue(SnowSpecGloss.r, SnowSpecGlossCover.r, freezeFct);
			SnowSpecGloss.g = TargetValue(SnowSpecGloss.g, SnowSpecGlossCover.g, freezeFct);
			SnowSpecGloss.b = TargetValue(SnowSpecGloss.b, SnowSpecGlossCover.b, freezeFct);
			SnowSpecGloss.a = TargetValue(SnowSpecGloss.a, SnowSpecGlossCover.a, freezeFct);
			SnowGlitterColor.r = TargetValue(SnowGlitterColor.r, SnowGlitterColorCover.r, freezeFct);
			SnowGlitterColor.g = TargetValue(SnowGlitterColor.g, SnowGlitterColorCover.g, freezeFct);
			SnowGlitterColor.b = TargetValue(SnowGlitterColor.b, SnowGlitterColorCover.b, freezeFct);
			SnowGlitterColor.a = TargetValue(SnowGlitterColor.a, SnowGlitterColorCover.a, freezeFct);
			
			Frost -= (dt*temperatureFct*FROST_RATE); // frost
			if (Frost>1) Frost=1;

			SnowLevel -= fallIntensity*(dt*temperatureFct*SNOW_INCREASE_RATE);
			if (SnowLevel>1) SnowLevel=1; 

			if (AddWetWhenSnowPresent) {
				if (WaterLevel<SnowLevel) {
					WaterLevel=SnowLevel;
				}
			}

		}
		// (due to flowTimeScale we assume ice never evaporates)
		// (don't kill me for lack of any physical reasoning below... - just assume that wind has linear influence, temperature has linear influence)
		WaterLevel-=windF*(temperature+273f)*WATER_EVAPORATION_RATE*flowTimeScale*dt*snowPresentFct;
		if (WaterLevel<0) WaterLevel=0;
		WetnessAmount-=windF*(temperature+273f)*WET_EVAPORATION_RATE*flowTimeScale*dt*snowPresentFct;
		if (WetnessAmount<0) WetnessAmount=0;

		RefreshParticleSystem();

		bool changed = false;
		if (compareDelta (prev_RainIntensity, RainIntensity)) changed = true;
		else if (compareDelta (prev_temperature, temperature)) changed = true;
		else if (compareDelta (prev_flowTimeScale, flowTimeScale)) changed = true;
		else if (compareDelta (prev_FlowBumpStrength, FlowBumpStrength)) changed = true;
		else if (compareDelta (prev_WaterLevel, WaterLevel)) changed = true;
		else if (compareDelta (prev_WetnessAmount, WetnessAmount)) changed = true;
		else if (compareDelta (prev_SnowLevel, SnowLevel)) changed = true;
		else if (compareDelta (prev_SnowDissolve, SnowDissolve)) changed = true;
		else if (compareDelta (prev_SnowBumpMicro, SnowBumpMicro)) changed = true;
		else if (compareDelta (prev_SnowSpecGloss, SnowSpecGloss)) changed = true;
		else if (compareDelta (prev_SnowGlitterColor, SnowGlitterColor)) changed = true;

		if (changed) SetupIt (); 
	}

	bool compareDelta(float propA, float propB) {
		return Mathf.Abs(propA-propB)>0.000001f;
	}

	bool compareDelta(Color propA, Color propB) {
		if (Mathf.Abs (propA.r - propB.r) > 0.000001f) return true;
		if (Mathf.Abs (propA.g - propB.g) > 0.000001f) return true;
		if (Mathf.Abs (propA.b - propB.b) > 0.000001f) return true;
		if (Mathf.Abs (propA.a - propB.a) > 0.000001f) return true;
		return false;
	}

	// (delta must be positive)
	float TargetValue(float val, float target_val, float delta) {
		if (val < target_val) {
			val+=delta;
			if (val>target_val) val=target_val;
		} else if (val > target_val) {
			val-=delta;
			if (val<target_val) val=target_val;
		}
		return val;
	}

	public void RefreshParticleSystem() {
		if (paricleSystemActive != UseParticleSystem) {
			if (rainGameObject) rainGameObject.SetActive (UseParticleSystem);
			if (snowGameObject) snowGameObject.SetActive (UseParticleSystem);
			paricleSystemActive = UseParticleSystem;
        }
		if (UseParticleSystem) {
			if (rainGameObject != null) {
				rainGameObject.transform.position=transform.position+Vector3.up*3;
				if (psRain == null) {
					psRain = rainGameObject.GetComponent<ParticleSystem> ();
				}
			}
			if (snowGameObject != null) {
				snowGameObject.transform.position=transform.position+Vector3.up*3;
                if (psSnow == null) {
					psSnow = snowGameObject.GetComponent<ParticleSystem> ();
				}
			}
			if (psRain != null) {
                //psRain.emissionRate = (fallIntensity * 3000 * Mathf.Clamp01(temperature + 1));
                ParticleSystem.EmissionModule emission = psRain.emission;
                ParticleSystem.MinMaxCurve rateCurve = new ParticleSystem.MinMaxCurve(fallIntensity * 3000 * Mathf.Clamp01(temperature + 1));
                emission.rateOverTime = rateCurve;
                //psRain.emission = emission;
            }
            if (psSnow != null) {
				//psSnow.emissionRate = fallIntensity * 3000 * Mathf.Clamp01 (1 - temperature);
                ParticleSystem.EmissionModule emission = psSnow.emission;
                ParticleSystem.MinMaxCurve rateCurve = new ParticleSystem.MinMaxCurve(fallIntensity * 3000 * Mathf.Clamp01 (1 - temperature));
                emission.rateOverTime = rateCurve; 
                //psSnow.emission = emission;
            }
        }
	}
}
