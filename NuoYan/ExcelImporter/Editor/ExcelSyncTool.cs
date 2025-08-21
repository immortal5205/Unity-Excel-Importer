using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

public static class ExcelSyncTool
{
    // 将SO数据同步回Excel
    public static bool SyncSOToExcel(ReadExcelDataBaseSO so)
    {
        if (so == null)
        {
            Debug.LogError("SO为空，无法同步");
            return false;
        }

        string excelPath = so.excelFilePath;
        if (string.IsNullOrEmpty(excelPath) || !File.Exists(excelPath))
        {
            Debug.LogError($"Excel文件不存在：{excelPath}");
            return false;
        }

        try
        {
            // 1. 打开Excel文件
            IWorkbook workbook = OpenExcel(excelPath);
            if (workbook == null)
            {
                Debug.LogError("无法打开Excel文件");
                return false;
            }

            // 2. 反射获取SO中的所有工作表数据（假设SO字段为List<T>，字段名=工作表名）
            Type soType = so.GetType();
            FieldInfo[] soFields = soType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in soFields)
            {
                // 只处理List<T>类型的字段（对应Excel的工作表）
                if (!field.FieldType.IsGenericType || field.FieldType.GetGenericTypeDefinition() != typeof(List<>))
                    continue;

                string sheetName = field.Name; // 字段名=工作表名
                ISheet sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    Debug.LogWarning($"Excel中不存在工作表：{sheetName}，跳过");
                    continue;
                }

                // 3. 获取SO中该字段的列表数据
                var dataList = field.GetValue(so) as System.Collections.IEnumerable;
                if (dataList == null)
                {
                    Debug.LogWarning($"SO中工作表数据为空：{sheetName}");
                    continue;
                }

                // 4. 清空Excel原有数据（保留表头第0行）
                ClearSheetData(sheet);

                // 5. 将SO数据写入Excel（从第1行开始）
                WriteDataToSheet(sheet, dataList, field.FieldType.GetGenericArguments()[0]);
            }

            // 6. 保存Excel文件（先备份原文件）
            BackupExcel(excelPath);
            SaveExcel(workbook, excelPath);

            Debug.Log($"成功同步SO数据到Excel：{excelPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"同步失败：{e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    // 打开Excel文件
    private static IWorkbook OpenExcel(string path)
    {
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
        {
            if (Path.GetExtension(path).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                return new HSSFWorkbook(stream);
            else if (Path.GetExtension(path).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return new XSSFWorkbook(stream);
            else
                return null;
        }
    }

    // 清空工作表数据（保留表头第0行）
    private static void ClearSheetData(ISheet sheet)
    {
        int rowCount = sheet.LastRowNum;
        // 从第1行开始删除（保留表头）
        for (int i = rowCount; i >= 1; i--)
        {
            IRow row = sheet.GetRow(i);
            if (row != null)
                sheet.RemoveRow(row);
        }
    }

    // 将SO列表数据写入工作表
    private static void WriteDataToSheet(ISheet sheet, System.Collections.IEnumerable dataList, Type entityType)
    {
        IRow headerRow = sheet.GetRow(0); // 表头行（第0行）
        if (headerRow == null)
        {
            Debug.LogError("工作表无表头，无法写入数据");
            return;
        }

        // 获取表头列名（用于匹配实体字段）
        Dictionary<int, string> columnNameMap = new Dictionary<int, string>();
        for (int i = 0; i < headerRow.LastCellNum; i++)
        {
            ICell cell = headerRow.GetCell(i);
            if (cell != null && !string.IsNullOrEmpty(cell.StringCellValue))
                columnNameMap[i] = cell.StringCellValue;
        }

        // 获取实体字段（用于反射取值）
        FieldInfo[] entityFields = entityType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        int rowIndex = 1; // 数据从第1行开始
        foreach (var entity in dataList)
        {
            IRow dataRow = sheet.CreateRow(rowIndex); // 创建行

            // 遍历列，写入数据
            foreach (var (colIndex, columnName) in columnNameMap)
            {
                // 找到实体中与列名匹配的字段
                FieldInfo field = entityFields.FirstOrDefault(f => f.Name == columnName);
                if (field == null)
                    continue;

                // 获取字段值
                object value = field.GetValue(entity);
                if (value == null)
                    continue;

                // 创建单元格并赋值（根据值类型设置单元格类型）
                ICell cell = dataRow.CreateCell(colIndex);
                SetCellValue(cell, value);
            }

            rowIndex++;
        }
    }

    // 设置单元格值（处理不同数据类型）
    private static void SetCellValue(ICell cell, object value)
    {
        if (value is string str)
        {
            cell.SetCellValue(str);
        }
        else if (value is int intVal)
        {
            cell.SetCellValue(intVal);
        }
        else if (value is float floatVal)
        {
            cell.SetCellValue(floatVal);
        }
        else if (value is double doubleVal)
        {
            cell.SetCellValue(doubleVal);
        }
        else if (value is bool boolVal)
        {
            cell.SetCellValue(boolVal);
        }
        else if (value is Enum enumVal)
        {
            cell.SetCellValue(enumVal.ToString());
        }
        else if (value is UnityEngine.Object obj)
        {
            // 资源类型存名称（如Sprite/Texture存名字）
            cell.SetCellValue(obj.name);
        }
        else
        {
            // 其他类型转字符串
            cell.SetCellValue(value.ToString());
        }
    }

    private static void SaveExcel(IWorkbook workbook, string path)
    {
        using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(stream);
        }
        AssetDatabase.Refresh();
    }

    private static void BackupExcel(string path)
    {
        string backupPath = $"{path}.bak";
        if (File.Exists(backupPath))
            File.Delete(backupPath);
        File.Copy(path, backupPath);
    }
}