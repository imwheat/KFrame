# UISystem

### UIData
* 每个UI都有一个UIData
* 包括资源路径string path(key)
* 图层int layer

### UI分为UIBase和UIPanelBase

* UIBase 最基础的UI比如一个图片、Text、一个血条什么的
* UIPanelBase 基础UI面板，比如设置面板什么的。它的下面有其他的UI元素。

## 本地化配置

### UI本地化

* UI的数据都存储在LocalizationConfig里面
* UI的资源数据都是用key来引用LocalizationConfig里面的资源

### 游戏本地化

* 游戏的本地化数据都存储在LocalizationConfig里面  
* 游戏的本地化资源数据都是用key来引用LocalizationConfig里面的资源  
* 在UI界面设置语言的时候只设置UI语言，在重启游戏  
或者调用KFrame.UI.LocalizationSystem.UpdateGameLanguage更新语言

### 本地化编辑
你可以使用“项目工具/本地化编辑器”来进行编辑你的本地化配置。  
编辑器脚本见 [编辑器脚本](./Editor/LocalizationEditorWindow.cs)
