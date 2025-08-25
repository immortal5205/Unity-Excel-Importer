using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeExtension
{
    
}
namespace NuoYan.AttributeExtension
{
    /// <summary>
    /// 颜色
    /// </summary>
    public enum ColorType
    {
        Red,
        Yellow,
        Green,
        Blue,
        Black,
        White,
        Gray,
        Pink,
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    /// <summary>
    /// 显示枚举类型中文名称
    /// </summary>
    public class SampleChinese : PropertyAttribute
    {

    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    /// <summary>
    /// 修饰枚举中字段，赋予对应中文名称
    /// </summary>
    public class Chinese : PropertyAttribute
    {
        /// <summary>
        /// 枚举字段中文名
        /// </summary>
        public string chineseName;
        public Chinese()
        {

        }
        public Chinese(string chinese)
        {
            this.chineseName = chinese;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class NuoYanHeader : PropertyAttribute
    {
        /// <summary>
        /// Header的名称
        /// </summary>
        public string headerName;
        /// <summary>
        /// header的颜色
        /// </summary>
        public ColorType color;
        /// <summary>
        /// 是否居中
        /// </summary>
        public bool isMiddle = false;

        /// <summary>
        /// 自定义Header
        /// </summary>
        /// <param name="name">Header名称</param>
        /// <param name="color">文字颜色</param>
        /// <param name="isMiddle">是否居中</param>
        public NuoYanHeader(string name, ColorType color, bool isMiddle = false)
        {
            this.headerName = name;
            this.color = color;
            this.isMiddle = isMiddle;
        }
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ReadInEditor : PropertyAttribute
    {
        public ReadInEditor()
        {

        }
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ReadInPlay : PropertyAttribute
    {
        public ReadInPlay()
        {

        }
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class LabelText : PropertyAttribute
    {
        public string text;
        public ColorType color = ColorType.White;
        public LabelText(string text)
        {
            this.text = text;
        }
        public LabelText(string text, ColorType color)
        {
            this.text = text;
            this.color = color;
        }
    }
    /// <summary>
    /// 修饰描述 MonoBehavior，被修饰的MonoBehavior如果有自定义的Inspector编辑器方法，则此特性会失去作用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class MonoText : PropertyAttribute
    {
        public string text;
        public int fontSize = 14;
        public ColorType color = ColorType.White;
        public MonoText(string text)
        {
            this.text = text;
        }
        public MonoText(string text, int fontSize)
        {
            this.text = text;
            this.fontSize = fontSize;
        }
        public MonoText(string text, ColorType color)
        {
            this.text = text;
            this.color = color;
        }
        public MonoText(string text, int fontSize, ColorType color)
        {
            this.text = text;
            this.fontSize = fontSize;
            this.color = color;
        }
    }
    /// <summary>
    /// 枚举化一个类的所有子类，并显示其名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnumClassAttribute : PropertyAttribute
    {
        public bool flage;
        public Type baseClassType;
        public EnumClassAttribute(Type baseClassType)
        {
            this.baseClassType = baseClassType;
        }
        public EnumClassAttribute(Type baseClassType, bool flage)
        {
            this.baseClassType = baseClassType;
            this.flage = flage;
        }
    }
    /// <summary>
    /// 枚举化bool
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnumBool : PropertyAttribute
    {
        public EnumBool()
        {
        }
    }
    /// <summary>
    /// 枚举化一个数组
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnumArray : PropertyAttribute
    {
        public string[] strArray;
        public bool flage;
        public EnumArray(string[] strArray, bool flage = false)
        {
            this.strArray = strArray;
            this.flage = flage;
        }
    }
    /// <summary>
    /// 枚举化Unity中的Tag
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnumTage : PropertyAttribute
    {
        public bool flage;
        public EnumTage()
        {
        }
        public EnumTage(bool flage)
        {
            this.flage = flage;
        }
    }
}
