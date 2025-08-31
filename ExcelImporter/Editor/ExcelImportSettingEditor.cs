using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static ExcelImporter;

[CustomEditor(typeof(ExcelImportSetting))]
public class ExcelImportSettingEditor : Editor
{
    private static ExcelImportSetting importSetting;
    private static List<string> excelNames;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        importSetting = (ExcelImportSetting)target;
        if (importSetting.excelAssetsPath == null)
        {
            EditorGUILayout.HelpBox("Excel文件不可为空", MessageType.Warning);
            return;
        }
        if (importSetting.outpScriptPath == null)
        {
            EditorGUILayout.HelpBox("脚本输出路径不可为空", MessageType.Warning);
            return;
        }
        if (importSetting.outputSOAssetsPath == null)
        {
            EditorGUILayout.HelpBox("输出路径不可为空", MessageType.Warning);
            return;
        }
        excelNames = ExcelAssetScriptMenu.GetAllExcelFiles(importSetting.excelAssetsPath);

        //显示Excel文件列表
        EditorGUILayout.LabelField("Excel Files", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        {
            if (ExcelImporter.cachedInfos == null)
                ExcelImporter.cachedInfos = ExcelImporter.FindExcelAssetInfos();
            bool imported = false;
            foreach (string excel in excelNames)
            {
                var excelName = Path.GetFileNameWithoutExtension(excel) + "SO";
                if (excelName.StartsWith("~$")) continue;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(excel.Replace($"{importSetting.excelAssetsPath}\\", "").Replace(".xlsx", ""));

                DrawScriptField(excelName, importSetting.outputSOAssetsPath + "/" + excelName + ".asset");

                if (GUILayout.Button("创建脚本"))
                {
                    List<string> sheetNames = ExcelAssetScriptMenu.GetSheetNames(excel);

                    if (sheetNames.Count == 0)
                    {
                        Debug.LogWarning($"Excel文件 {excelName} 中没有找到工作表");
                        continue;
                    }

                    string scriptString = ExcelAssetScriptMenu.BuildScriptString(excelName, sheetNames);

                    string fullPath = Path.Combine(importSetting.outpScriptPath, $"{excelName}.cs");

                    if (File.Exists(fullPath))
                    {
                        Debug.LogWarning($"脚本 {fullPath} 已存在，跳过创建");
                        continue;
                    }

                    File.WriteAllText(fullPath, scriptString);
                    Debug.Log($"已创建脚本: {fullPath}");
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("生成SO"))
                {
                    string extension = Path.GetExtension(excel);
                    if (extension != ".xls" && extension != ".xlsx") continue;


                    if (excelName.StartsWith("~$")) continue;

                    ExcelAssetInfo info = ExcelImporter.cachedInfos.Find(i => i.ExcelName == excelName);
                    if (info == null)
                    {
                        Debug.LogWarning($"未找到与 {excelName} 对应的ExcelAsset类型，跳过");
                        continue;
                    }

                    ExcelImporter.ImportExcel(excel.Replace("\\", "/"), info, importSetting.outputSOAssetsPath);
                    imported = true;
                }
                if (GUILayout.Button("删除脚本"))
                {
                    var b = EditorUtility.DisplayDialog("警告", $"确定要删除{excelName}.cs文件吗？", "确定", "取消");
                    if (b)
                    {
                        var s = importSetting.outpScriptPath + "/" + excelName + ".cs";
                        AssetDatabase.DeleteAsset(s);
                    }
                }
                if (GUILayout.Button("删除SO"))
                {
                    var b = EditorUtility.DisplayDialog("警告", $"确定要删除{excelName}.asset文件吗？", "确定", "取消");
                    if (b)
                    {
                        var s = importSetting.outputSOAssetsPath + "/" + excelName + ".asset";
                        AssetDatabase.DeleteAsset(s);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal("box");
        {
            if (GUILayout.Button("创建所有脚本", GUILayout.Width(85)))
            {
                foreach (var excel in excelNames)
                {
                    var excelName = Path.GetFileNameWithoutExtension(excel) + "SO";
                    List<string> sheetNames = ExcelAssetScriptMenu.GetSheetNames(excel);

                    if (sheetNames.Count == 0)
                    {
                        Debug.LogWarning($"Excel文件 {excelName} 中没有找到工作表");
                        continue;
                    }

                    string scriptString = ExcelAssetScriptMenu.BuildScriptString(excelName, sheetNames);

                    string fullPath = Path.Combine(importSetting.outpScriptPath, $"{excelName}.cs");

                    if (File.Exists(fullPath))
                    {
                        Debug.LogWarning($"脚本 {fullPath} 已存在，跳过创建");
                        continue;
                    }

                    File.WriteAllText(fullPath, scriptString);
                    Debug.Log($"已创建脚本: {fullPath}");
                    AssetDatabase.Refresh();
                }
            }
            if (GUILayout.Button("生成所有SO", GUILayout.Width(85)))
            {
                foreach (string excel in excelNames)
                {
                    var excelName = Path.GetFileNameWithoutExtension(excel) + "SO";

                    string extension = Path.GetExtension(excel);
                    if (extension != ".xls" && extension != ".xlsx") continue;

                    if (excelName.StartsWith("~$")) continue;

                    ExcelAssetInfo info = ExcelImporter.cachedInfos.Find(i => i.ExcelName == excelName);
                    if (info == null)
                    {
                        Debug.LogWarning($"未找到与 {excelName} 对应的ExcelAsset类型，跳过");
                        continue;
                    }

                    ExcelImporter.ImportExcel(excel.Replace("\\", "/"), info, importSetting.outputSOAssetsPath);
                }
            }
            if (GUILayout.Button("删除所有脚本", GUILayout.Width(85)))
            {
                var b = EditorUtility.DisplayDialog("警告", $"确定要删除所有脚本文件吗？", "确定", "取消");
                if (b)
                {
                    foreach (string excel in excelNames)
                    {
                        var excelName = Path.GetFileNameWithoutExtension(excel) + "SO";
                        AssetDatabase.DeleteAsset(importSetting.outpScriptPath + "/" + excelName + ".cs");
                    }
                }
            }
            if (GUILayout.Button("删除所有SO", GUILayout.Width(85)))
            {
                var b = EditorUtility.DisplayDialog("警告", $"确定要删除所有SO文件吗？", "确定", "取消");
                if (b)
                {
                    foreach (string excel in excelNames)
                    {
                        var excelName = Path.GetFileNameWithoutExtension(excel) + "SO";
                        AssetDatabase.DeleteAsset(importSetting.outputSOAssetsPath + "/" + excelName + ".asset");
                    }
                }
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Reveal", GUILayout.Width(70)))
            {
                EditorUtility.RevealInFinder(importSetting.excelAssetsPath + "/");
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void DrawScriptField(string ScriptName, string objPath)
    {
        GUI.enabled = false;
        var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(objPath);
        EditorGUILayout.ObjectField(so, Type.GetType(ScriptName), false);
        EditorGUILayout.ObjectField(MonoScript.FromScriptableObject(so), Type.GetType(ScriptName), false);
        GUI.enabled = true;
    }

    public static void RuntimeReadExcel()
    {
        bool imported = false;
        ExcelImportSetting[] importSettings = ReadExcelDataBaseEditor.FindAllExcelImportSettings();
        if (importSettings == null || importSettings.Length == 0)
        {
            Debug.LogError("未找到任何ExcelImportSetting配置文件");
            return;
        }

        // 获取缓存实例
        var cache = ExcelImportCache.Instance;

        foreach (var importSetting in importSettings)
        {
            if (string.IsNullOrEmpty(importSetting.excelAssetsPath)) continue;

            List<string> excelPaths = ExcelAssetScriptMenu.GetAllExcelFiles(importSetting.excelAssetsPath);
            if (excelPaths == null || excelPaths.Count == 0) continue;

            if (cachedInfos == null)
                cachedInfos = FindExcelAssetInfos();

            foreach (string excelPath in excelPaths)
            {
                string excelName = Path.GetFileNameWithoutExtension(excelPath);
                if (excelName.StartsWith("~$")) continue; // 跳过临时文件

                if (!cache.NeedImport(excelPath))
                {
                    // Debug.Log($"Excel文件未修改，跳过导入：{excelPath}");
                    continue;
                }

                string realExcelName = excelName + "SO";
                ExcelAssetInfo info = cachedInfos.Find(i => i.ExcelName == realExcelName);
                if (info == null)
                {
                    Debug.LogWarning($"未找到与 {realExcelName} 对应的ExcelAsset类型，跳过");
                    continue;
                }

                ImportExcel(excelPath.Replace("\\", "/"), info, importSetting.outputSOAssetsPath);
                imported = true;

                cache.UpdateCache(excelPath);
                DateTime currentTime = File.GetLastWriteTimeUtc(excelPath);
               // Debug.Log($"更新Excel修改时间：{excelPath} → {currentTime}".ColorString(Color.green));
            }
        }

        if (imported)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}