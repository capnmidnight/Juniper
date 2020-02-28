/// <summary>
/// Shader Control - (C) Copyright 2016-2018 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {

        class KeywordView {
            public SCKeyword keyword;
            public List<SCShader> shaders;
            public bool foldout;
        }

        const string PRAGMA_COMMENT_MARK = "// Edited by Shader Control: ";
        const string PRAGMA_DISABLED_MARK = "// Disabled by Shader Control: ";
        const string BACKUP_SUFFIX = "_backup";
        const string PRAGMA_UNDERSCORE = "__ ";

        List<SCShader> shaders;
        int minimumKeywordCount;
        int totalShaderCount;
        int maxKeywordsCountFound = 0;
        int totalKeywords, totalGlobalKeywords, totalVariants, totalUsedKeywords, totalBuildVariants, totalGlobalShaderFeatures;
        Dictionary<string, List<SCShader>> uniqueKeywords, uniqueEnabledKeywords;
        Dictionary<string, SCKeyword> keywordsDict;
        List<KeywordView> keywordView;

        #region Shader handling

        void ScanProject() {
            try {
                if (shaders == null) {
                    shaders = new List<SCShader>();
                } else {
                    shaders.Clear();
                }
                // Add shaders from Resources folder
                string[] guids = AssetDatabase.FindAssets("t:Shader");
                totalShaderCount = guids.Length;
                for (int k = 0; k < totalShaderCount; k++) {
                    string guid = guids[k];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path != null) {
                        string pathUpper = path.ToUpper();
                        if (scanAllShaders || pathUpper.Contains("\\RESOURCES\\") || pathUpper.Contains("/RESOURCES/")) {   // this shader will be included in build
                            Shader unityShader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                            if (unityShader != null) {
                                SCShader shader = new SCShader();
                                shader.fullName = unityShader.name;
                                shader.name = SCShader.GetSimpleName(shader.fullName); //  Path.GetFileNameWithoutExtension(path);
                                shader.path = path;
                                string shaderGUID = path + "/" + unityShader.name;
                                shader.GUID = shaderGUID;
                                ScanShader(shader);
                                if (shader.keywords.Count > 0) {
                                    shaders.Add(shader);
                                }
                            }
                        }
                    }
                }

                // Load and reference materials
                Dictionary<string, SCShader> shaderCache = new Dictionary<string, SCShader>(shaders.Count);
                shaders.ForEach(shader => {
                    shaderCache.Add(shader.GUID, shader);
                });
                string[] matGuids = AssetDatabase.FindAssets("t:Material");
                int totaMatCount = matGuids.Length;
                for (int k = 0; k < totaMatCount; k++) {
                    string matGUID = matGuids[k];
                    string matPath = AssetDatabase.GUIDToAssetPath(matGUID);
                    Material mat = (Material)AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    if (mat.shader == null)
                        continue;
                    SCMaterial scMat = new SCMaterial(mat, matPath, matGUID);
                    scMat.SetKeywords(mat.shaderKeywords);
                    string path = AssetDatabase.GetAssetPath(mat.shader);
                    string shaderGUID = path + "/" + mat.shader.name;
                    SCShader shader;
                    if (shaderCache.ContainsKey(shaderGUID)) {
                        shader = shaderCache[shaderGUID];
                    } else {
                        if (mat.shaderKeywords == null || mat.shaderKeywords.Length == 0)
                            continue;
                        Shader shad = AssetDatabase.LoadAssetAtPath<Shader>(path);
                        // add non-sourced shader
                        shader = new SCShader();
                        shader.isReadOnly = IsFileWritable(path);
                        shader.GUID = shaderGUID;
                        if (shad != null) {
                            shader.fullName = shad.name;
                            shader.name = SCShader.GetSimpleName(shader.fullName); // Path.GetFileNameWithoutExtension(path);
                            shader.path = path;
                            ScanShader(shader);
                        } else {
                            shader.fullName = mat.shader.name;
                            shader.name = SCShader.GetSimpleName(shader.fullName);
                        }
                        shaders.Add(shader);
                        shaderCache.Add(shaderGUID, shader);
                        totalShaderCount++;
                    }
                    shader.materials.Add(scMat);
                    shader.AddKeywordsByName(mat.shaderKeywords);
                }

                // refresh variant and keywords count due to potential additional added keywords from materials (rogue keywords) and shader features count
                maxKeywordsCountFound = 0;
                shaders.ForEach((SCShader shader) => {
                    if (shader.keywordEnabledCount > maxKeywordsCountFound) {
                        maxKeywordsCountFound = shader.keywordEnabledCount;
                    }
                    shader.UpdateVariantCount();
                });

                switch (sortType) {
                    case SortType.VariantsCount:
                        shaders.Sort((SCShader x, SCShader y) => {
                            return y.actualBuildVariantCount.CompareTo(x.actualBuildVariantCount);
                        });
                        break;
                    case SortType.EnabledKeywordsCount:
                        shaders.Sort((SCShader x, SCShader y) => {
                            return y.keywordEnabledCount.CompareTo(x.keywordEnabledCount);
                        });
                        break;
                    case SortType.ShaderFileName:
                        shaders.Sort((SCShader x, SCShader y) => {
                            return x.name.CompareTo(y.name);
                        });
                        break;
                }
                UpdateProjectStats();
            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while scanning project: " + ex.Message);
            }
        }


        void ScanShader(SCShader shader) {

            // Inits shader
            shader.passes.Clear();
            shader.keywords.Clear();
            shader.hasBackup = File.Exists(shader.path + BACKUP_SUFFIX);
            shader.pendingChanges = false;
            shader.editedByShaderControl = shader.hasBackup;

            // Reads shader
            string[] shaderLines = File.ReadAllLines(shader.path);
            string[] separator = new string[] { " " };
            SCShaderPass currentPass = new SCShaderPass();
            SCShaderPass basePass = null;
            int pragmaControl = 0;
            int pass = -1;
            bool blockComment = false;
            SCKeywordLine keywordLine = new SCKeywordLine();
            for (int k = 0; k < shaderLines.Length; k++) {
                string line = shaderLines[k].Trim();
                if (line.Length == 0)
                    continue;

                int lineCommentIndex = line.IndexOf("//");
                int blocCommentIndex = line.IndexOf("/*");
                int endCommentIndex = line.IndexOf("*/");
                if (blocCommentIndex > 0 && (lineCommentIndex > blocCommentIndex || lineCommentIndex < 0)) {
                    blockComment = true;
                }
                if (endCommentIndex > blocCommentIndex && (lineCommentIndex > endCommentIndex || lineCommentIndex < 0)) {
                    blockComment = false;
                }
                if (blockComment)
                    continue;

                string lineUPPER = line.ToUpper();
                if (lineUPPER.Equals("PASS") || lineUPPER.StartsWith("PASS ")) {
                    if (pass >= 0) {
                        currentPass.pass = pass;
                        if (basePass != null)
                            currentPass.Add(basePass.keywordLines);
                        shader.Add(currentPass);
                    } else if (currentPass.keywordCount > 0) {
                        basePass = currentPass;
                    }
                    currentPass = new SCShaderPass();
                    pass++;
                    continue;
                }
                int j = line.IndexOf(PRAGMA_COMMENT_MARK);
                if (j >= 0) {
                    pragmaControl = 1;
                } else {
                    j = line.IndexOf(PRAGMA_DISABLED_MARK);
                    if (j >= 0)
                        pragmaControl = 3;
                }
                if (lineCommentIndex == 0 && pragmaControl != 1 && pragmaControl != 3) {
                    continue; // do not process lines commented by user
                }

                PragmaType pragmaType = PragmaType.Unknown;
                int offset = 0;
                j = line.IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_GLOBAL);
                if (j >= 0) {
                    pragmaType = PragmaType.MultiCompileGlobal;
                    offset = SCKeywordLine.PRAGMA_MULTICOMPILE_GLOBAL.Length;
                } else {
                    j = line.IndexOf(SCKeywordLine.PRAGMA_FEATURE_GLOBAL);
                    if (j >= 0) {
                        pragmaType = PragmaType.FeatureGlobal;
                        offset = SCKeywordLine.PRAGMA_FEATURE_GLOBAL.Length;
                    } else {
                        j = line.IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_LOCAL);
                        if (j >= 0) {
                            pragmaType = PragmaType.MultiCompileLocal;
                            offset = SCKeywordLine.PRAGMA_MULTICOMPILE_LOCAL.Length;
                        } else {
                            j = line.IndexOf(SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                            if (j >= 0) {
                                pragmaType = PragmaType.FeatureLocal;
                                offset = SCKeywordLine.PRAGMA_FEATURE_LOCAL.Length;
                            }
                        }
                    }
                }
                if (j >= 0) {
                    if (pragmaControl != 2) {
                        keywordLine = new SCKeywordLine();
                    }
                    keywordLine.pragmaType = pragmaType;
                    // exclude potential comments inside the #pragma line
                    int lastStringPos = line.IndexOf("//", j + offset);
                    if (lastStringPos < 0) {
                        lastStringPos = line.Length;
                    }
                    int length = lastStringPos - j - offset;
                    string[] kk = line.Substring(j + offset, length).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    // Sanitize keywords
                    for (int i = 0; i < kk.Length; i++) {
                        kk[i] = kk[i].Trim();
                    }
                    // Act on keywords
                    switch (pragmaControl) {
                        case 1: // Edited by Shader Control line
                            shader.editedByShaderControl = true;
                            // Add original keywords to current line
                            for (int s = 0; s < kk.Length; s++) {
                                keywordLine.Add(shader.GetKeyword(kk[s]));
                            }
                            pragmaControl = 2;
                            break;
                        case 2:
                            // check enabled keywords
                            keywordLine.DisableKeywords();
                            for (int s = 0; s < kk.Length; s++) {
                                SCKeyword keyword = keywordLine.GetKeyword(kk[s]);
                                if (keyword != null)
                                    keyword.enabled = true;
                            }
                            currentPass.Add(keywordLine);
                            pragmaControl = 0;
                            break;
                        case 3: // disabled by Shader Control line
                            shader.editedByShaderControl = true;
                            // Add original keywords to current line
                            for (int s = 0; s < kk.Length; s++) {
                                SCKeyword keyword = shader.GetKeyword(kk[s]);
                                keyword.enabled = false;
                                keywordLine.Add(keyword);
                            }
                            currentPass.Add(keywordLine);
                            pragmaControl = 0;
                            break;
                        case 0:
                            // Add keywords to current line
                            for (int s = 0; s < kk.Length; s++) {
                                keywordLine.Add(shader.GetKeyword(kk[s]));
                            }
                            currentPass.Add(keywordLine);
                            break;
                    }
                }
            }
            currentPass.pass = Mathf.Max(pass, 0);
            if (basePass != null)
                currentPass.Add(basePass.keywordLines);
            shader.Add(currentPass);
            shader.UpdateVariantCount();
        }

        void UpdateProjectStats() {
            totalKeywords = 0;
            totalGlobalKeywords = 0;
            totalUsedKeywords = 0;
            totalVariants = 0;
            totalBuildVariants = 0;
            totalGlobalShaderFeatures = 0;

            if (shaders == null)
                return;

            if (keywordsDict == null) {
                keywordsDict = new Dictionary<string, SCKeyword>();
            } else {
                keywordsDict.Clear();
            }
            if (uniqueKeywords == null) {
                uniqueKeywords = new Dictionary<string, List<SCShader>>();
            } else {
                uniqueKeywords.Clear();
            }
            if (uniqueEnabledKeywords == null) {
                uniqueEnabledKeywords = new Dictionary<string, List<SCShader>>();
            } else {
                uniqueEnabledKeywords.Clear();
            }

            int shadersCount = shaders.Count;
            for (int k = 0; k < shadersCount; k++) {
                SCShader shader = shaders[k];
                int keywordsCount = shader.keywords.Count;
                for (int w = 0; w < keywordsCount; w++) {
                    SCKeyword keyword = shader.keywords[w];
                    List<SCShader> shadersWithThisKeyword;
                    if (!uniqueKeywords.TryGetValue(keyword.name, out shadersWithThisKeyword)) {
                        shadersWithThisKeyword = new List<SCShader>();
                        uniqueKeywords[keyword.name] = shadersWithThisKeyword;
                        totalKeywords++;
                        if (keyword.isGlobal) totalGlobalKeywords++;
                        if (keyword.isGlobal && !keyword.isMultiCompile) totalGlobalShaderFeatures++;
                        keywordsDict[keyword.name] = keyword;
                    }
                    shadersWithThisKeyword.Add(shader);
                    if (keyword.enabled) {
                        List<SCShader> shadersWithThisKeywordEnabled;
                        if (!uniqueEnabledKeywords.TryGetValue(keyword.name, out shadersWithThisKeywordEnabled)) {
                            shadersWithThisKeywordEnabled = new List<SCShader>();
                            uniqueEnabledKeywords[keyword.name] = shadersWithThisKeywordEnabled;
                            totalUsedKeywords++;
                        }
                        shadersWithThisKeywordEnabled.Add(shader);
                    }
                }
                totalVariants += shader.totalVariantCount;
                totalBuildVariants += shader.actualBuildVariantCount;
            }

            if (keywordView == null) {
                keywordView = new List<KeywordView>();
            } else {
                keywordView.Clear();
            }
            foreach (KeyValuePair<string, List<SCShader>> kvp in uniqueEnabledKeywords) {
                SCKeyword kw;
                if (!keywordsDict.TryGetValue(kvp.Key, out kw)) continue;
                KeywordView kv = new KeywordView { keyword = kw, shaders = kvp.Value };
                keywordView.Add(kv);
            }
            keywordView.Sort(delegate (KeywordView x, KeywordView y) {
                return y.shaders.Count.CompareTo(x.shaders.Count);
            });
        }

        bool IsFileWritable(string path) {
            FileStream stream = null;

            try {
                FileAttributes fileAttributes = File.GetAttributes(path);
                if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    return true;
                }
                FileInfo file = new FileInfo(path);
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            } catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            } finally {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        void MakeBackup(SCShader shader) {
            string backupPath = shader.path + BACKUP_SUFFIX;
            if (!File.Exists(backupPath)) {
                AssetDatabase.CopyAsset(shader.path, backupPath);
                shader.hasBackup = true;
            }
        }

        void UpdateShader(SCShader shader) {
            if (shader.isReadOnly) {
                EditorUtility.DisplayDialog("Locked file", "Shader file " + shader.name + " is read-only.", "Ok");
                return;
            }
            try {
                // Create backup
                MakeBackup(shader);

                // Reads and updates shader from disk
                string[] shaderLines = File.ReadAllLines(shader.path);
                string[] separator = new string[] { " " };
                StringBuilder sb = new StringBuilder();
                int pragmaControl = 0;
                shader.editedByShaderControl = false;
                SCKeywordLine keywordLine = new SCKeywordLine();
                bool blockComment = false;
                for (int k = 0; k < shaderLines.Length; k++) {

                    int lineCommentIndex = shaderLines[k].IndexOf("//");
                    int blocCommentIndex = shaderLines[k].IndexOf("/*");
                    int endCommentIndex = shaderLines[k].IndexOf("*/");
                    if (blocCommentIndex > 0 && (lineCommentIndex > blocCommentIndex || lineCommentIndex < 0)) {
                        blockComment = true;
                    }
                    if (endCommentIndex > blocCommentIndex && (lineCommentIndex > endCommentIndex || lineCommentIndex < 0)) {
                        blockComment = false;
                    }

                    int j = -1;
                    PragmaType pragmaType = PragmaType.Unknown;
                    if (!blockComment) {
                        j = shaderLines[k].IndexOf(PRAGMA_COMMENT_MARK);
                        if (j >= 0) {
                            pragmaControl = 1;
                        }
                        j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_GLOBAL);
                        if (j >= 0) {
                            pragmaType = PragmaType.MultiCompileGlobal;
                        } else {
                            j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_FEATURE_GLOBAL);
                            if (j >= 0) {
                                pragmaType = PragmaType.FeatureGlobal;
                            } else {
                                j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_MULTICOMPILE_LOCAL);
                                if (j >= 0) {
                                    pragmaType = PragmaType.MultiCompileLocal;
                                } else {
                                    j = shaderLines[k].IndexOf(SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                                    if (j >= 0) {
                                        pragmaType = PragmaType.FeatureLocal;
                                    }

                                }
                            }
                        }
                        if (pragmaControl != 1 && lineCommentIndex == 0 && shaderLines[k].IndexOf(PRAGMA_DISABLED_MARK) < 0) {
                            // do not process a commented line
                            j = -1;
                        }
                    }
                    if (j >= 0) {
                        if (pragmaControl != 2) {
                            keywordLine.Clear();
                        }
                        keywordLine.pragmaType = pragmaType;
                        j = shaderLines[k].IndexOf(' ', j + 20) + 1; // first space after pragma declaration
                        if (j >= shaderLines[k].Length) continue;
                        // exclude potential comments inside the #pragma line
                        int lastStringPos = shaderLines[k].IndexOf("//", j);
                        if (lastStringPos < 0) {
                            lastStringPos = shaderLines[k].Length;
                        }
                        int length = lastStringPos - j;
                        string[] kk = shaderLines[k].Substring(j, length).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        // Sanitize keywords
                        for (int i = 0; i < kk.Length; i++) {
                            kk[i] = kk[i].Trim();
                        }
                        // Act on keywords
                        switch (pragmaControl) {
                            case 1:
                                // Read original keywords
                                for (int s = 0; s < kk.Length; s++) {
                                    SCKeyword keyword = shader.GetKeyword(kk[s]);
                                    keywordLine.Add(keyword);
                                }
                                pragmaControl = 2;
                                break;
                            case 0:
                            case 2:
                                if (pragmaControl == 0) {
                                    for (int s = 0; s < kk.Length; s++) {
                                        SCKeyword keyword = shader.GetKeyword(kk[s]);
                                        keywordLine.Add(keyword);
                                    }
                                }
                                int kCount = keywordLine.keywordCount;
                                int kEnabledCount = keywordLine.keywordsEnabledCount;
                                if (kEnabledCount < kCount) {
                                    // write original keywords
                                    if (kEnabledCount == 0) {
                                        sb.Append(PRAGMA_DISABLED_MARK);
                                    } else {
                                        sb.Append(PRAGMA_COMMENT_MARK);
                                    }
                                    shader.editedByShaderControl = true;
                                    sb.Append(keywordLine.GetPragma());
                                    if (keywordLine.hasUnderscoreVariant)
                                        sb.Append(PRAGMA_UNDERSCORE);
                                    for (int s = 0; s < kCount; s++) {
                                        SCKeyword keyword = keywordLine.keywords[s];
                                        sb.Append(keyword.name);
                                        if (s < kCount - 1)
                                            sb.Append(" ");
                                    }
                                    sb.AppendLine();
                                }

                                if (kEnabledCount > 0) {
                                    // Write actual keywords
                                    sb.Append(keywordLine.GetPragma());
                                    if (keywordLine.hasUnderscoreVariant)
                                        sb.Append(PRAGMA_UNDERSCORE);
                                    for (int s = 0; s < kCount; s++) {
                                        SCKeyword keyword = keywordLine.keywords[s];
                                        if (keyword.enabled) {
                                            sb.Append(keyword.name);
                                            if (s < kCount - 1)
                                                sb.Append(" ");
                                        }
                                    }
                                    sb.AppendLine();
                                }
                                pragmaControl = 0;
                                break;
                        }
                    } else {
                        sb.AppendLine(shaderLines[k]);
                    }
                }

                // Writes modified shader
                File.WriteAllText(shader.path, sb.ToString());
                AssetDatabase.Refresh();

                // Also update materials
                CleanMaterials(shader);

                ScanShader(shader); // Rescan shader

                // do not include in build (sync with Build View)
                BuildUpdateShaderKeywordsState(shader);

            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while updating shader: " + ex.Message);
            }
        }

        void RestoreShader(SCShader shader) {
            try {
                string shaderBackupPath = shader.path + BACKUP_SUFFIX;
                if (!File.Exists(shaderBackupPath)) {
                    EditorUtility.DisplayDialog("Restore shader", "Shader backup is missing!", "OK");
                    return;
                }
                File.Copy(shaderBackupPath, shader.path, true);
                File.Delete(shaderBackupPath);
                if (File.Exists(shaderBackupPath + ".meta"))
                    File.Delete(shaderBackupPath + ".meta");
                AssetDatabase.Refresh();

                ScanShader(shader); // Rescan shader
                UpdateProjectStats();
            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while restoring shader: " + ex.Message);
            }
        }

        #endregion

        #region Material handling

        void CleanMaterials(SCShader shader) {
            // Updates any material using this shader
            Shader shad = (Shader)AssetDatabase.LoadAssetAtPath<Shader>(shader.path);
            if (shad != null) {
                bool requiresSave = false;
                string[] matGUIDs = AssetDatabase.FindAssets("t:Material");
                foreach (string matGUID in matGUIDs) {
                    string matPath = AssetDatabase.GUIDToAssetPath(matGUID);
                    Material mat = (Material)AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    if (mat != null && mat.shader.name.Equals(shad.name)) {
                        foreach (SCKeyword keyword in shader.keywords) {
                            foreach (string matKeyword in mat.shaderKeywords) {
                                if (matKeyword.Equals(keyword.name)) {
                                    if (!keyword.enabled && mat.IsKeywordEnabled(keyword.name)) {
                                        mat.DisableKeyword(keyword.name);
                                        EditorUtility.SetDirty(mat);
                                        requiresSave = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                if (requiresSave) {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        void CleanAllMaterials() {
            if (!EditorUtility.DisplayDialog("Clean All Materials", "This option will scan all materials and will prune any disabled keywords. This option is provided to ensure no materials are referencing a disabled shader keyword.\n\nRemember: to disable keywords, first expand any shader from the list and uncheck the unwanted keywords (press 'Save' to modify the shader file and to clean any existing material that uses that specific shader).\n\nDo you want to continue?", "Yes", "Cancel")) {
                return;
            }
            try {
                for (int k = 0; k < shaders.Count; k++) {
                    CleanMaterials(shaders[k]);
                }
                ScanProject();
                Debug.Log("Cleaning finished.");
            } catch (Exception ex) {
                Debug.LogError("Unexpected exception caught while cleaning materials: " + ex.Message);
            }
        }

        void PruneMaterials(SCShader shader, string keywordName) {
            try {
                bool requiresSave = false;
                int materialCount = shader.materials.Count;
                for (int k = 0; k < materialCount; k++) {
                    SCMaterial material = shader.materials[k];
                    if (material.ContainsKeyword(keywordName)) {
                        Material theMaterial = (Material)AssetDatabase.LoadAssetAtPath<Material>(shader.materials[k].path);
                        if (theMaterial == null)
                            continue;
                        theMaterial.DisableKeyword(keywordName);
                        EditorUtility.SetDirty(theMaterial);
                        material.RemoveKeyword(keywordName);
                        shader.RemoveKeyword(keywordName);
                        requiresSave = true;
                    }
                }
                if (requiresSave) {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            } catch (Exception ex) {
                Debug.Log("Unexpected exception caught while pruning materials: " + ex.Message);
            }

        }

        void ConvertToLocal(SCKeyword keyword) {
            List<SCShader> shaders;
            if (!uniqueKeywords.TryGetValue(keyword.name, out shaders)) return;
            if (shaders == null) return;
            for (int k=0;k<shaders.Count;k++) {
                ConvertToLocal(keyword, shaders[k]);
            }
            AssetDatabase.Refresh();
        }

        void ConvertToLocal(SCKeyword keyword, SCShader shader) {
            // Check total local keyword does not exceed 64 limit
            int potentialCount = 0;
            int kwCount = shader.keywords.Count;
            for (int k=0;k<kwCount;k++) {
                SCKeyword kw = shader.keywords[k];
                if (!kw.isMultiCompile) potentialCount++;
            }
            if (potentialCount > 64) return;

            string path = shader.path;
            if (!File.Exists(path)) return;
            string[] lines = File.ReadAllLines(path);
            bool changed = false;
            for (int k = 0; k < lines.Length; k++) {
                // Just convert to local shader_features for now since multi_compile global keywords can be nabled using the Shader global API
                if (lines[k].IndexOf(SCKeywordLine.PRAGMA_FEATURE_GLOBAL, StringComparison.InvariantCultureIgnoreCase) >= 0 && lines[k].IndexOf(keyword.name, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                    lines[k] = lines[k].Replace(SCKeywordLine.PRAGMA_FEATURE_GLOBAL, SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                    lines[k] = lines[k].Replace(SCKeywordLine.PRAGMA_FEATURE_GLOBAL.ToUpper(), SCKeywordLine.PRAGMA_FEATURE_LOCAL);
                    changed = true;
                }
            }
            if (changed) {
                MakeBackup(shader);
                File.WriteAllLines(path, lines, Encoding.UTF8);
            }
        }

        void ConvertToLocalAll() {
            int kvCount = keywordView.Count;
            for (int s = 0; s < kvCount; s++) {
                SCKeyword keyword = keywordView[s].keyword;
                if (keyword.isGlobal && !keyword.isMultiCompile) {
                    ConvertToLocal(keyword);
                }
            }
            AssetDatabase.Refresh();
        }

        #endregion
    }

}