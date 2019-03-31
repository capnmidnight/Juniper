namespace Juniper.Unity.Audio
{
    /// <summary>
    /// The audio portion of the interaction system.
    /// </summary>
    public class InteractionAudio :
#if RESONANCEAUDIO
        ResonanceInteractionAudio
#elif UNITY_XR_OCULUS
        OculusInteractionAudio
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRInteractionAudio
#elif UNITY_XR_MAGICLEAP
        MagicLeapInteractionAudio
#else
        DefaultInteractionAudio
#endif
    {
    }
}
