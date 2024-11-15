using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ExcelAssetAttribute : Attribute
{
	public string AssetPath { get; set; }
	public string ExcelName { get; set; }
	public bool LogOnImport { get; set; }
}


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class PictureName  : Attribute
{
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class PicturePath  : Attribute
{
}