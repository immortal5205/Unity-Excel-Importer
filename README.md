![04](https://github.com/user-attachments/assets/9d48b548-595b-4432-931d-cd011739f030)# Unity-Excel-Importer
用于在 Unity 编辑器中将数据从 xls、xlsx 自动导入到自定义 ScriptableObject 的扩展，基于https://github.com/mikito/unity-excel-importer 的修改
这里建议先查看原作者介绍的使用方法，掌握后在查看篇
使用方法可以查看原作者的介绍，这里介绍如何配置图片的读取

## 1、属性的命名
在类中需要将图片的路径命名为”---Path“，图片命名为”---Sprite“或者”---Texture“，如下图（-名字要相同- ）

![01](https://github.com/user-attachments/assets/debfa762-9b16-4182-8cd5-d9fcd43ceeea)
## 2、资源路径
将想要读取的图片资源放在Resoures文件夹中

![03](https://github.com/user-attachments/assets/a9766711-f6db-4272-902d-aea9cb6a2b69)
## 3、配置表格
只需要配置图片的路径即可
![04](https://github.com/user-attachments/assets/53476a86-b2e5-428f-9093-c4922f3df9e0)

## 4、刷新
表格配置好后，返回Unity，如果SO文件没有更新，需要Reimport下


![04](https://github.com/user-attachments/assets/5142b984-1e4d-4bd5-a3e9-38e7c04473cc)


如果你想要在编辑器下修改读取的SO文件，可以继承 ReadExcelDataBaseSO ，此时点击资源右上角的三个点多出两个选项
SetFlages 打开编辑
HideFlages 关闭编辑


tips：由于AssetsDatabase读取资源时需要传入全部的路径和资源的后缀，笔者感觉有点麻烦，所以这里用的是Resource，如果想要修改成其他方法，只需要修改Excelimporter中的如下方法

![02](https://github.com/user-attachments/assets/3ec75570-987f-4ac7-9a74-1a64d13efe6a)
