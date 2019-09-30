using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Azure;
using Juniper.Azure.CognitiveServices;
using Juniper.Data;
using Juniper.Serialization;
using Juniper.Speech;

using UnityEditor;

using UnityEngine;

namespace Juniper.Events
{
    [CustomEditor(typeof(Speakable))]
    public class SpeechOutputEditor : Editor
    {
        private static readonly GUIContent VoiceLocaleDropdownLabel = new GUIContent("Locale");
        private static readonly GUIContent VoiceGenderDropdownLabel = new GUIContent("Gender");
        private static readonly GUIContent VoiceNameDropdownLabel = new GUIContent("Voice");

        private static Voice[] voices;

        static SpeechOutputEditor()
        {
            if (voices == null)
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
                if (File.Exists(keyFile))
                {
                    var lines = File.ReadAllLines(keyFile);
                    var azureApiKey = lines[0];
                    var azureRegion = lines[1];
                    var cache = new UnityCachingStrategy("Assets");
                    var voiceListDecoder = new JsonFactory<Voice[]>();
                    var voiceListRequest = new VoiceListRequest(azureRegion);
                    var task = Task.CompletedTask;
                    if (!cache.IsCached(voiceListRequest))
                    {
                        var plainText = new StreamStringDecoder();
                        var tokenRequest = new AuthTokenRequest(azureRegion, azureApiKey);
                        task = tokenRequest.GetDecoded(plainText)
                            .ContinueWith(tT =>
                                voiceListRequest.AuthToken = tT.Result);
                    }

                    task.ContinueWith(async (_) =>
                        voices = await cache.GetDecoded(voiceListRequest, voiceListDecoder))
                        .ConfigureAwait(false);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (voices != null)
            {
                serializedObject.Update();
                var value = (Speakable)serializedObject.targetObject;
                var voiceLanguages = voices
                    .Select(v => v.Locale)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToArray();

                var voiceLanguagesLabels = voiceLanguages.ToGUIContents();

                var selectedLocaleIndex = ArrayUtility.IndexOf(voiceLanguages, value.voiceLanguage);
                selectedLocaleIndex = EditorGUILayout.Popup(VoiceLocaleDropdownLabel, selectedLocaleIndex, voiceLanguagesLabels);

                if (0 <= selectedLocaleIndex)
                {
                    var selectedLanguage = voiceLanguages[selectedLocaleIndex];
                    if(selectedLanguage != value.voiceLanguage)
                    {
                        value.voiceLanguage = selectedLanguage;
                        value.voiceGender = string.Empty;
                        value.voiceName = string.Empty;
                    }

                    var langVoices = voices
                        .Where(v => v.Locale == value.voiceLanguage);

                    var voiceGenders = langVoices
                        .Select(v => v.Gender)
                        .Distinct()
                        .ToArray();

                    var voiceGenderLabels = voiceGenders.ToGUIContents();

                    var selectedGenderIndex = ArrayUtility.IndexOf(voiceGenders, value.voiceGender);
                    selectedGenderIndex = EditorGUILayout.Popup(VoiceGenderDropdownLabel, selectedGenderIndex, voiceGenderLabels);

                    if (0 <= selectedGenderIndex)
                    {
                        var selectedGender = voiceGenders[selectedGenderIndex];
                        if (selectedGender != value.voiceGender)
                        {
                            value.voiceGender = selectedGender;
                            value.voiceName = string.Empty;
                        }

                        var gendVoices = langVoices
                            .Where(v => v.Gender == value.voiceGender);

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
                    }
                    else
                    {
                        value.voiceGender = string.Empty;
                    }
                }
                else
                {
                    value.voiceLanguage = string.Empty;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
