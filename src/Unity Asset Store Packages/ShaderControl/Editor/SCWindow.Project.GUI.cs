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

        enum SortType {
            VariantsCount = 0,
            EnabledKeywordsCount = 1,
            ShaderFileName = 2,
            Keyword = 3
        }


        SortType sortType = SortType.VariantsCount;
        GUIStyle blackStyle, commentStyle, disabledStyle, foldoutBold, foldoutNormal, foldoutDim, foldoutRTF, titleStyle;
        Vector2 scrollViewPosProject;
        bool firstTime;
        GUIContent matIcon, shaderIcon;
        bool scanAllShaders;
        string keywordFilter;
        string projectShaderNameFilter;

        void DrawProjectGUI() {

            GUILayout.Box(new GUIContent("List of shader files in your project and modify their keywords.\nOnly shaders with source code or referenced by materials are included in this tab.\nUse the <color=orange><b>Build View</b></color> tab for a complete list of shaders, including hidden/internal Unity shaders."), titleStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            scanAllShaders = EditorGUILayout.Toggle(new GUIContent("Force scan all shaders", "Also includes shaders that are not located within a Resources folder"), scanAllShaders);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Scan Project Folders", "Quickly scans the project and finds shaders that use keywords."))) {
                ScanProject();
                GUIUtility.ExitGUI();
                return;
            }
            if (shaders != null && shaders.Count > 0 && GUILayout.Button(new GUIContent("Clean All Materials", "Removes all disabled keywords in all materials.  This option is provided to ensure no existing materials are referencing a disabled shader keyword.\n\nTo disable keywords, first expand any shader from the list and uncheck the unwanted keywords (press 'Save' to modify the shader file and to clean any existing material that uses that specific shader)."), GUILayout.Width(120))) {
                CleanAllMaterials();
                GUIUtility.ExitGUI();
                return;
            }
            if (GUILayout.Button("Help", GUILayout.Width(40)))
                ShowHelpWindow();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (shaders != null) {
                if (totalKeywords == totalUsedKeywords || totalKeywords == 0) {
                    EditorGUILayout.HelpBox("Shaders Found: " + totalShaderCount.ToString() + "  Shaders Using Keywords: " + shaders.Count.ToString() + "\nKeywords: " + totalKeywords.ToString() + "  Variants: " + totalVariants.ToString(), MessageType.Info);
                } else {
                    int keywordsPerc = totalUsedKeywords * 100 / totalKeywords;
                    int variantsPerc = totalBuildVariants * 100 / totalVariants;
                    EditorGUILayout.HelpBox("Shaders Found: " + totalShaderCount.ToString() + "  Shaders Using Keywords: " + shaders.Count.ToString() + "\nUsed Keywords: " + totalUsedKeywords.ToString() + " of " + totalKeywords.ToString() + " (" + keywordsPerc.ToString() + "%)  Actual Variants: " + totalBuildVariants.ToString() + " of " + totalVariants.ToString() + " (" + variantsPerc.ToString() + "%)", MessageType.Info);
                }
                EditorGUILayout.Separator();
                int shaderCount = shaders.Count;
                if (shaderCount > 1) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Sort By", GUILayout.Width(90));
                    SortType prevSortType = sortType;
                    sortType = (SortType)EditorGUILayout.EnumPopup(sortType);
                    if (sortType != prevSortType) {
                        ScanProject();
                        EditorGUIUtility.ExitGUI();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Filter", GUILayout.Width(90));
                    projectShaderNameFilter = EditorGUILayout.TextField(projectShaderNameFilter);
                    if (GUILayout.Button(new GUIContent("Clear", "Clear filter."), EditorStyles.miniButton, GUILayout.Width(60))) {
                        projectShaderNameFilter = "";
                        GUIUtility.keyboardControl = 0;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (sortType != SortType.Keyword) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Keywords >=", GUILayout.Width(90));
                        minimumKeywordCount = EditorGUILayout.IntSlider(minimumKeywordCount, 0, maxKeywordsCountFound);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Keyword Filter", GUILayout.Width(90));
                    keywordFilter = EditorGUILayout.TextField(keywordFilter);
                    if (GUILayout.Button(new GUIContent("Clear", "Clear filter."), EditorStyles.miniButton, GUILayout.Width(60))) {
                        keywordFilter = "";
                        GUIUtility.keyboardControl = 0;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();
                }
                scrollViewPosProject = EditorGUILayout.BeginScrollView(scrollViewPosProject);
                if (sortType == SortType.Keyword) {
                    if (keywordView != null) {
                        int kvCount = keywordView.Count;
                        for (int s = 0; s < kvCount; s++) {
                            if (!string.IsNullOrEmpty(keywordFilter) && keywordView[s].keyword.IndexOf(keywordFilter, StringComparison.CurrentCultureIgnoreCase) < 0)
                                continue;
                            EditorGUILayout.BeginHorizontal();
                            keywordView[s].foldout = EditorGUILayout.Foldout(keywordView[s].foldout, new GUIContent("Keyword <b>" + keywordView[s].keyword + "</b> found in " + keywordView[s].shaders.Count + " shader(s)"), foldoutRTF);
                            EditorGUILayout.EndHorizontal();
                            if (keywordView[s].foldout) {
                                int kvShadersCount = keywordView[s].shaders.Count;
                                for (int m = 0; m < kvShadersCount; m++) {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("", GUILayout.Width(30));
                                    EditorGUILayout.LabelField(shaderIcon, GUILayout.Width(18));
                                    EditorGUILayout.LabelField(keywordView[s].shaders[m].name);
                                    SCShader shader = keywordView[s].shaders[m];
                                    GUI.enabled = shader.hasSource;
                                    if (GUILayout.Button(new GUIContent("Locate", "Locates the shader in the shader view."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                        Shader theShader = AssetDatabase.LoadAssetAtPath<Shader>(shader.path);
                                        Selection.activeObject = theShader;
                                        EditorGUIUtility.PingObject(theShader);
                                    }
                                    GUI.enabled = true;
                                    if (GUILayout.Button(new GUIContent("Filter", "Show shaders using this keyword."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                        keywordFilter = keywordView[s].keyword;
                                        sortType = SortType.VariantsCount;
                                        EditorGUIUtility.ExitGUI();
                                        return;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                } else {

                    for (int s = 0; s < shaderCount; s++) {
                        SCShader shader = shaders[s];
                        if (shader.keywordEnabledCount < minimumKeywordCount)
                            continue;
                        if (!string.IsNullOrEmpty(keywordFilter)) {
                            int kwCount = shader.keywords.Count;
                            bool found = false;
                            for (int w = 0; w < kwCount; w++) {
                                if (shader.keywords[w].name.IndexOf(keywordFilter, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                continue;
                        }
                        if (!string.IsNullOrEmpty(projectShaderNameFilter) && shader.name.IndexOf(projectShaderNameFilter, StringComparison.CurrentCultureIgnoreCase)<0) {
                           continue;
                        }
                        EditorGUILayout.BeginHorizontal();
                        string shaderName = shader.isReadOnly ? shader.name + " (readonly)" : shader.name;
                        if (shader.hasSource) {
                            shader.foldout = EditorGUILayout.Foldout(shader.foldout, new GUIContent(shaderName + " (" + shader.keywords.Count + " keywords, " + shader.keywordEnabledCount + " used, " + shader.actualBuildVariantCount + " variants)"), shader.editedByShaderControl ? foldoutBold : foldoutNormal);
                        } else {
                            shader.foldout = EditorGUILayout.Foldout(shader.foldout, new GUIContent(shaderName + " (" + shader.keywordEnabledCount + " keywords used by materials)"), foldoutDim);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (shader.foldout) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(15));
                            if (shader.hasSource) {
                                if (GUILayout.Button(new GUIContent("Locate", "Locate the shader in the project panel."), EditorStyles.miniButton)) {
                                    Shader theShader = AssetDatabase.LoadAssetAtPath<Shader>(shader.path);
                                    Selection.activeObject = theShader;
                                    EditorGUIUtility.PingObject(theShader);
                                }
                                if (GUILayout.Button(new GUIContent("Open", "Open the shader with the system default editor."), EditorStyles.miniButton)) {
                                    EditorUtility.OpenWithDefaultApp(shader.path);
                                }
                                if (!shader.pendingChanges)
                                    GUI.enabled = false;
                                if (GUILayout.Button(new GUIContent("Save", "Saves any keyword change to the shader file (disable/enable the keywords just clicking on the toggle next to the keywords shown below). A backup is created in the same folder."), EditorStyles.miniButton)) {
                                    UpdateShader(shader);
                                }
                                GUI.enabled = true;
                                if (!shader.hasBackup)
                                    GUI.enabled = false;
                                if (GUILayout.Button(new GUIContent("Restore", "Restores shader from the backup copy."), EditorStyles.miniButton)) {
                                    RestoreShader(shader);
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                                GUI.enabled = true;
                            } else {
                                EditorGUILayout.LabelField("(Shader source not available)");
                            }
                            if (shader.materials.Count > 0) {
                                if (GUILayout.Button(new GUIContent(shader.showMaterials ? "Hide Materials" : "List Materials", "Show or hide the materials that use these keywords."), EditorStyles.miniButton)) {
                                    shader.showMaterials = !shader.showMaterials;
                                    GUIUtility.ExitGUI();
                                    return;
                                }
                                if (shader.showMaterials && GUILayout.Button("Select All", EditorStyles.miniButton)) {
                                    int matCount = shader.materials.Count;
                                    List<Material> allMaterials = new List<Material>();
                                    for (int m = 0; m < matCount; m++) {
                                        SCMaterial material = shader.materials[m];
                                        Material theMaterial = AssetDatabase.LoadAssetAtPath<Material>(material.path) as Material;
                                        allMaterials.Add(theMaterial);
                                    }
                                    if (allMaterials.Count > 0) {
                                        Selection.objects = allMaterials.ToArray();
                                        EditorGUIUtility.PingObject(allMaterials[0]);
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();


                            for (int k = 0; k < shader.keywords.Count; k++) {
                                SCKeyword keyword = shader.keywords[k];
                                if (keyword.isUnderscoreKeyword)
                                    continue;
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("", GUILayout.Width(15));
                                if (shader.hasSource) {
                                    bool prevState = keyword.enabled;
                                    keyword.enabled = EditorGUILayout.Toggle(prevState, GUILayout.Width(18));
                                    if (prevState != keyword.enabled) {
                                        shader.pendingChanges = true;
                                        shader.UpdateVariantCount();
                                        UpdateProjectStats();
                                        GUIUtility.ExitGUI();
                                        return;
                                    }
                                } else {
                                    EditorGUILayout.Toggle(true, GUILayout.Width(18));
                                }
                                if (!keyword.enabled) {
                                    EditorGUILayout.LabelField(keyword.name, disabledStyle);
                                } else {
                                    EditorGUILayout.LabelField(keyword.name);
                                    if (!shader.hasSource && GUILayout.Button(new GUIContent("Prune Keyword", "Removes the keyword from all materials that reference it."), EditorStyles.miniButton, GUILayout.Width(110))) {
                                        if (EditorUtility.DisplayDialog("Prune Keyword", "This option will disable the keyword " + keyword.name + " in all materials that use " + shader.name + " shader.\nDo you want to continue?", "Ok", "Cancel")) {
                                            PruneMaterials(shader, keyword.name);
                                            UpdateProjectStats();
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                                if (shader.showMaterials) {
                                    // show materials using this shader
                                    int matCount = shader.materials.Count;
                                    for (int m = 0; m < matCount; m++) {
                                        SCMaterial material = shader.materials[m];
                                        if (material.ContainsKeyword(keyword.name)) {
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("", GUILayout.Width(30));
                                            EditorGUILayout.LabelField(matIcon, GUILayout.Width(18));
                                            EditorGUILayout.LabelField(material.name);
                                            if (GUILayout.Button(new GUIContent("Locate", "Locates the material in the project panel."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                                Material theMaterial = AssetDatabase.LoadAssetAtPath<Material>(material.path) as Material;
                                                Selection.activeObject = theMaterial;
                                                EditorGUIUtility.PingObject(theMaterial);
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                }
                            }

                            if (shader.showMaterials) {
                                // show materials using this shader that does not use any keywords
                                bool first = true;
                                int matCount = shader.materials.Count;
                                for (int m = 0; m < matCount; m++) {
                                    SCMaterial material = shader.materials[m];
                                    if (material.keywords.Count == 0) {
                                        if (first) {
                                            first = false;
                                            EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField("", GUILayout.Width(15));
                                            EditorGUILayout.LabelField("Materials using this shader and no keywords:");
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("", GUILayout.Width(15));
                                        EditorGUILayout.LabelField(matIcon, GUILayout.Width(18));
                                        EditorGUILayout.LabelField(material.name);
                                        if (GUILayout.Button(new GUIContent("Locate", "Locates the material in the project panel."), EditorStyles.miniButton, GUILayout.Width(60))) {
                                            Material theMaterial = AssetDatabase.LoadAssetAtPath<Material>(material.path) as Material;
                                            Selection.activeObject = theMaterial;
                                            EditorGUIUtility.PingObject(theMaterial);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                            }
                        }
                        EditorGUILayout.Separator();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        
    }

}