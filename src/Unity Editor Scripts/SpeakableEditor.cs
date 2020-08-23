using System;
using System.IO;
using System.Linq;

using Juniper.IO;
using Juniper.Sound;
using Juniper.Speech;
using Juniper.Speech.Azure.CognitiveServices;

using UnityEditor;

using UnityEngine;

using static UnityEditor.EditorGUILayout;

namespace Juniper.Events
{

    [CustomEditor(typeof(Speakable))]
    public class SpeakableEditor : Editor
    {
#if AZURE_SPEECHSDK
        private static readonly GUIContent VoiceLocaleDropdownLabel = new GUIContent("Locale");
        private static readonly GUIContent VoiceGenderDropdownLabel = new GUIContent("Gender");
        private static readonly GUIContent VoiceNameDropdownLabel = new GUIContent("Voice");
        private static readonly Voice[] voices = GetVoices();

        private static Voice[] GetVoices()
        {
            Voice[] voices = null;
            var audio = Find.Any<InteractionAudio>();
            if (audio != null)
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var assetsRoot = Path.Combine(userProfile, "Box", "VR Initiatives", "Engineering", "Assets");
                var keyFile = Path.Combine(assetsRoot, "DevKeys", "azure-speech.txt");
                if (File.Exists(keyFile))
                {
                    var lines = File.ReadAllLines(keyFile);
                    var azureApiKey = lines[0];
                    var azureRegion = lines[1];
                    var cache = audio.GetCachingStrategy();
                    var voicesDecoder = new JsonFactory<Voice[]>();
                    var voicesClient = new VoicesClient(azureRegion, azureApiKey, voicesDecoder, cache);
                    var voicesTask = voicesClient.GetVoicesAsync();
                    voices = voicesTask.Result;
                }
            }

            return voices;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "<Pending>")]
        public override void OnInspectorGUI()
        {
            using (new EditorGUI.ChangeCheckScope())
            {
                serializedObject.Update();

                var value = (Speakable)serializedObject.targetObject;
                EditorGUILayoutExt.ShowScriptField(value);

                PropertyField(serializedObject.FindProperty("playOnAwake"), new GUIContent("Play on Awake"));
                PropertyField(serializedObject.FindProperty("text"), new GUIContent("Text"));

                if (voices == null)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        Popup(VoiceLocaleDropdownLabel, 0, Array.Empty<GUIContent>());
                    }
                }
                else
                {

                    var voiceLanguages = voices
                        .Select(v => v.Locale)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToArray();

                    var voiceLanguagesLabels = voiceLanguages.ToGUIContents();

                    var selectedLocaleIndex = ArrayUtility.IndexOf(voiceLanguages, value.voiceLanguage);
                    selectedLocaleIndex = Popup(VoiceLocaleDropdownLabel, selectedLocaleIndex, voiceLanguagesLabels);

                    if (selectedLocaleIndex < 0)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            Popup(VoiceGenderDropdownLabel, 0, Array.Empty<GUIContent>());
                        }
                    }
                    else
                    {
                        var selectedLanguage = voiceLanguages[selectedLocaleIndex];
                        if (selectedLanguage != value.voiceLanguage)
                        {
                            value.voiceLanguage = selectedLanguage;
                            value.voiceGender = string.Empty;
                            value.voiceName = string.Empty;
                        }

                        var langVoices = voices
                            .Where(v => v.Locale == selectedLanguage);

                        var voiceGenders = langVoices
                            .Select(v => v.Gender)
                            .Distinct()
                            .ToArray();

                        var voiceGenderLabels = voiceGenders.ToGUIContents();

                        var selectedGenderIndex = ArrayUtility.IndexOf(voiceGenders, value.voiceGender);
                        selectedGenderIndex = Popup(VoiceGenderDropdownLabel, selectedGenderIndex, voiceGenderLabels);

                        if (selectedGenderIndex < 0)
                        {
                            Popup(VoiceNameDropdownLabel, 0, Array.Empty<GUIContent>());
                        }
                        else
                        {
                            var selectedGender = voiceGenders[selectedGenderIndex];
                            if (selectedGender != value.voiceGender)
                            {
                                value.voiceGender = selectedGender;
                                value.voiceName = string.Empty;
                            }

                            var gendVoices = langVoices
                                .Where(v => v.Gender == selectedGender);

                            var voiceNames = gendVoices
                                .Select(v => v.ShortName)
                                .ToArray();

                            var voiceNameLabels = voiceNames.ToGUIContents();

                            var selectedNameIndex = ArrayUtility.IndexOf(voiceNames, value.voiceName);
                            selectedNameIndex = Popup(VoiceNameDropdownLabel, selectedNameIndex, voiceNameLabels);
                            if (0 <= selectedNameIndex)
                            {
                                value.voiceName = voiceNames[selectedNameIndex];

                                value.pitch = Slider("Pitch", value.pitch, -0.1f, 3f);
                                value.speakingRate = Slider("Rate", value.speakingRate, -0.1f, 5f);
                            }
                            else
                            {
                                value.voiceName = string.Empty;
                            }
                        }
                    }

                }

                PropertyField(serializedObject.FindProperty("OnEnd"), new GUIContent("On End Event"));
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
