using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class UtilityEditor
{
    public static List<Type> GetSubCalss(Type type)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes());
        return types.Where(t => t.IsSubclassOf(type)).ToList();
    }

    public static List<string> GetSubCalssName(Type type)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes());
        return types.Where(t => t.IsSubclassOf(type)).Select(t => t.Name).ToList();
    }
    /// <summary>
    /// 获取工程目录下文件夹所有指定后缀的文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ext"></param>
    /// <returns></returns> <summary>
    /// <returns></returns>
    public static List<string> GetAllFilesForPath(string path, string ext)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(ext)) return null;
        if (!System.IO.Directory.Exists(path)) return null;
        return System.IO.Directory.GetFiles(path, ext, System.IO.SearchOption.TopDirectoryOnly).ToList();
    }
    /// <summary>
    /// 获得Asset目录下文件夹所有指定后缀的文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ext"></param>
    /// <returns></returns>
    public static List<string> GetAllAssetsForPath(string path, string ext)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (!System.IO.Directory.Exists(path)) return null;
        var files = new List<string>();
        var sos = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (var s in sos)
        {
             files.Add(s.name);
        }
        return files;
    }
}
