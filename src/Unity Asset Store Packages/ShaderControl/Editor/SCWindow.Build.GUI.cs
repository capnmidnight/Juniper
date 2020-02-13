/// <summary>
/// Shader Control - (C) Copyright 2016-2019 Ramiro Oliva (Kronnect)
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

    public enum BuildVIewSortType {
        ShaderName = 0,
        Keyword = 1
    }

    public enum BuildViewShaderOption {
        AllShaders = 0,
        ProjectShaders = 1,
        UnityInternalShaders = 2
    }
    public partial class SCWindow : EditorWindow {

        Vector2 scrollViewPosBuild;
        string buildShaderNameFilter;
        GUIContent[] viewShaderTexts = { new GUIContent("All Shaders", "Show all shaders included in the build"), new GUIContent("Project Shaders", "Show shaders with source code available"), new GUIContent("Hidden Shaders", "Show Unity internal shaders included in the build") };

        void DrawBuildGUI() {

            GUILayout.Box(new GUIContent("This tab shows all shaders compiled in your last build.\nHere you can exclude any number of shaders or keywords from future compilations. No file is modified, only excluded from the build.\nIf you have exceeded the maximum allowed keywords in your project, use the <color=orange><b>Project View</b></color> tab to remove shaders or disable any unwanted keyword from the project."), titleStyle);
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Quick Build", "Forces a quick compilation to extract all shaders and keywords included in the build."))) {
                EditorUtility.DisplayDialog("Ready for the Build!", "Now make a build as normal (select 'File -> Build Settings -> Build').\n\nShader Control will detect the shaders and keywords from the build process and list that information here.\n\nNote: in order to make this special build faster, shaders won't be compiled in this build.", "Ok");
                SetEditorPrefBool("QUICK_BUILD", true);
                nextQuickBuild = true;
                ClearBuildData();
            }
            if (GUILayout.Button("Help", GUILayout.Width(40))) {
                ShowHelpWindow();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (nextQuickBuild) {
                EditorGUILayout.HelpBox("Shader Control is ready to collect data during the next build.", MessageType.Info);
            }

            int shadersCount = shadersBuildInfo == null || shadersBuildInfo.shaders == null ? 0 : shadersBuildInfo.shaders.Count;

            if (!nextQuickBuild) {
                EditorGUILayout.LabelField("Last build: " + ((shadersBuildInfo.creationDateTicks != 0) ? shadersBuildInfo.creationDateString : "no data yet. Click 'Quick Build' for more details."), EditorStyles.boldLabel);
            }

            if (shadersCount > 0) {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("View", GUILayout.Width(90));
                EditorGUI.BeginChangeCheck();

                shadersBuildInfo.viewType = (BuildViewShaderOption)GUILayout.SelectionGrid((int)shadersBuildInfo.viewType, viewShaderTexts, 3);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(shadersBuildInfo);
                    RefreshBuildStats(true);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sort By", GUILayout.Width(90));
                EditorGUI.BeginChangeCheck();
                shadersBuildInfo.sortType = (BuildVIewSortType)EditorGUILayout.EnumPopup(shadersBuildInfo.sortType);
                if (EditorGUI.EndChangeCheck()) {
                    if (shadersBuildInfo != null) {
                        shadersBuildInfo.Resort();
                    }
                    EditorUtility.SetDirty(shadersBuildInfo);
                    EditorGUIUtility.ExitGUI();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name Filter", GUILayout.Width(90));
                EditorGUI.BeginChangeCheck();
                buildShaderNameFilter = EditorGUILayout.TextField(buildShaderNameFilter);
                if (GUILayout.Button(new GUIContent("Clear", "Clear filter."), EditorStyles.miniButton, GUILayout.Width(60))) {
                    buildShaderNameFilter = "";
                    GUIUtility.keyboardControl = 0;
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck()) {
                    RefreshBuildStats(true);
                }


                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Keywords >=", GUILayout.Width(90));
                minimumKeywordCount = EditorGUILayout.IntSlider(minimumKeywordCount, 0, maxBuildKeywordsCountFound);
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Keyword Filter", GUILayout.Width(90));
                keywordFilter = EditorGUILayout.TextField(keywordFilter);
                if (GUILayout.Button(new GUIContent("Clear", "Clear filter."), EditorStyles.miniButton, GUILayout.Width(60))) {
                    keywordFilter = "";
                    GUIUtility.keyboardControl = 0;
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck()) {
                    RefreshBuildStats(true);
                }

                EditorGUILayout.Separator();

                if (totalBuildShaders == 0 || totalBuildIncludedShaders == 0 || totalBuildKeywords == 0 || (totalBuildKeywords == totalBuildIncludedKeywords && totalBuildShaders == totalBuildIncludedShaders))
                {
                    EditorGUILayout.HelpBox("Total Shaders: " + totalBuildShaders + "  Shaders Using Keywords: " + totalBuildShadersWithKeywords + "\nTotal Keywords: " + totalBuildKeywords, MessageType.Info);
                }
                else
                {
                    int shadersPerc = totalBuildIncludedShaders * 100 / totalBuildShaders;
                    int shadersWithKeywordsPerc = totalBuildIncludedShadersWithKeywords * 100 / totalBuildIncludedShaders;
                    int keywordsPerc = totalBuildIncludedKeywords * 100 / totalBuildKeywords;
                    EditorGUILayout.HelpBox("Total Shaders: " + totalBuildIncludedShaders + " of " + totalBuildShaders + " (" + shadersPerc + "%" + "  Shaders Using Keywords: " + totalBuildIncludedShadersWithKeywords + " of " + totalBuildShadersWithKeywords + " (" + shadersWithKeywordsPerc + "%)\nTotal Keywords: " + totalBuildIncludedKeywords + " of " + totalBuildKeywords + " (" + keywordsPerc.ToString() + "%)", MessageType.Info);
                }

                EditorGUILayout.Separator();

                scrollViewPosProject = EditorGUILayout.BeginScrollView(scrollViewPosProject);

                bool requireUpdate = false;
                for (int k = 0; k < shadersCount; k++) {
                    ShaderBuildInfo sb = shadersBuildInfo.shaders[k];
                    int kwCount = sb.keywords == null ? 0 : sb.keywords.Count;
                    if (kwCount < minimumKeywordCount) continue;
                    if (shadersBuildInfo.viewType == BuildViewShaderOption.ProjectShaders && sb.isInternal) continue;
                    if (shadersBuildInfo.viewType == BuildViewShaderOption.UnityInternalShaders && !sb.isInternal) continue;
                    if (!string.IsNullOrEmpty(keywordFilter) && !sb.ContainsKeyword(keywordFilter, false))
                        continue;
                    if (!string.IsNullOrEmpty(buildShaderNameFilter) && sb.name.IndexOf(buildShaderNameFilter, StringComparison.CurrentCulture)<0) continue;

                    GUI.enabled = sb.includeInBuild;
                    EditorGUILayout.BeginHorizontal();
                    string shaderName = (sb.isInternal && shadersBuildInfo.viewType != BuildViewShaderOption.UnityInternalShaders) ? sb.name + " (internal)" : sb.name;
                    sb.isExpanded = EditorGUILayout.Foldout(sb.isExpanded, shaderName + " (" + kwCount + " keyword" + (kwCount != 1 ? "s)" : ")"), sb.isInternal ? foldoutDim : foldoutNormal);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = true;
                    if (sb.name != "Standard") {
                        EditorGUI.BeginChangeCheck();
                        sb.includeInBuild = EditorGUILayout.ToggleLeft("Include", sb.includeInBuild, GUILayout.Width(90));
                        if (EditorGUI.EndChangeCheck()) {
                            requireUpdate = true;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (sb.isExpanded) {
                        GUI.enabled = sb.includeInBuild;
                        EditorGUI.indentLevel++;
                        if (kwCount == 0) {
                            EditorGUILayout.LabelField("No keywords.");
                        } else {
                            if (!sb.isInternal)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("", GUILayout.Width(15));
                                if (GUILayout.Button("Locate", EditorStyles.miniButton, GUILayout.Width(80)))
                                {
                                    PingShader(sb.name);
                                }
                                if (!sb.isInternal && GUILayout.Button("Show In Project View", EditorStyles.miniButton, GUILayout.Width(120)))
                                {
                                    projectShaderNameFilter = sb.simpleName;
                                    scanAllShaders = true;
                                    PingShader(sb.name);
                                    if (shaders == null) ScanProject();
                                    viewMode = ViewMode.Project;
                                    EditorGUIUtility.ExitGUI();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            for (int j = 0; j < kwCount; j++) {
                                KeywordBuildSettings kw = sb.keywords[j];
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(kw.keyword);
                                GUILayout.FlexibleSpace();
                                EditorGUI.BeginChangeCheck();
                                kw.includeInBuild = EditorGUILayout.ToggleLeft("Include", kw.includeInBuild, GUILayout.Width(90));
                                if (EditorGUI.EndChangeCheck()) {
                                    requireUpdate = true;
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndScrollView();

                if (requireUpdate)
                {
                    RefreshBuildStats(true);
                    EditorUtility.SetDirty(shadersBuildInfo);
                    AssetDatabase.SaveAssets();
                }

                // detect changes
                if (issueRefresh)
                {
                    shadersBuildInfo.Refresh();
                    RefreshBuildStats(false);
                }
            }

        }


    }

}