用于在 Unity 编辑器中将数据从 xls、xlsx 自动导入到自定义 ScriptableObject 的扩展，基于https://github.com/mikito/unity-excel-importer 的修改
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

![4](https://github.com/user-attachments/assets/ce7da973-b642-4d83-b744-aee1646855dc)

## 4、表格配置
之前的版本中只能将资源放在Resources中，并且配置路径时也十分繁杂，现在可以将其放在Assets中的任意位置，然后配置成【Assets+资源名】即可
![5](https://github.com/user-attachments/assets/cf05deb9-8928-46ed-af5d-8f7acedf5e44)
注意：如果资源类型为Sprite，需要确保资源自身的名字与Sprite类型的名字相同

![7](https://github.com/user-attachments/assets/e1bf1c04-cb9c-4827-be4e-87b826a5846b)

如果想要在编辑器下修改读取的SO文件，点击资源右上角的三个点多出两个选项
SetFlages 打开编辑
HideFlages 关闭编辑

![07](https://github.com/user-attachments/assets/696841a7-d001-4c75-86db-a3b0b1525d97)

点击之后会出现【同步到Excel】选项，点击会将SO当前中的数据同步到Excel中，并将当前的Excel文件内容备份，防止失误

![9](https://github.com/user-attachments/assets/091a372c-3e5e-458c-948f-9bb540a91db6)
