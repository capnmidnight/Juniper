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

namespace ShaderControl
{

    public partial class SCWindow : EditorWindow
    {

        ShadersBuildInfo shadersBuildInfo;
        int totalBuildKeywords, totalBuildIncludedKeywords, totalBuildShadersWithKeywords, totalBuildShaders, totalBuildIncludedShaders, totalBuildIncludedShadersWithKeywords;
        int maxBuildKeywordsCountFound = 0;
        bool nextQuickBuild;
        public static bool issueRefresh;

        void RefreshBuildStats(bool quick)
        {
            issueRefresh = false;
            nextQuickBuild = GetEditorPrefBool("QUICK_BUILD", false);
            ShaderDebugBuildProcessor.CheckShadersBuildStore();
            totalBuildKeywords = totalBuildIncludedKeywords = totalBuildShadersWithKeywords = totalBuildShaders = totalBuildIncludedShaders = totalBuildIncludedShadersWithKeywords = 0;
            shadersBuildInfo = Resources.Load<ShadersBuildInfo>("BuiltShaders");
            if (shadersBuildInfo == null || shadersBuildInfo.shaders == null) return;

            int count = shadersBuildInfo.shaders.Count;
            totalBuildShaders = 0;
            maxBuildKeywordsCountFound = 0;

            for (int k = 0; k < count; k++)
            {
                ShaderBuildInfo sb = shadersBuildInfo.shaders[k];
                int kwCount = sb.keywords != null ? sb.keywords.Count : 0;
                if (minimumKeywordCount > 0 && kwCount < minimumKeywordCount) continue;
                if (shadersBuildInfo.viewType == BuildViewShaderOption.ProjectShaders && sb.isInternal) continue;
                if (shadersBuildInfo.viewType == BuildViewShaderOption.UnityInternalShaders && !sb.isInternal) continue;
                if (!string.IsNullOrEmpty(keywordFilter) && !sb.ContainsKeyword(keywordFilter, false)) continue;
                if (!string.IsNullOrEmpty(buildShaderNameFilter) && sb.name.IndexOf(buildShaderNameFilter, StringComparison.CurrentCulture) < 0) continue;
                totalBuildShaders++;

                // Check shaders exist
                if (!quick && Shader.Find(sb.name) == null)
                {
                    shadersBuildInfo.shaders.RemoveAt(k);
                    k--;
                    totalBuildShaders--;
                    continue;
                }
                if (sb.includeInBuild)
                {
                    totalBuildIncludedShaders++;
                }
                if (kwCount > 0)
                {
                    if (kwCount > maxBuildKeywordsCountFound)
                    {
                        maxBuildKeywordsCountFound = kwCount;
                    }
                    totalBuildKeywords += kwCount;
                    totalBuildShadersWithKeywords++;
                    if (sb.includeInBuild)
                    {
                        totalBuildIncludedShadersWithKeywords++;
                        for (int j = 0; j < kwCount; j++)
                        {
                            if (sb.keywords[j].includeInBuild)
                            {
                                totalBuildIncludedKeywords++;
                            }
                        }
                    }
                }
            }
        }

        void ClearBuildData()
        {
            ShaderDebugBuildProcessor.CheckShadersBuildStore();
            if (shadersBuildInfo != null)
            {
                shadersBuildInfo.Clear();
            }
        }

        void BuildUpdateShaderKeywordsState(SCShader shader)
        {
            if (shader == null || shader.keywords == null) return;
            if (shadersBuildInfo == null) return;
            ShaderBuildInfo sb = shadersBuildInfo.GetShader(shader.fullName);
            if (sb == null) return;
            for (int k = 0; k < shader.keywords.Count; k++)
            {
                sb.ToggleIncludeKeyword(shader.keywords[k].name, shader.keywords[k].enabled);
            }
        }


    }

}