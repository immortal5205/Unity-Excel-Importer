using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ReadExcelDataBaseSO),true)]
public class ReadExcelDataBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        ReadExcelDataBaseSO readSprite = (ReadExcelDataBaseSO)target;
        var sub = GetSubCalss(typeof(ReadExcelDataBaseSO));
        string subName = "";
        // 更改字体颜色和大小
        GUIStyle styleH = new GUIStyle(EditorStyles.boldLabel);
        styleH.normal.textColor = Color.green;
        styleH.fontSize = 16;
        GUIStyle styleH2 = new GUIStyle();
        styleH2.normal.textColor = Color.white;
        styleH2.fontSize = 14;
        
        GUILayout.Label($"继承ReadExcelDataBaseSO的所有子类：{sub.Count}", styleH);
        GUILayout.Space(10); // 添加空格
        for (int i = 0; i < sub.Count; i++)
        {
            subName += sub[i].Name + "\n\n";
        }
        GUILayout.Label(subName, styleH2);

    }

    /// <summary>
    /// 获得一个类型的所有子类
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns> <summary>
    List<Type> GetSubCalss(Type type)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes());
        return types.Where(t => t.IsSubclassOf(type)).ToList();
    }
    List<FieldInfo> GetListFields(Type type)
    {
        List<FieldInfo> listFields = new List<FieldInfo>();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                listFields.Add(field);
            }
        }
        return listFields;
    }
}
