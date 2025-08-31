![4](https://github.com/user-attachments/assets/dcfe9e9f-83a3-4c1e-9699-78eaed6f448c)用于在 Unity 编辑器中将数据从 xls、xlsx 自动导入到自定义 ScriptableObject 的扩展，基于https://github.com/mikito/unity-excel-importer 的修改
这里建议先查看原作者介绍的使用方法，掌握后在查看篇
使用方法可以查看原作者的介绍，这里介绍如何配置Unity资源类型的配置

 1、创建ExcelImprortSettings
![1](https://github.com/user-attachments/assets/2de488d3-1742-4061-95d3-3cef112ef626)

## 2、选择输入输出路径
（1）表格资源路径：可以设置在这个工程文件的任意位置
（2）脚本与SO文件的输出路径需要在Assets内
这里的路径字段均为String类型，之所以在Inspector面板显示为下拉选项框，是因为特性的原因
![3](https://github.com/user-attachments/assets/7d958b33-148b-44f8-8a4f-fa6fa28f52fa)
配置好后点击ExcelImprortSettings中对应的选项即可
## 3、配置类型字段
路径字段为String类型，需要字段名为“××Path”
资源类型字段，需要字段名为“××类型的Type”
![Uploading 4.png…]()

## 4、刷新
表格配置好后，返回Unity，如果SO文件没有更新，需要Reimport下
![06](https://github.com/user-attachments/assets/b6cb7e35-2b7e-42f2-acdd-166641e27426)


如果你想要在编辑器下修改读取的SO文件，可以继承 ReadExcelDataBaseSO ，此时点击资源右上角的三个点多出两个选项
SetFlages 打开编辑
HideFlages 关闭编辑

![07](https://github.com/user-attachments/assets/696841a7-d001-4c75-86db-a3b0b1525d97)


tips：由于AssetsDatabase读取资源时需要传入全部的路径和资源的后缀，笔者感觉有点麻烦，所以这里用的是Resource，如果想要修改成其他方法，只需要修改Excelimporter中的如下方法

![02](https://github.com/user-attachments/assets/3ec75570-987f-4ac7-9a74-1a64d13efe6a)
