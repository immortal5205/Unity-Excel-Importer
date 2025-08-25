using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace NuoYan.Extension
{
    [CustomPropertyDrawer(typeof(ReadInEditor), true)]
    public class ReadInEditorEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            EditorGUI.EndDisabledGroup();
        }

    }
    [CustomPropertyDrawer(typeof(ReadInPlay), true)]
    public class ReadInPlayEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUI.BeginDisabledGroup(true);
                {

                    EditorGUI.PropertyField(position, property, label, true);

                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                // 在编辑模式下直接绘制字段
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

    }
    [CustomPropertyDrawer(type: typeof(NuoYanHeader), true)]
    public class HeaderEditor : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            NuoYanHeader myHeader = (NuoYanHeader)attribute;
            string headerName = myHeader.headerName;
            bool isMiddle = myHeader.isMiddle;
            // 设置字体样式
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14, // 设置字体大小
                fontStyle = FontStyle.Bold // 设置字体为粗体
            };

            if (isMiddle)
            {
                position = new Rect(position.x + (position.width / 2 - EditorGUIUtility.labelWidth / 2), position.y, EditorGUIUtility.labelWidth, position.height);
            }


            switch (myHeader.color)
            {
                case ColorType.Red:
                    GUI.color = Color.red;
                    break;
                case ColorType.Yellow:
                    GUI.color = Color.yellow;
                    break;
                case ColorType.Green:
                    GUI.color = Color.green;
                    break;
                case ColorType.Blue:
                    GUI.color = Color.blue;
                    break;
                case ColorType.Black:
                    GUI.color = Color.black;
                    break;
                case ColorType.White:
                    GUI.color = Color.white;
                    break;
                case ColorType.Gray:
                    GUI.color = Color.gray;
                    break;
            }
            EditorGUI.LabelField(position, headerName, style);
        }
    }

    [CustomPropertyDrawer(type: typeof(SampleChinese), true)]
    public class ChineseEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type classType = property.serializedObject.targetObject.GetType();
            FieldInfo fieldInfo = classType.GetField(property.name);
            Type enumType = fieldInfo.FieldType;
            string[] enumNames = property.enumNames;
            string[] displayNames = new string[enumNames.Length];

            // 获取字段上的 SampleChinese 特性
            SampleChinese sampleChinese = (SampleChinese)Attribute.GetCustomAttribute(fieldInfo, typeof(SampleChinese));

            if (sampleChinese != null)
            {
                for (int i = 0; i < enumNames.Length; i++)
                {
                    FieldInfo enumFieldInfo = enumType.GetField(enumNames[i]);
                    // 获取枚举值上的 Chinese 特性
                    Chinese chinese = (Chinese)Attribute.GetCustomAttribute(enumFieldInfo, typeof(Chinese));

                    if (chinese != null)
                    {
                        displayNames[i] = chinese.chineseName;
                    }
                    else
                    {
                        displayNames[i] = enumNames[i];
                    }
                }
            }
            else
            {
                // 如果没有 SampleChinese 特性，使用枚举的默认名称
                displayNames = enumNames;
            }

            property.enumValueIndex = EditorGUI.Popup(position, property.name, property.enumValueIndex, displayNames);
        }
    }

    [CustomPropertyDrawer(typeof(LabelText), true)]
    public class LabelTextEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                // 获取字段信息（处理继承和私有字段）
                var field = GetFieldInfo(property);
                LabelText labelText = field != null
                    ? field.GetCustomAttribute<LabelText>()
                    : null;

                if (labelText != null)
                {
                    // 设置标签文本和颜色
                    label.text = labelText.text;
                    Color originalColor = GUI.color;

                    // 优化颜色设置逻辑
                    GUI.color = GetColorByType(labelText.color);

                    // 绘制字段
                    EditorGUI.PropertyField(position, property, label, true);

                    // 重置GUI颜色
                    GUI.color = originalColor;
                }
                else
                {
                    // 如果没有LabelText特性，正常绘制字段
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            catch (Exception ex)
            {
                // 错误处理
                EditorGUI.LabelField(position, label.text, "LabelText绘制错误: " + ex.Message);
                Debug.LogError($"LabelTextEditor异常: {ex}");
            }
        }

        private FieldInfo GetFieldInfo(SerializedProperty property)
        {
            // 获取目标对象类型
            var targetType = property.serializedObject.targetObject.GetType();
            // 查找字段（包括私有和继承的字段）
            return targetType.GetField(property.name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private Color GetColorByType(ColorType colorType)
        {
            // 使用字典替代大量switch分支
            switch (colorType)
            {
                case ColorType.Red: return Color.red;
                case ColorType.Green: return Color.green;
                case ColorType.Blue: return Color.blue;
                case ColorType.Yellow: return Color.yellow;
                case ColorType.White: return Color.white;
                case ColorType.Black: return Color.black;
                default: return Color.white;
            }
        }

    }

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoTextEditor : Editor
    {
        private MonoText monoText;

        private void OnEnable()
        {
            monoText = (MonoText)target.GetType().GetCustomAttributes(typeof(MonoText), true).FirstOrDefault();
        }

        public override void OnInspectorGUI()
        {
            if (monoText != null)
            {
                Color color = Color.white;
                if (monoText.color.HasFlag(ColorType.Red))
                {
                    color = Color.red;
                }
                if (monoText.color.HasFlag(ColorType.Green))
                {
                    color = Color.green;
                }
                if (monoText.color.HasFlag(ColorType.Blue))
                {
                    color = Color.blue;
                }
                if (monoText.color.HasFlag(ColorType.Yellow))
                {
                    color = Color.yellow;
                }
                if (monoText.color.HasFlag(ColorType.White))
                {
                    color = Color.white;
                }

                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = color;
                style.fontSize = monoText.fontSize;
                style.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(new GUIContent(monoText.text, $"详情：{monoText.text}"), style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                // var obj = target.GameObject();
                // if (obj != null)
                // {
                //     obj.name = $"{target.GetType().Name}------ {monoText.text}";
                // }
            }
            GUILayout.Space(4);
            base.OnInspectorGUI();
        }
    }
    #region 枚举扩展
    [CustomPropertyDrawer(typeof(EnumClassAttribute), true)]
    public class EnumClassAttributeEditor : PropertyDrawer
    {
        private string[] classNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 检查属性类型
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "EnumClassAttribute只能用于string类型字段");
                return;
            }

            try
            {
                // 获取字段上的 EnumClass 特性
                var field = GetFieldInfo(property);
                EnumClassAttribute enumClass = field != null
                    ? field.GetCustomAttribute<EnumClassAttribute>()
                    : null;

                if (enumClass != null)
                {
                    classNames = GetClassNames(enumClass.baseClassType, enumClass.flage);
                    if (enumClass.flage)
                    {
                        DrawFlagsSelector(position, property, label); // 多选模式
                    }
                    else
                    {
                        DrawSingleSelector(position, property, label); // 单选模式
                    }
                }
                else
                {
                    // 特性不存在时绘制默认控件
                    EditorGUI.PropertyField(position, property, label);
                }
            }
            catch (Exception ex)
            {
                // 错误处理
                EditorGUI.LabelField(position, label.text, "EnumClass绘制错误: " + ex.Message);
                Debug.LogError($"EnumClassAttributeEditor异常: {ex}");
            }
        }

        private FieldInfo GetFieldInfo(SerializedProperty property)
        {
            var targetType = property.serializedObject.targetObject.GetType();
            return targetType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private string[] GetClassNames(Type type, bool flage)
        {
            if (type == null)
                return new string[] { "Empty" };

            var classList = UtilityEditor.GetSubCalssName(type)?.ToList();
            if (classList == null || classList.Count == 0)
                return new string[] { "Empty" };

            // 处理特殊选项（Nothing/Everything 或 Empty）
            if (flage)
            {
                classList.Remove("Nothing");
                classList.Remove("Everything");
                classList.Insert(0, "Nothing");
                classList.Insert(1, "Everything");
            }
            else
            {
                classList.Remove("Empty");
                classList.Insert(0, "Empty");
            }

            return classList.ToArray();
        }

        // 单选模式（flage=false）- 保持原Popup逻辑
        private void DrawSingleSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            if (classNames == null || classNames.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "没有可用的类");
                return;
            }

            int selectIndex = GetSingleSelectedIndex(property.stringValue);
            int index = EditorGUI.Popup(position, label.text, selectIndex, classNames);

            if (index != selectIndex)
            {
                property.stringValue = index > 0 ? classNames[index] : string.Empty;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        // 多选模式（flage=true）- 支持勾选和切换
        private void DrawFlagsSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            if (classNames == null || classNames.Length < 2)
            {
                EditorGUI.LabelField(position, label.text, "没有可用的类");
                return;
            }

            // 分割布局：标签 + 属性框 + 菜单按钮
            Rect labelRect = EditorGUI.PrefixLabel(position, label);
            Rect buttonRect = new Rect(labelRect.xMax - 50, position.y, 50, position.height); // "+"按钮
            Rect propertyRect = new Rect(labelRect.x, position.y, labelRect.width - 50, position.height); // 属性输入框

            // 绘制属性输入框（显示当前选中的类名）
            EditorGUI.PropertyField(propertyRect, property, GUIContent.none);

            // 点击"+"按钮显示多选菜单
            if (GUI.Button(buttonRect, "+"))
            {
                GenericMenu menu = new GenericMenu();
                HashSet<string> selectedClasses = GetSelectedClasses(property); // 当前选中的类集合
                List<string> actualClasses = classNames.Skip(2).ToList(); // 排除Nothing和Everything的实际类

                foreach (string className in classNames)
                {
                    bool isSelected = false;
                    // 特殊处理Nothing和Everything的勾选状态
                    if (className == "Nothing")
                    {
                        isSelected = string.IsNullOrEmpty(property.stringValue); // 空值对应Nothing选中
                    }
                    else if (className == "Everything")
                    {
                        isSelected = actualClasses.All(c => selectedClasses.Contains(c)); // 全选时选中
                    }
                    else
                    {
                        isSelected = selectedClasses.Contains(className); // 普通类直接判断
                    }

                    // 添加菜单项（带勾选状态）
                    string currentClass = className; // 闭包捕获当前类名
                    menu.AddItem(
                        new GUIContent(currentClass),
                        isSelected,
                        () => OnFlagMenuItemClicked(property, currentClass, actualClasses)
                    );
                }

                menu.ShowAsContext(); // 显示右键菜单
            }
        }

        // 获取当前选中的类集合（用于判断勾选状态）
        private HashSet<string> GetSelectedClasses(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.stringValue))
                return new HashSet<string>();

            return new HashSet<string>(
                property.stringValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
            );
        }

        // 多选菜单项点击事件（添加/移除类）
        private void OnFlagMenuItemClicked(SerializedProperty property, string className, List<string> actualClasses)
        {
            HashSet<string> selectedClasses = GetSelectedClasses(property);

            switch (className)
            {
                case "Nothing":
                    // 清空所有选中项
                    property.stringValue = string.Empty;
                    break;
                case "Everything":
                    // 切换全选/取消全选
                    bool isAllSelected = actualClasses.All(c => selectedClasses.Contains(c));
                    property.stringValue = isAllSelected ? string.Empty : string.Join(",", actualClasses);
                    break;
                default:
                    // 普通类：点击切换（添加/移除）
                    if (selectedClasses.Contains(className))
                    {
                        selectedClasses.Remove(className); // 移除已选中的类
                    }
                    else
                    {
                        selectedClasses.Add(className); // 添加未选中的类
                    }
                    property.stringValue = string.Join(",", selectedClasses); // 重新拼接
                    break;
            }

            // 应用修改
            property.serializedObject.ApplyModifiedProperties();
        }

        // 单选模式的索引计算（flage=false时用）
        private int GetSingleSelectedIndex(string className)
        {
            if (classNames == null || classNames.Length <= 1)
                return 0;

            if (string.IsNullOrEmpty(className))
                return 0;

            for (int i = 0; i < classNames.Length; i++)
            {
                if (classNames[i] == className)
                {
                    return i;
                }
            }

            return 0;
        }
    }


    [CustomPropertyDrawer(typeof(EnumBool), true)]
    public class EnumBoolEditor : PropertyDrawer
    {
        private string[] boolValues = new string[] { "False", "True" };
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 检查属性类型是否为bool
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, label.text, "EnumBool只能用于bool类型字段");
                return;
            }

            DrawBoolSelector(position, property, label);
        }

        private void DrawBoolSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            int selectIndex = GetSelectedIndex(property.boolValue.ToString());
            int index = EditorGUI.Popup(position, label.text, selectIndex, boolValues);
            if (index != selectIndex)
            {
                property.boolValue = index == 1;
            }

        }
        private int GetSelectedIndex(string boolValue)
        {
            for (int i = 0; i < boolValues.Length; i++)
            {
                if (boolValues[i].Equals(boolValue))
                {
                    return i;
                }
            }

            return 0;
        }
    }

    [CustomPropertyDrawer(typeof(EnumArray), true)]
    public class EnumArrayAttributeEditor : PropertyDrawer
    {
        private string[] strArray;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "EnumArray只能用于string类型字段");
                return;
            }

            try
            {
                var field = GetFieldInfo(property);
                EnumArray enumArray = field?.GetCustomAttribute<EnumArray>();

                if (enumArray != null)
                {
                    strArray = GetStrArray(enumArray.strArray, enumArray.flage);
                    if (enumArray.flage)
                    {
                        DrawStringFlagSelector(position, property, label);
                    }
                    else
                    {
                        DrawStringSelector(position, property, label);
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                }
            }
            catch (Exception ex)
            {
                EditorGUI.LabelField(position, label.text, "EnumArray绘制错误: " + ex.Message);
                Debug.LogError($"EnumArrayAttributeEditor异常: {ex}");
            }
        }

        private FieldInfo GetFieldInfo(SerializedProperty property)
        {
            var targetType = property.serializedObject.targetObject.GetType();
            return targetType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private string[] GetStrArray(string[] strArray, bool flage)
        {
            if (strArray == null)
                return new string[] { "Empty" };

            var classList = strArray.ToList();
            if (classList.Count == 0)
                return new string[] { "Empty" };

            if (flage)
            {
                classList.Remove("Nothing");
                classList.Remove("Everything");
                classList.Insert(0, "Nothing");
                classList.Insert(1, "Everything");
            }
            else
            {
                classList.Remove("Empty");
                classList.Insert(0, "Empty");
            }

            return classList.ToArray();
        }

        private void DrawStringSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            if (strArray == null || strArray.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "空字符串数组");
                return;
            }

            int selectIndex = GetSelectedIndex(property.stringValue);
            int index = EditorGUI.Popup(position, label.text, selectIndex, strArray);

            if (index != selectIndex)
            {
                property.stringValue = index > 0 ? strArray[index] : string.Empty;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawStringFlagSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelRect = EditorGUI.PrefixLabel(position, label);
            Rect buttonRect = new Rect(labelRect.xMax - 50, position.y, 50, position.height);
            Rect propertyRect = new Rect(labelRect.x, position.y, labelRect.width - 50, position.height);

            EditorGUI.PropertyField(propertyRect, property, GUIContent.none);

            if (GUI.Button(buttonRect, "+"))
            {
                GenericMenu menu = new GenericMenu();
                HashSet<string> existStr = GetExistStr(property);
                List<string> actualItems = strArray.Skip(2).ToList(); // 排除Nothing和Everything

                foreach (string str in strArray)
                {
                    bool isSelected = false;
                    // 特殊处理Nothing和Everything的勾选状态
                    if (str == "Nothing")
                    {
                        isSelected = string.IsNullOrEmpty(property.stringValue);
                    }
                    else if (str == "Everything")
                    {
                        isSelected = actualItems.All(item => existStr.Contains(item));
                    }
                    else
                    {
                        isSelected = existStr.Contains(str);
                    }

                    // 添加菜单项（带勾选状态）
                    string currentStr = str;
                    menu.AddItem(
                        new GUIContent(currentStr),
                        isSelected,
                        () => OnMenuItemClicked(property, currentStr, actualItems)
                    );
                }

                menu.ShowAsContext();
            }
        }

        // 获取当前已选择的项
        private HashSet<string> GetExistStr(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.stringValue))
                return new HashSet<string>();

            return new HashSet<string>(
                property.stringValue.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
            );
        }

        // 菜单项点击事件（添加/移除项）
        private void OnMenuItemClicked(SerializedProperty property, string str, List<string> actualItems)
        {
            HashSet<string> existStr = GetExistStr(property);

            switch (str)
            {
                case "Nothing":
                    // 清空所有选择
                    property.stringValue = string.Empty;
                    break;
                case "Everything":
                    // 切换全选/全不选
                    bool isAllSelected = actualItems.All(item => existStr.Contains(item));
                    property.stringValue = isAllSelected ? string.Empty : string.Join(",", actualItems);
                    break;
                default:
                    // 普通项：切换添加/移除
                    if (existStr.Contains(str))
                    {
                        existStr.Remove(str); // 移除
                    }
                    else
                    {
                        existStr.Add(str); // 添加
                    }
                    property.stringValue = string.Join(",", existStr);
                    break;
            }

            // 应用修改
            property.serializedObject.ApplyModifiedProperties();
        }

        private int GetSelectedIndex(string className)
        {
            if (strArray == null || strArray.Length <= 1)
                return 0;

            if (string.IsNullOrEmpty(className))
                return 0;

            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] == className)
                {
                    return i;
                }
            }

            return 0;
        }
    }
    [CustomPropertyDrawer(typeof(EnumTage))]
    public class EnumTageEditor : PropertyDrawer
    {
        private string[] strArray;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "EnumTage只能用于string类型字段");
                return;
            }

            try
            {
                var field = GetFieldInfo(property);
                EnumTage enumTage = field?.GetCustomAttribute<EnumTage>();

                if (enumTage != null)
                {
                    strArray = GetStrArray(enumTage.flage);
                    if (enumTage.flage)
                    {
                        DrawStringFlagSelector(position, property, label);
                    }
                    else
                    {
                        DrawStringSelector(position, property, label);
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                }
            }
            catch (Exception ex)
            {
                EditorGUI.LabelField(position, label.text, "EnumTage绘制错误: " + ex.Message);
                Debug.LogError($"EnumTageAttributeEditor异常: {ex}");
            }
        }
        private FieldInfo GetFieldInfo(SerializedProperty property)
        {
            var targetType = property.serializedObject.targetObject.GetType();
            return targetType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        private string[] GetStrArray(bool flage)
        {
            var tags = UnityEditorInternal.InternalEditorUtility.tags;
            if (tags == null)
                return new string[] { "Empty" };

            var classList = tags.ToList();
            if (classList.Count == 0)
                return new string[] { "Empty" };

            if (flage)
            {
                classList.Remove("Nothing");
                classList.Remove("Everything");
                classList.Insert(0, "Nothing");
                classList.Insert(1, "Everything");
            }
            else
            {
                classList.Remove("Empty");
                classList.Insert(0, "Empty");
            }

            return classList.ToArray();
        }
        private void DrawStringSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            if (strArray == null || strArray.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "空字符串数组");
                return;
            }

            int selectIndex = GetSelectedIndex(property.stringValue);
            int index = EditorGUI.Popup(position, label.text, selectIndex, strArray);

            if (index != selectIndex)
            {
                property.stringValue = index > 0 ? strArray[index] : string.Empty;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawStringFlagSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelRect = EditorGUI.PrefixLabel(position, label);
            Rect buttonRect = new Rect(labelRect.xMax - 50, position.y, 50, position.height);
            Rect propertyRect = new Rect(labelRect.x, position.y, labelRect.width - 50, position.height);

            EditorGUI.PropertyField(propertyRect, property, GUIContent.none);

            if (GUI.Button(buttonRect, "+"))
            {
                GenericMenu menu = new GenericMenu();
                HashSet<string> existStr = GetExistStr(property);
                List<string> actualItems = strArray.Skip(2).ToList(); // 排除Nothing和Everything

                foreach (string str in strArray)
                {
                    bool isSelected = false;
                    // 特殊处理Nothing和Everything的勾选状态
                    if (str == "Nothing")
                    {
                        isSelected = string.IsNullOrEmpty(property.stringValue);
                    }
                    else if (str == "Everything")
                    {
                        isSelected = actualItems.All(item => existStr.Contains(item));
                    }
                    else
                    {
                        isSelected = existStr.Contains(str);
                    }

                    // 添加菜单项（带勾选状态）
                    string currentStr = str;
                    menu.AddItem(
                        new GUIContent(currentStr),
                        isSelected,
                        () => OnMenuItemClicked(property, currentStr, actualItems)
                    );
                }

                menu.ShowAsContext();
            }
        }

        // 获取当前已选择的项
        private HashSet<string> GetExistStr(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.stringValue))
                return new HashSet<string>();

            return new HashSet<string>(
                property.stringValue.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
            );
        }

        // 菜单项点击事件（添加/移除项）
        private void OnMenuItemClicked(SerializedProperty property, string str, List<string> actualItems)
        {
            HashSet<string> existStr = GetExistStr(property);

            switch (str)
            {
                case "Nothing":
                    // 清空所有选择
                    property.stringValue = string.Empty;
                    break;
                case "Everything":
                    // 切换全选/全不选
                    bool isAllSelected = actualItems.All(item => existStr.Contains(item));
                    property.stringValue = isAllSelected ? string.Empty : string.Join(",", actualItems);
                    break;
                default:
                    // 普通项：切换添加/移除
                    if (existStr.Contains(str))
                    {
                        existStr.Remove(str); // 移除
                    }
                    else
                    {
                        existStr.Add(str); // 添加
                    }
                    property.stringValue = string.Join(",", existStr);
                    break;
            }

            // 应用修改
            property.serializedObject.ApplyModifiedProperties();
        }

        private int GetSelectedIndex(string className)
        {
            if (strArray == null || strArray.Length <= 1)
                return 0;

            if (string.IsNullOrEmpty(className))
                return 0;

            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] == className)
                {
                    return i;
                }
            }

            return 0;
        }


    }
    #endregion
}
