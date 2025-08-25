using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReadExcelDataBaseSO), true)]
public class ReadExcelDataBaseEditor : Editor
{
    private ExcelImportSetting[] excelImportSettings;
    private string soPath;
    private string settingPath;

    private void OnEnable()
    {
        excelImportSettings = FindAllExcelImportSettings();

        // if (excelImportSettings == null || excelImportSettings.Length == 0)
        // {
        //     Debug.LogWarning("未找到任何ExcelImportSetting类型的资源");
        // }
        // else
        // {
        //     Debug.Log($"找到 {excelImportSettings.Length} 个ExcelImportSetting资源");
        // }
    }

    public static ExcelImportSetting[] FindAllExcelImportSettings()
    {
        List<ExcelImportSetting> results = new List<ExcelImportSetting>();
        
        string[] guids = AssetDatabase.FindAssets($"t:{nameof(ExcelImportSetting)}");
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            ExcelImportSetting setting = AssetDatabase.LoadAssetAtPath<ExcelImportSetting>(assetPath);
            
            if (setting != null)
            {
                results.Add(setting);
            }
        }
        
        return results.ToArray();
    }

    public override void OnInspectorGUI()
    {
        ReadExcelDataBaseSO excelDataBaseSO = (ReadExcelDataBaseSO)target;
        soPath = AssetDatabase.GetAssetPath(excelDataBaseSO);

        if (excelImportSettings != null && excelImportSettings.Length > 0)
        {
            foreach (var setting in excelImportSettings)
            {

                settingPath = AssetDatabase.GetAssetPath(setting);
                if (setting.outputSOAssetsPath == soPath.Replace($"/{excelDataBaseSO.GetType().Name}.asset", ""))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Settings");
                        EditorGUILayout.ObjectField(setting, typeof(ExcelImportSetting), false);
                    }
                    EditorGUILayout.EndHorizontal();
                    break;
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("Settings", "未找到任何ExcelImportSetting资源");
        }

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("SO");
            EditorGUILayout.ObjectField(excelDataBaseSO, typeof(ReadExcelDataBaseSO), false);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Settings Path", settingPath);
        EditorGUILayout.LabelField("SO Path", soPath);

        if (excelDataBaseSO.hideFlags == HideFlags.None)
        {
            if (GUILayout.Button("同步到Excel", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                    "确认同步",
                    $"是否将SO数据同步回Excel？\n{excelDataBaseSO.excelFilePath}\n（会覆盖原文件，自动创建备份）",
                    "同步",
                    "取消"))
                {
                    bool success = ExcelSyncTool.SyncSOToExcel(excelDataBaseSO);
                    if (success)
                        EditorUtility.DisplayDialog("成功", "数据已同步到Excel", "确定");
                }
            }
        }
        base.OnInspectorGUI();

        
    }
}
