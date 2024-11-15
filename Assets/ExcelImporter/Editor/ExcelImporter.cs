using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Linq;
using UnityEngine.U2D;


public class ExcelImporter : AssetPostprocessor
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

	static List<ExcelAssetInfo> cachedInfos = null; // Clear on compile.

    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		bool imported = false;
		foreach(string path in importedAssets)
		{
			if(Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx") 
			{
				if(cachedInfos == null) cachedInfos = FindExcelAssetInfos();

				var excelName = Path.GetFileNameWithoutExtension(path);
				if(excelName.StartsWith("~$")) continue;
				//如果在StreamingAssets中，不导入
				if(path.Contains("StreamingAssets")) continue;

				ExcelAssetInfo info = cachedInfos.Find(i => i.ExcelName == excelName);

				if(info == null) continue;

				ImportExcel(path, info);
				imported = true;
			}
		}

		if(imported) 
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}

	static List<ExcelAssetInfo> FindExcelAssetInfos()
	{
		var list = new List<ExcelAssetInfo>();
		foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach(var type in assembly.GetTypes())
			{
				var attributes = type.GetCustomAttributes(typeof(ExcelAssetAttribute), false);
				if(attributes.Length == 0) continue;
				var attribute = (ExcelAssetAttribute)attributes[0];
				var info = new ExcelAssetInfo()
				{
					AssetType = type,
					Attribute = attribute
				};
				list.Add(info);
			}
		}
		return list;
	}

	static UnityEngine.Object LoadOrCreateAsset(string assetPath, Type assetType)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

		var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);

		if (asset == null)
		{
			asset = ScriptableObject.CreateInstance(assetType.Name);
			AssetDatabase.CreateAsset((ScriptableObject)asset, assetPath);
			//asset.hideFlags = HideFlags.NotEditable;
			asset.hideFlags = HideFlags.NotEditable;
		}

		return asset;
	}

	static IWorkbook LoadBook(string excelPath)
	{
		using(FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
			else return new XSSFWorkbook(stream);
		}
	}

	static List<string> GetFieldNamesFromSheetHeader(ISheet sheet)
	{
		IRow headerRow = sheet.GetRow(0);

		var fieldNames = new List<string>();
		for (int i = 0; i < headerRow.LastCellNum; i++)
		{
			var cell = headerRow.GetCell(i);
			if(cell == null || cell.CellType == CellType.Blank) break;
			fieldNames.Add(cell.StringCellValue);
		}
		return fieldNames;
	}

	static object CellToFieldObject(ICell cell, FieldInfo fieldInfo, bool isFormulaEvalute = false)
	{
		var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;

		switch(type)
		{
			case CellType.String:
				if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
				else return cell.StringCellValue;
			case CellType.Boolean:
				return cell.BooleanCellValue;
			case CellType.Numeric:
				return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
			case CellType.Formula:
				if(isFormulaEvalute) return null;
				return CellToFieldObject(cell, fieldInfo, true); 
			default:
				if(fieldInfo.FieldType.IsValueType)
				{
					return Activator.CreateInstance(fieldInfo.FieldType);
				}
				return null;
		}
	}
	//将列表中的数据一一对应
	static object CreateEntityFromRow(IRow row, List<string> columnNames, Type entityType, string sheetName)
	{
		var entity = Activator.CreateInstance(entityType);

		var fields = entityType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		for (int i = 0; i < columnNames.Count; i++)
		{
			FieldInfo entityField = fields.FirstOrDefault(f => f.Name == columnNames[i]);//获得表格中的属性
			//Debug.Log("EntityField.Name: " + entityField.Name);
			if (entityField == null) continue;
			if (!entityField.IsPublic && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

			ICell cell = row.GetCell(i);
			if (cell == null) continue;
			//Tips:图片的名字通过类的字段读取，路径的名称通过表格读取
			try
			{
				object fieldValue = CellToFieldObject(cell, entityField);//表格中所有的值
				//Debug.Log("fieldValue: " + fieldValue);

				foreach (var field in fields)
                {
                    ReadSpriteFromSheet(entity, entityField, fieldValue, field);
                    ReadTextureFromSheet(entity, entityField, fieldValue, field);
                }
                entityField.SetValue(entity, fieldValue);
			}
			catch
			{
				throw new Exception(string.Format("Invalid excel cell type at row {0}, column {1}, {2} sheet.", row.RowNum, cell.ColumnIndex, sheetName));
			}
		}
		return entity;
	}

    static void ReadTextureFromSheet(object entity, FieldInfo entityField, object fieldValue, FieldInfo field)
	{
		if (field.FieldType == typeof(Texture2D))
		{
			string nameSp = field.Name.Substring(0, field.Name.Length - 7);
			if (entityField.Name.Length > 4)
			{
				string namePa = entityField.Name.Substring(0, entityField.Name.Length - 4);
				if (entityField.Name.Substring(entityField.Name.Length - 4) == "Path" && nameSp == namePa && fieldValue != null)
				{
					Debug.Log($"属性名: {field.Name}, 资源类型: {field.FieldType}, 资源路径: {fieldValue}");

					string fieldStr = fieldValue.ToString();
					if(fieldStr.Contains("+"))
					{
						string[] pn = fieldStr.Split('+');
						Texture2D[] tes = Resources.LoadAll<Texture2D>(pn[0]);
						foreach (var te in tes)
						{
							if(te.name == pn[1])
							{
								field.SetValue(entity, te);
							}
						}
					}
				}
			}
		}
	}

	static void ReadSpriteFromSheet(object entity, FieldInfo entityField, object fieldValue, FieldInfo field)
	{
		if (field.FieldType != typeof(Sprite))
			return;

		string nameSp = field.Name.Substring(0, field.Name.Length - 6);
		if (entityField.Name.Length > 4)
		{
			string namePa = entityField.Name.Substring(0, entityField.Name.Length - 4);
			if (entityField.Name.Substring(entityField.Name.Length - 4) == "Path" && nameSp == namePa && fieldValue != null)
			{
				Debug.Log($"属性名: {field.Name}, 资源类型: {field.FieldType}, 资源路径: {fieldValue}");

				string fieldStr = fieldValue.ToString();
				if(fieldStr.Contains("+"))
				{
					string[] pn = fieldStr.Split('+');
					Sprite[] allSp = Resources.LoadAll<Sprite>(pn[0]);//可改
					foreach (var sp in allSp)
					{
						if(sp.name == pn[1])
						{
							field.SetValue(entity, sp);
						}
					}
				}
				// var spriteRL = Resources.Load<Sprite>(fieldStr);
				// if (spriteRL == null)
				// {
				// 	Debug.LogError($"未找到资源{fieldStr},请检查资源路径是否正确");
				// 	return;
				// }

				// Debug.Log($"正常读取的图片：{spriteRL}");
				// field.SetValue(entity, spriteRL);
			}
		}
	}



	static object GetEntityListFromSheet(ISheet sheet, Type entityType)
	{
		List<string> excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

		Type listType = typeof(List<>).MakeGenericType(entityType);
		MethodInfo listAddMethod = listType.GetMethod("Add", new Type[]{entityType});
		object list = Activator.CreateInstance(listType);

		// row of index 0 is header
		for (int i = 1; i <= sheet.LastRowNum; i++)
		{
			IRow row = sheet.GetRow(i);
			if(row == null) break;

			ICell entryCell = row.GetCell(0); 
			if(entryCell == null || entryCell.CellType == CellType.Blank) break;

			// skip comment row
			if(entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

			var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);
			listAddMethod.Invoke(list, new object[] { entity });
		}
		return list;
	}

	static void ImportExcel(string excelPath, ExcelAssetInfo info)
	{
		string assetPath = "";
		string assetName = info.AssetType.Name + ".asset";

		if(string.IsNullOrEmpty(info.Attribute.AssetPath))
		{
			string basePath = Path.GetDirectoryName(excelPath);
			assetPath = Path.Combine(basePath, assetName);
		}else{
			var path = Path.Combine("Assets", info.Attribute.AssetPath);
			assetPath = Path.Combine(path, assetName);
		}
		UnityEngine.Object asset = LoadOrCreateAsset(assetPath, info.AssetType);

		IWorkbook book = LoadBook(excelPath);

		var assetFields = info.AssetType.GetFields();
		int sheetCount = 0;

		foreach (var assetField in assetFields)
		{
			ISheet sheet =  book.GetSheet(assetField.Name);
			if(sheet == null) continue;

			Type fieldType = assetField.FieldType;
			if(! fieldType.IsGenericType || (fieldType.GetGenericTypeDefinition() != typeof(List<>))) continue;

			Type[] types = fieldType.GetGenericArguments();
			Type entityType = types[0];

			object entities = GetEntityListFromSheet(sheet, entityType);
			assetField.SetValue(asset, entities);
			sheetCount++;
		}

		if(info.Attribute.LogOnImport)
		{
			Debug.Log(string.Format("Imported {0} sheets form {1}.", sheetCount, excelPath));
		}

		EditorUtility.SetDirty(asset);
	}
}
