using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
/// <summary>
/// 用于制作拓展功能基类
/// </summary>
public class ReadExcelDataBaseSO : ScriptableObject
{
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
}
