using System.Collections.Generic;
using NuoYan.Extension;
using UnityEngine;

[CreateAssetMenu(menuName = "NuoYan/Excel/ExcelImportSetting")]
public class ExcelImportSetting : ScriptableObject
{
    [Tooltip("工程目录下存放Excel的路径")]
    [EnumArray(new[] { "ExcelAssets", "Assets/GameMain/DataTables/Tables" })]
    public string excelAssetsPath = "";
    [EnumArray(new[] { "Assets/GameMain/Scripts/DataTable/SOScripts" })]
    public string outpScriptPath = "";
    [Tooltip("Assets目录下的输出路径")]
    [EnumArray(new[] { "Assets/GameMain/DataTables/Tables", "Assets/GameMain/DataTables/ExcelData" })]
    public string outputSOAssetsPath = "";
}