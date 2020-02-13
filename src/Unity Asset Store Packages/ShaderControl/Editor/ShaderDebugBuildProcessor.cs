using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderControl {

    class ShaderDebugBuildProcessor : IPreprocessShaders, IPostprocessBuildWithReport {

        ShadersBuildInfo shadersBuildInfo;

        public static void CheckShadersBuildStore() {
            // Check if scriptable object exists
            string path = GetStoredDataPath();
            if (!File.Exists(path)) {
                string dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir);
                ShadersBuildInfo shadersBuildInfo = ScriptableObject.CreateInstance<ShadersBuildInfo>();
                AssetDatabase.CreateAsset(shadersBuildInfo, path);
                AssetDatabase.SaveAssets();
            }
        }


        public void OnPostprocessBuild(BuildReport report) {
            SaveResults();
        }

        public int callbackOrder { get { return 0; } }

        static string GetStoredDataPath() {
            // Locate shader control path
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int k = 0; k < paths.Length; k++) {
                if (paths[k].EndsWith("/ShaderControl/Editor", StringComparison.InvariantCulture)) {
                    return paths[k] + "/Resources/BuiltShaders.asset";
                }
            }
            return null;
        }

        void SaveResults() {

            SCWindow.SetEditorPrefBool("QUICK_BUILD", false);

            if (shadersBuildInfo != null) {
                shadersBuildInfo.creationDateTicks = DateTime.Now.Ticks;
                EditorUtility.SetDirty(shadersBuildInfo);
                string filename = GetStoredDataPath();
                if (filename == null) {
                    Debug.LogError("Shader Control path not found.");
                } else {
                    AssetDatabase.SaveAssets();
                }
            }
            SCWindow.issueRefresh = true;
        }

        public void OnProcessShader(
            Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerData) {

            bool skipCompilation = false;
            if (SCWindow.GetEditorPrefBool("QUICK_BUILD", false)) {
                skipCompilation = true;
            }

            if (shadersBuildInfo == null) {
                string filename = GetStoredDataPath();
                shadersBuildInfo = AssetDatabase.LoadAssetAtPath<ShadersBuildInfo>(filename);
                if (shadersBuildInfo == null) {
                    shadersBuildInfo = ScriptableObject.CreateInstance<ShadersBuildInfo>();
                    EditorUtility.SetDirty(shadersBuildInfo);
                }
            }

            ShaderBuildInfo sb = shadersBuildInfo.GetShader(shader.name);
            if (sb == null) {
                sb = new ShaderBuildInfo();
                sb.name = shader.name;
                sb.simpleName = GetSimpleName(sb.name);
                sb.type = snippet.shaderType;
                string path = AssetDatabase.GetAssetPath(shader);
                sb.isInternal = string.IsNullOrEmpty(path) || !File.Exists(path);
                shadersBuildInfo.Add(sb);
                EditorUtility.SetDirty(shadersBuildInfo);
            } else if (!sb.includeInBuild) {
                skipCompilation = true;
            }

            int count = shaderCompilerData.Count;
            for (int i = 0; i < count; ++i) {
                ShaderKeywordSet ks = shaderCompilerData[i].shaderKeywordSet;
                foreach (ShaderKeyword kw in ks.GetShaderKeywords()) {
                    string kname = kw.GetKeywordName();
                    if (!sb.KeywordsIsIncluded(kname)) {
                        shaderCompilerData.RemoveAt(i);
                        count--;
                        i--;
                        break;
                    } else {
                        EditorUtility.SetDirty(shadersBuildInfo);
                    }
                }
            }

            if (skipCompilation) {
                shaderCompilerData.Clear();
            }

        }

        string GetSimpleName(string longName) {
            int k = longName.LastIndexOf("/");
            if (k >= 0) {
                return longName.Substring(k + 1);
            }
            return longName;
        }

    }
}