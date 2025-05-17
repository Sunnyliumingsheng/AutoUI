
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFramework;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    /*
    // 对每个层级进行复杂处理
    public class AutoUIPixelLayersProcessor
    {
        /// README
        /// LayerProcessor 所有用来处理GUI的回调函数都应该作为一个局部函数放在每个case里面，这样作为回调，处理起来是超级超级方便的。
        private enum PixelState
        {
            idle,// 空闲状态不进行任何操作
            exit,// 退出
            SkipImportImage,// 等待用户选择本地的图片进行导入
            GetImagePathAndDestPath,
            ChooseNewSprite,
        };
        public static void PixelLayerProcessor(in Layer layer, GameObject layerGameObject)
        {
            // 首先生成一个rectTransform和一个确认按钮

            // 得益于这是个子线程，我将采用有利于编程的状态机模式进行处理

            switch (layer.eLayerKind)
            {
                case ELayerKind.pixel:

                    PixelState pixelState = PixelState.idle;

                    string srcPath = null; // 选择的图片路径
                    string destPath = null; // 选择的导入路径
                    Sprite selectSprite = null;


                    UIBase guiChangeRectTransform = GUIManager.CreateGUIRectTransformMode();
                    UIBase guiNotSelectSprite = GUIManager.CreateGUINotSelectSprite();
                    UIBase guiOneCertainSprite = GUIManager.CreateGUIOneCertainSprite();
                    UIBase guiManySpriteCandidate = GUIManager.CreateGUIManySpriteCandidate();
                    UIBase guiLayerConfirm = GUIManager.CreateGUILayerConfirm();

                    void OnGUINotFindSpriteEvent(object sender, GUINotFindSpriteEventArgs args)
                    {
                        guiLayerConfirm.Show();
                        if (args.SkipImportImage)
                        {
                            pixelState = PixelState.SkipImportImage;
                        }
                        else
                        {
                            pixelState = PixelState.GetImagePathAndDestPath;
                            srcPath = args.ImageSrcPath;
                            destPath = args.ImageDestPath;
                        }
                    }
                    void OnGUIManySpriteCandidateEvent(object sender, GUIManySpriteCandidateArgs args)
                    {
                        selectSprite = args.SelectSprite;
                        pixelState = PixelState.ChooseNewSprite;
                    }
                    void OnGUILayerConfirm(object sender, GUILayerConfirmArgs args)
                    {
                        pixelState = PixelState.exit;
                    }
                    void OnGUIChooseNewRectTransform(object sender, GUIChooseNewRectTransformArgs args)
                    {
                        AutoUIRectTransformProcessor.UpdateRectTransformProcessor(ref layerGameObject, args.mode);
                    }

                    FindSpriteResult findSpriteResult = AutoUIAssets.GetSprite(layer.name);


                    guiChangeRectTransform.PutOnLine0();
                    guiLayerConfirm.PutOnLine4();

                    AutoUIEventManager.GUILayerConfirmEvent.Subscribe(OnGUILayerConfirm);
                    AutoUIEventManager.GUIChooseNewRectTransformEvent.Subscribe(OnGUIChooseNewRectTransform);

                    // todo : 添加功能到这里
                    // 设置一下默认的rectTransoform样式的值
                    if (guiChangeRectTransform is GUIRectTransformMode isGuiChangeRectTransform)
                    {
                        isGuiChangeRectTransform.SetOriginRectTransform(ERectTransformMode.middleCenter);
                    }

                    if (findSpriteResult == null)
                    {
                        LogUtil.LogError("寻找资源后发生错误得到的是null");
                    }
                    if (findSpriteResult.status == EFindAssetStatus.cantFind)
                    {   // 让用户自行选择是否需要导入图片
                        guiNotSelectSprite.PutOnLine1();
                        AutoUIEventManager.GUINotFindSpriteEvent.Subscribe(OnGUINotFindSpriteEvent);
                        // 这里还有错误
                        guiLayerConfirm.Hide();
                    }
                    else
                    {
                        if (findSpriteResult.status == EFindAssetStatus.oneResult)
                        {
                            // 有一个确切的图片资源，直接使用即可
                            guiOneCertainSprite.PutOnLine1();
                            if (guiOneCertainSprite is GUIOneCertainSprite gui)
                            {
                                gui.SetSprite(findSpriteResult.oneResult.sprite);
                            }
                            // 直接执行给layerGameObject添加图片的操作
                            AutoUI.MainThread.Run(() =>
                            {
                                AutoUIPixelTool.PixelLayerGameObjectAddSprite(layerGameObject, findSpriteResult.oneResult.sprite);
                            });
                        }
                        if (findSpriteResult.status == EFindAssetStatus.manyResult)
                        {
                            // 有多个图片资源，需要用户进行选择
                            guiManySpriteCandidate.PutOnLine1();
                            AutoUIEventManager.GUIManySpriteCandidateEvent.Subscribe(OnGUIManySpriteCandidateEvent);
                            if (guiManySpriteCandidate is GUIManySpriteCandidate isGUIManySpriteCandidate)
                            {
                                Sprite[] sprites = new Sprite[findSpriteResult.manyResult.Count];
                                for (int i = 0; i < findSpriteResult.manyResult.Count; i++)
                                {
                                    sprites[i] = findSpriteResult.manyResult[i].sprite;
                                }
                                isGUIManySpriteCandidate.SetSprites(sprites);
                            }
                        }
                    }

                    // 状态机处理
                    while (true)
                    {
                        switch (pixelState)
                        {
                            case PixelState.idle:
                                if (AutoUIControllor.checkExit()) return;
                                break;
                            case PixelState.exit:
                                AutoUIEventManager.GUINotFindSpriteEvent.Unsubscribe(OnGUINotFindSpriteEvent);
                                AutoUIEventManager.GUIManySpriteCandidateEvent.Unsubscribe(OnGUIManySpriteCandidateEvent);
                                AutoUIEventManager.GUILayerConfirmEvent.Unsubscribe(OnGUILayerConfirm);
                                AutoUIEventManager.GUIChooseNewRectTransformEvent.Unsubscribe(OnGUIChooseNewRectTransform);


                                guiOneCertainSprite.Destroy();
                                guiManySpriteCandidate.Destroy();
                                guiChangeRectTransform.Destroy();
                                guiNotSelectSprite.Destroy();
                                guiLayerConfirm.Destroy();
                                return;
                            case PixelState.SkipImportImage:
                                // todo : 有空思考一下如何处理
                                LogUtil.Log("按下了跳过按钮");
                                pixelState = PixelState.idle;
                                break;
                            case PixelState.GetImagePathAndDestPath:
                                LogUtil.Log("遇到了检索不到对应图片的情况选择了路径信息" + destPath + srcPath);
                                // 导入图片，并将资源导入
                                AutoUIImagesImportProcessor.ImportImage(srcPath, destPath);

                                pixelState = PixelState.idle;
                                break;
                            case PixelState.ChooseNewSprite:
                                AutoUI.MainThread.Run(() =>
                                {
                                    AutoUIPixelTool.PixelLayerGameObjectAddSprite(layerGameObject, selectSprite);
                                });
                                pixelState = PixelState.idle;
                                break;
                        }
                    }
            }
        }

    }
    */
}