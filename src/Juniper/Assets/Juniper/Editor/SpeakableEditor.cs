using System;
using System.IO;
using System.Linq;

using Juniper.Azure.CognitiveServices;
using Juniper.IO;
using Juniper.Speech;

using UnityEditor;

using UnityEngine;

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
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
            Voice[] voices = null;
            if (File.Exists(keyFile))
            {
                var lines = File.ReadAllLines(keyFile);
                var azureApiKey = lines[0];
                var azureRegion = lines[1];
                var cache = new CachingStrategy()
                    .AddLayer(new FileCacheLayer(Path.Combine("Assets", "StreamingAssets")));
                var voicesDecoder = new JsonFactory<Voice[]>();
                var voicesClient = new VoicesClient(azureRegion, azureApiKey, voicesDecoder, cache);
                var voicesTask = voicesClient.GetVoices();
                voicesTask.ConfigureAwait(false);
                voices = voicesTask.Result;
            }

            return voices;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            var value = (Speakable)serializedObject.targetObject;
            EditorGUILayoutExt.ShowScriptField(value);
            if (voices == null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.Popup(VoiceLocaleDropdownLabel, 0, Array.Empty<GUIContent>());
                }
            }
            else
            {
                value.text = EditorGUILayout.TextField("Text", value.text);

                var voiceLanguages = voices
                    .Select(v => v.Locale)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToArray();

                var voiceLanguagesLabels = voiceLanguages.ToGUIContents();

                var selectedLocaleIndex = ArrayUtility.IndexOf(voiceLanguages, value.voiceLanguage);
                selectedLocaleIndex = EditorGUILayout.Popup(VoiceLocaleDropdownLabel, selectedLocaleIndex, voiceLanguagesLabels);

                if (selectedLocaleIndex < 0)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.Popup(VoiceGenderDropdownLabel, 0, Array.Empty<GUIContent>());
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
                    selectedGenderIndex = EditorGUILayout.Popup(VoiceGenderDropdownLabel, selectedGenderIndex, voiceGenderLabels);

                    if (selectedGenderIndex < 0)
                    {
                        EditorGUILayout.Popup(VoiceNameDropdownLabel, 0, Array.Empty<GUIContent>());
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
                        selectedNameIndex = EditorGUILayout.Popup(VoiceNameDropdownLabel, selectedNameIndex, voiceNameLabels);
                        if (0 <= selectedNameIndex)
                        {
                            value.voiceName = voiceNames[selectedNameIndex];
                        }
                        else
                        {
                            value.voiceName = string.Empty;
                        }

                        value.pitch = EditorGUILayout.Slider("Pitch", value.pitch, -0.1f, 3f);
                        value.speakingRate = EditorGUILayout.Slider("Rate", value.speakingRate, -0.1f, 5f);
                    }
                }

            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
#endif
    }
}
