# 目前的最新版本
**效果已经很爽了**
首先需要拉取这个脚本，然后到PS插件市场中下载我的插件，


主要更新内容
‒ 配置表共用，PS端和Unity端共用一个配置表，考虑将配置表放在服务器上，共享配置表。
‒ 配表模式，PS端根据配置内容动态生成面板样式，简单的添加标记，Unity端可以很方便的对标记添加处理功能。不再写死功能，可扩展性大大增强
‒ prefab支持，不再只有一个prefab，而是可以通过标记，将内容标记为一个prefab
![image](https://github.com/user-attachments/assets/d3a97743-dfc4-479e-9136-f9c69b0e6de1)
![image](https://github.com/user-attachments/assets/583e69a1-08d4-452a-9f3d-45e2870bd300)



演示和记录新增组件流程
新增标题组件
描述：希望美术选择采用哪种字体，现有title和main text ，之前是根据美术惯例，提取PS中使用的字体，从而大概知道是标题还是正文。现在希望不依赖规则，而是让美术手动选择。
一 修改config.json
找到text列表，添加以下内容
![image](https://github.com/user-attachments/assets/13577c93-67e2-4d93-9eaf-d9b86feff162)

PS面板会识别到这个新增配置并在点击文字图层的时候捕获到事件，然后生成面板
![image](https://github.com/user-attachments/assets/adc0f1e8-39f0-4efb-9891-51d649428501)

二 修改Unity脚本文件
只需要找到AutoUITextLayerProcessor文件，在TextLayerProcessor函数中添加如下语句即可
![image](https://github.com/user-attachments/assets/cb22844a-f65c-47f3-95af-49869c7469c2)

整个流程可谓及其简单化，可扩展性大大增加。

添加按钮支持
描述：一般情况下，按钮都是在一个组图层上实现(逻辑与样式分离)所以只需要在组图层新增一个新的checkbox，另外注意到很多按钮都用到了一个写好的按钮效果脚本ButtonClickEffect.cs 需要在配置中动态设置新增这个按钮效果的配置。
一 修改config.json
找到group列表，新增一条按钮的checkbox
二 修改Unity脚本
找到AutoUIGroupLayerProcessor.cs的GroupLayerProcessor函数
新增内容
![image](https://github.com/user-attachments/assets/71d12f1d-367e-489b-9c3e-f4a5a312ba49)

效果展示
title设置成功
![image](https://github.com/user-attachments/assets/4a010ca6-4121-4110-a80b-277c1dc98958)

button设置成功
![image](https://github.com/user-attachments/assets/49d9d2b6-51ed-4d2f-b796-8ffeb2d524c4)

以上两个组件的添加和测试只花了我50分钟，可以说是很快了。如果熟悉Unity各种组件我相信会更快


存在的问题
美术必须明白最基础的Unity排版和组件功能，否则经常出现样式很对，但是层级关系不行不能使用的问题
例如，美术经常做一种操作，将文字放到一个组中，将图片放到一个组中，实际上应该是分出很多组，然后每个组中有一个文字和图片。


如果有更多的问题，请咨询 wechat yang15279925030
我目前还没上架插件市场，因为adobe太麻烦了，如果有需要我会私发给你。如果有公司想使用……我其实很想小赚一笔
