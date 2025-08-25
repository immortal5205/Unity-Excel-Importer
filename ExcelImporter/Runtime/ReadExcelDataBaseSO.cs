using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[ExcelAsset]
/// <summary>
/// 用于制作拓展功能基类
/// </summary>
public abstract class ReadExcelDataBaseSO : ScriptableObject
{
	/// <summary>
	/// 数据缓存字典
	/// </summary>
	protected Dictionary<string, ItemDataBase> ItemDataDic = new Dictionary<string, ItemDataBase>();
	public string excelFilePath;
	[ContextMenu("Set Flages")]
	public void SetFlages()
	{
		hideFlags = HideFlags.None;
	}
	[ContextMenu("Hide Flages")]
	public void HideFlages()
	{
		hideFlags = HideFlags.NotEditable;
	}
	public void AddData(string key, ItemDataBase data)
	{
		if (ItemDataDic.ContainsKey(key))
		{
			ItemDataDic[key] = data;
		}
		else
		{
			ItemDataDic.Add(key, data);
		}
	}
	public ItemDataBase GetData(string key)
	{
		if(string.IsNullOrEmpty(key)) return null;
		if (ItemDataDic.ContainsKey(key))
		{
			return ItemDataDic[key];
		}
		else
		{
			Debug.LogError($"没有找到key为<{key}>的数据");
			return null;
		}
	}
	public T GetData<T>(string key) where T : ItemDataBase
	{
		return GetData(key) as T;
	}

	public void RemoveData(string key)
	{
		if (ItemDataDic.ContainsKey(key))
		{
			ItemDataDic.Remove(key);
		}
		else
		{
			Debug.LogError($"没有找到key为<{key}>的数据");
		}
	}
	public int Count()
	{
		return ItemDataDic.Count;
	}
	public string[] Keys()
	{
		return ItemDataDic.Keys.ToArray();
	}
	public ItemDataBase[] Values()
	{
		return ItemDataDic.Values.ToArray();
	}
	public int IndexOfKey(string key)
	{
		var list = ItemDataDic.Keys.ToList();
		return list.IndexOf(key);
	}

	public int IndexOfValue(ItemDataBase value)
	{
		var list = ItemDataDic.Values.ToList();
		return list.IndexOf(value);
	}
	public void LogAll()
	{
		foreach (var item in ItemDataDic)
		{
			Debug.Log(item.Key + "  " + item.Value);
		}
	}
	public bool ContainsKey(string key)
	{
		return ItemDataDic.ContainsKey(key);
	}
	public bool ContainsValue(ItemDataBase value)
	{
		return ItemDataDic.ContainsValue(value);
	}
	public abstract void Init();
	
}
