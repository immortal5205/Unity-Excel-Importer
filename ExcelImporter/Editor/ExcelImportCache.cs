using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExcelImportCache
{
    // 缓存数据结构：Excel文件路径 → 最后修改时间
    private Dictionary<string, DateTime> _cache = new Dictionary<string, DateTime>();
    // 缓存文件路径
    private static string CacheFilePath => Path.Combine(Application.dataPath, "NuoYan/ExcelImporter/Editor/ExcelImportCache.json").Replace("/", "\\");

    // 单例实例
    private static ExcelImportCache _instance;
    public static ExcelImportCache Instance => _instance ?? (_instance = Load());

    // 加载缓存
    private static ExcelImportCache Load()
    {
        var cache = new ExcelImportCache();
        if (File.Exists(CacheFilePath))
        {
            try
            {
                string json = File.ReadAllText(CacheFilePath);
                var data = JsonUtility.FromJson<CacheData>(json);
                cache._cache = data.pathToTime;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"加载Excel缓存失败：{e.Message}，将使用新缓存");
                cache._cache = new Dictionary<string, DateTime>();
            }
        }
        return cache;
    }

    // 检查文件是否需要导入（返回true表示需要导入）
    public bool NeedImport(string excelPath)
    {
        // 获取当前文件的最后修改时间
        DateTime currentTime = File.GetLastWriteTimeUtc(excelPath);
        
        // 缓存中无记录 → 需要导入
        if (!_cache.TryGetValue(excelPath, out DateTime cachedTime))
            return true;
        
        // 当前时间晚于缓存时间 → 文件已修改，需要导入
        return currentTime > cachedTime;
    }

    // 更新缓存（记录文件的最新修改时间）
    public void UpdateCache(string excelPath)
    {
        DateTime currentTime = File.GetLastWriteTimeUtc(excelPath);
        if (_cache.ContainsKey(excelPath))
            _cache[excelPath] = currentTime;
        else
            _cache.Add(excelPath, currentTime);
        
        Save();
    }

    // 保存缓存到JSON文件
    private void Save()
    {
        var data = new CacheData { pathToTime = _cache };
        string json = JsonUtility.ToJson(data, true);

        Directory.CreateDirectory(Path.GetDirectoryName(CacheFilePath));
        File.WriteAllText(CacheFilePath, json);
    }
    [Serializable]
    private class CacheData
    {
        [SerializeField] public Dictionary<string, DateTime> pathToTime = new Dictionary<string, DateTime>();
    }
}