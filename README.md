Easy workflow to pack texture to atlas,support sprite's border messages.


## TexturePacker
It is a power tools,recommend you to support it use authorised edition [https://www.codeandweb.com/texturepacker](https://www.codeandweb.com/texturepacker)

A cracked old version can be download [here](https://download.csdn.net/download/iningwei/15137428)

## Steps

Step1: Download unity's FREE package:[TexturePacker Importer](https://assetstore.unity.com/packages/tools/sprite-management/texturepacker-importer-16641) to your project.

Step2：

- Move folder [UnityTexturePackerTool](https://github.com/iningwei/UnityTexturePackerTool/tree/master/Assets/UnityTexturePackerTool) to your project as Assets's sub dirctory

- File UnityTexturePackerTool/Resources/TexturePackerConfig.asset contains settings for origin textures、temp texturepacker output folder、final sprite output folder.Note all these settings should start with Assets.
![](https://raw.githubusercontent.com/iningwei/SelfPictureHost/master/Blog/20210427120339.png)
- Creat SpriteConfig.asset under your target folder where you want to build into sprite atlas.
![](https://raw.githubusercontent.com/iningwei/SelfPictureHost/master/Blog/20210427120045.png)
As pic shown the ``File Path`` and ``Folder Path`` are auto gen by code while validate.You can only set the ``Sprite Name``、``Width``、``Height``、``Max Size``


Step3：After you finished Step2.Use provided auto tool to gen atlas,you can do by one operation from unity's menu area:``TexturePackerTool->Build``





This project is inspired by [this blog](https://blog.csdn.net/Wrinkle2017/article/details/113618934)

