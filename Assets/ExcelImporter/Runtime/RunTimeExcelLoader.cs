namespace ExcelRuntimeTools
{

    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.IO;
    using System.Reflection;
    using NPOI.SS.UserModel;
    using System.Linq;

    public class ExcelLoader 
    {
        class ExcelAssetInfo
        {
            public Type AssetType { get; set; }
            public ExcelAssetAttribute Attribute { get; set; }
            public string ExcelName
            {
                get
                {
                    return string.IsNullOrEmpty(Attribute.ExcelName) ? AssetType.Name : Attribute.ExcelName;
                }
            }
        }

        static List<string> GetFieldNamesFromSheetHeader(ISheet sheet)
        {
            IRow headerRow = sheet.GetRow(0);

            var fieldNames = new List<string>();
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                var cell = headerRow.GetCell(i);
                if (cell == null || cell.CellType == CellType.Blank) break;
                fieldNames.Add(cell.StringCellValue);
            }
            return fieldNames;
        }

        static object CellToFieldObject(ICell cell, FieldInfo fieldInfo, bool isFormulaEvalute = false)
        {
            var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;

            switch (type)
            {
                case CellType.String:
                    if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
                    else return cell.StringCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
                case CellType.Formula:
                    if (isFormulaEvalute) return null;
                    return CellToFieldObject(cell, fieldInfo, true);
                default:
                    if (fieldInfo.FieldType.IsValueType)
                    {
                        return Activator.CreateInstance(fieldInfo.FieldType);
                    }
                    return null;
            }
        }

        static object CreateEntityFromRow(IRow row, List<string> columnNames, Type entityType, string sheetName)
        {
            var entity = Activator.CreateInstance(entityType);

            var fields = entityType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < columnNames.Count; i++)
            {
                FieldInfo entityField = fields.FirstOrDefault(f => f.Name == columnNames[i]);
                if (entityField == null) continue;
                if (!entityField.IsPublic && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

                ICell cell = row.GetCell(i);
                if (cell == null) continue;

                try
                {
                    object fieldValue = CellToFieldObject(cell, entityField);
                    entityField.SetValue(entity, fieldValue);
                }
                catch
                {
                    throw new Exception(string.Format("Invalid excel cell type at row {0}, column {1}, {2} sheet.", row.RowNum, cell.ColumnIndex, sheetName));
                }
            }
            return entity;
        }

        static object GetEntityListFromSheet(ISheet sheet, Type entityType)
        {
            List<string> excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

            Type listType = typeof(List<>).MakeGenericType(entityType);
            MethodInfo listAddMethod = listType.GetMethod("Add", new Type[] { entityType });
            object list = Activator.CreateInstance(listType);

            // row of index 0 is header
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) break;

                ICell entryCell = row.GetCell(0);
                if (entryCell == null || entryCell.CellType == CellType.Blank) break;

                // skip comment row
                if (entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

                var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);
                listAddMethod.Invoke(list, new object[] { entity });
            }
            return list;
        }

        static List<EntityType> GetEntityListFromSheet<EntityType>(ISheet sheet) where EntityType : class
        {
            List<string> excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

            var list = new List<EntityType>();

            var entityType = typeof(EntityType);

            // row of index 0 is header
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) break;

                ICell entryCell = row.GetCell(0);
                if (entryCell == null || entryCell.CellType == CellType.Blank) break;

                // skip comment row
                if (entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

                var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);

                list.Add((EntityType)entity);
            }
            return list;
        }

        static void SheetToList<EntityType>(List<EntityType> list, ISheet sheet)
        {
            List<string> excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

            var entityType = typeof(EntityType);

            // row of index 0 is header
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) break;

                ICell entryCell = row.GetCell(0);
                if (entryCell == null || entryCell.CellType == CellType.Blank) break;

                // skip comment row
                if (entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

                var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);

                list.Add((EntityType)entity);
            }
        }

        static void SheetToDictionary<EntityType, Key>(Dictionary<Key, EntityType> dict, ISheet sheet, string keyFieldName = null)
        {
            List<string> excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

            var entityType = typeof(EntityType);

            if (keyFieldName == null)
            {
                keyFieldName = sheet.GetRow(0).GetCell(0).StringCellValue;
            }

            // row of index 0 is header
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) break;

                ICell entryCell = row.GetCell(0);
                if (entryCell == null || entryCell.CellType == CellType.Blank) break;

                // skip comment row
                if (entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

                var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);

                
                var keyField = entityType.GetField(keyFieldName);
                var key = (Key)keyField.GetValue(entity);

                dict.Add(key, (EntityType)entity);
            }
        }

        public static void LoadToDictionary<EntityType, Key>(Dictionary<Key, EntityType> dict, Stream stream, string sheetName = null, string keyFieldName = null)
        {
            IWorkbook book = WorkbookFactory.Create(stream);
            if (sheetName == null)
            {
                sheetName = book.GetSheetName(0);
            }
            ISheet sheet = book.GetSheet(sheetName);
            if (sheet == null) return;
            SheetToDictionary(dict, sheet, keyFieldName);
        }

        public static void LoadToDictionary<EntityType, Key>(Dictionary<Key, EntityType> dict, byte[] bytes, string sheetName = null, string keyFieldName = null)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                LoadToDictionary(dict, stream, sheetName, keyFieldName);
            }
        }

        public static void LoadToDictionary<EntityType, Key>(Dictionary<Key, EntityType> dict, string excelPath, string sheetName = null, string keyFieldName = null)
        {
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                LoadToDictionary(dict, stream, sheetName, keyFieldName);
            }
        }

        public static bool LoadToList<T>(List<T> values, Stream stream, string sheetName = null) where T : class
        {
            IWorkbook book = WorkbookFactory.Create(stream);
            if (sheetName == null)
            {
                sheetName = book.GetSheetName(0);
            }
            ISheet sheet = book.GetSheet(sheetName);
            if (sheet == null) return false;
            SheetToList(values, sheet);
            return true;
        }

        public static bool LoadToList<T>(List<T> values, byte[] bytes, string sheetName = null) where T : class
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return LoadToList(values, stream, sheetName);
            }
        }

        public static bool LoadToList<T>(List<T> values, string excelPath, string sheetName = null) where T : class
        {
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return LoadToList(values, stream, sheetName);
            }
        }

        //从bytes读取
        public static T LoadExcel<T>(byte[] bytes) where T : class
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return LoadExcel<T>(stream);
            }
        }

        //从文件读取
        public static T LoadExcel<T>(string excelPath) where T : class
        {
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return LoadExcel<T>(stream);
            }
        }

        

        public static T LoadExcel<T, EntityType>(Stream stream) where T : class where EntityType : class
        {
            T asset = Activator.CreateInstance<T>();
            IWorkbook book = WorkbookFactory.Create(stream);
            var targetType = typeof(T);
            var attribute = targetType.GetCustomAttribute<ExcelAssetAttribute>();
            var info = new ExcelAssetInfo()
            {
                AssetType = targetType,
                Attribute = attribute
            };
            var assetFields = info.AssetType.GetFields();
            int sheetCount = 0;

            foreach (var assetField in assetFields)
            {
                ISheet sheet = book.GetSheet(assetField.Name);
                if (sheet == null) continue;

                Type fieldType = assetField.FieldType;
                if (!fieldType.IsGenericType || (fieldType.GetGenericTypeDefinition() != typeof(List<>))) continue;

                Type[] types = fieldType.GetGenericArguments();
                Type entityType = types[0];

                var entities = GetEntityListFromSheet<EntityType>(sheet);
                assetField.SetValue(asset, entities);
                sheetCount++;
            }

            if (info.Attribute.LogOnImport)
            {
                Debug.Log(string.Format("Imported {0} sheets.", sheetCount));
            }

            return asset;
        }

        public static T LoadExcel<T>(Stream stream) where T:class
        {
            T asset = Activator.CreateInstance<T>();
            IWorkbook book = WorkbookFactory.Create(stream);
            var targetType = typeof(T);
            var attribute = targetType.GetCustomAttribute<ExcelAssetAttribute>();
            var info = new ExcelAssetInfo()
            {
                AssetType = targetType,
                Attribute = attribute
            };
            var assetFields = info.AssetType.GetFields();
            int sheetCount = 0;

            foreach (var assetField in assetFields)
            {
                ISheet sheet = book.GetSheet(assetField.Name);
                if (sheet == null) continue;

                Type fieldType = assetField.FieldType;
                if (!fieldType.IsGenericType || (fieldType.GetGenericTypeDefinition() != typeof(List<>))) continue;

                Type[] types = fieldType.GetGenericArguments();
                Type entityType = types[0];

                object entities = GetEntityListFromSheet(sheet, entityType);
                assetField.SetValue(asset, entities);
                sheetCount++;
            }

            if (info.Attribute.LogOnImport)
            {
                Debug.Log(string.Format("Imported {0} sheets.", sheetCount));
            }

            return asset;
        }

    }

}