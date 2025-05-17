using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class RectTransformModule : IProcessorModule
    {
        public  GameObject gameObject;
        public UIBase rectTransformUI;
        public RectTransformModule(GameObject gameObject)
        {
            this.gameObject = gameObject;
            var rectTransform = AutoUIControllor.GetRectTransform(this.gameObject);
            if (rectTransform == null)
            {
                LogUtil.LogError("RectTransformModule的rectTransform为null");
            }
            ERectTransformMode originMode = AutoUIRectTransformProcessor.GetERectTransformModeFromRectTransform(rectTransform);
            this.rectTransformUI = new rectTransformUI(originMode, callbackChangeRectTransform);
            AutoUIBoard.SimplePut(this.rectTransformUI);
        }
        public void callbackChangeRectTransform(ERectTransformMode mode)
        {
            AutoUI.MainThread.Run(() =>
            {
                AutoUIRectTransformProcessor.UpdateRectTransformProcessor(ref gameObject, mode);
            });
        }

        public void DestoryModule()
        {
            rectTransformUI.Destroy();
        }
        
    }
}