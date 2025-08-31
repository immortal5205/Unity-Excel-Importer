
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// 示例：请将路径字符设置为 ”---Path“ ，如：FramePath 
///       将Sprite字符设置为 ”---Spritr“ ，如：FrameSprite
///       将Texture字符设置为 ”---Texture“ ，如：ATKTexture
/// </summary>
public class ReadExcelDataBase
{
    public string ID;
    public string IcoPath;
    public string FramePath;
    public string ATKPath;
    public Sprite FrameSprite;
    public Sprite IcoSprite;
    public Texture2D ATKTexture;

    public string Name;
}
