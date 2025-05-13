using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 这是一个对json数据进行递归处理和负责中断的线程。可以说是核心逻辑

    public class AutoUIControllor
    {

        // 原来你才是最终boss？！
        public static void MainControllor(Layer layers, GameObject canvasObj)
        {
            // 开始进行遍历
            RecursiveProcess(layers.layers, canvasObj);
        }
        private static void RecursiveProcess(List<Layer> layers, GameObject parent)
        {
            foreach (var layer in layers)
            {
                var task = AutoUI.MainThread.AsyncRun<GameObject>(() =>
                {
                    return AutoUIFrameworkProcesser.ProcessLayerFramework(in layer, ref parent);
                });
                GameObject layerGameObject = task.Result;
                if (layer.eLayerKind == ELayerKind.group)
                {
                    // 在PS中为Group意味着下面还有东西，并且由于group图层没有任何图片文字信息所以除了rectTransfrom之外不需要处理
                    RecursiveProcess(layer.layers, layerGameObject);
                }
                else
                {
                    LayerProcessor(layer, layerGameObject);
                }

            }
        }
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
        private static void LayerProcessor(Layer layer, GameObject layerGameObject)
        {
            // 首先生成一个rectTransform和一个确认按钮

            // 得益于这是个子线程，我将采用有利于编程的状态机模式进行处理

            switch (layer.eLayerKind)
            {
                case ELayerKind.pixel:

                    PixelState pixelState = PixelState.idle;

                    string srcPath = null; // 选择的图片路径
                    string destPath = null; // 选择的导入路径
                    string chooseSpritePath = "";

                    void OnGUINotSelectSpriteEvent(object sender, GUINotSelectSpriteEventArgs args)
                    {
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
                        chooseSpritePath = args.newPath;
                        pixelState = PixelState.ChooseNewSprite;
                    }
                    void OnGUILayerConfirm(object sender, GUIConfirmArgs args)
                    {
                        pixelState = PixelState.exit;
                    }
                    void OnGUIChooseNewRectTransform(object sender, GUIChooseNewRectTransformArgs args)
                    {
                        AutoUIRectTransformProcessor.UpdateRectTransformProcessor(ref layerGameObject, args.mode);
                    }
                    void OnGUIOneCertainSpriteEvent(object sender, GUIOneCertainSpriteArgs args)
                    {
                        Sprite sprite = args.sprite;
                        AutoUI.MainThread.Run(() =>
                        {
                            AutoUILayersProcessor.PixelLayerProcessorAddSprite(layerGameObject, sprite);
                        });
                    }

                    // 请求搜索功能搜索到图片资源的位置
                    var task = AutoUI.MainThread.AsyncRun<string[]>(() =>
                    {
                        return AutoUILayersProcessor.PixelLayerProcessorSelectSpritePath(layer, layerGameObject);
                    });
                    string[] paths = task.Result;


                    ProductBase guiChangeRectTransform = GUIManager.CreateGUIRectTransformMode();
                    ProductBase guiNotSelectSprite = GUIManager.CreateGUINotSelectSprite();
                    ProductBase guiOneCertainSprite = GUIManager.CreateGUIOneCertainSprite();
                    ProductBase guiManySpriteCandidate = GUIManager.CreateGUIManySpriteCandidate();
                    ProductBase guiLayerConfirm = GUIManager.CreateGUILayerConfirm();
                    guiChangeRectTransform.PutOnLine0();
                    guiLayerConfirm.PutOnLine4();

                    AutoUIEventManager.GUIConfirmEvent.Subscribe(OnGUILayerConfirm);
                    AutoUIEventManager.GUIChooseNewRectTransformEvent.Subscribe(OnGUIChooseNewRectTransform);

                    // todo : 添加功能到这里
                    // 默认是这个
                    if (guiChangeRectTransform is GUIRectTransformMode isGuiChangeRectTransform)
                    {
                        isGuiChangeRectTransform.SetOriginRectTransform(ERectTransformMode.middleCenter);
                    }

                    if (paths == null)
                    {   // 让用户自行选择是否需要导入图片
                        guiNotSelectSprite.PutOnLine1();
                        AutoUIEventManager.GUINotSelectSpriteEvent.Subscribe(OnGUINotSelectSpriteEvent);
                    }
                    else
                    {
                        if (paths.Length == 1)
                        {
                            // 有一个确切的图片资源，直接使用即可
                            guiOneCertainSprite.PutOnLine1();
                            AutoUIEventManager.GUIOneCertainSpriteEvent.Subscribe(OnGUIOneCertainSpriteEvent);
                            if (guiOneCertainSprite is GUIOneCertainSprite gui)
                            {
                                gui.SetSpritePath(paths[0]);
                            }
                        }
                        if (paths.Length > 1)
                        {
                            // 有多个图片资源，需要用户进行选择
                            guiManySpriteCandidate.PutOnLine1();
                            AutoUIEventManager.GUIManySpriteCandidateEvent.Subscribe(OnGUIManySpriteCandidateEvent);
                            if (guiManySpriteCandidate is GUIManySpriteCandidate isGUIManySpriteCandidate)
                            {
                                isGUIManySpriteCandidate.SetSpritePath(paths);
                            }
                        }
                    }


                    while (true)
                    {
                        switch (pixelState)
                        {
                            case PixelState.idle:
                                break;
                            case PixelState.exit:
                                AutoUIEventManager.GUINotSelectSpriteEvent.Unsubscribe(OnGUINotSelectSpriteEvent);
                                AutoUIEventManager.GUIManySpriteCandidateEvent.Unsubscribe(OnGUIManySpriteCandidateEvent);
                                AutoUIEventManager.GUIConfirmEvent.Unsubscribe(OnGUILayerConfirm);
                                AutoUIEventManager.GUIChooseNewRectTransformEvent.Unsubscribe(OnGUIChooseNewRectTransform);
                                AutoUIEventManager.GUIOneCertainSpriteEvent.Subscribe(OnGUIOneCertainSpriteEvent);


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
                                LogUtil.Log("得到了路径信息" + destPath + srcPath);
                                // 导入图片，并将资源导入
                                AutoUIImagesImportProcessor.ImportImage(srcPath, destPath);

                                pixelState = PixelState.idle;
                                break;
                            case PixelState.ChooseNewSprite:
                                AutoUI.MainThread.Run(() =>
                                {
                                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(chooseSpritePath);
                                    AutoUILayersProcessor.PixelLayerProcessorAddSprite(layerGameObject, sprite);
                                });
                                pixelState = PixelState.idle;
                                break;
                        }
                    }
            }
        }


    }

}