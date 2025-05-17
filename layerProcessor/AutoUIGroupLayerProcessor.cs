using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public enum EGroupStatus
    {
        idle,
        confirm,
    }
    public class AutoUIGroupLayerProcessor
    {
        private static EGroupStatus status = EGroupStatus.idle;
        public static void Process(GameObject gameObject)
        {
            RectTransformModule rectTransformModule = new RectTransformModule(gameObject);
            ConfirmModule confirmModule = new ConfirmModule();
            AutoUIEventManager.UIConfirmEvent.Subscribe(OnUIConfirmEvent);
            while (true)
            {
                switch (status)
                {
                    case EGroupStatus.idle:
                        if (AutoUIControllor.checkExit())return;
                        break;
                    case EGroupStatus.confirm:
                        rectTransformModule.DestoryModule();
                        confirmModule.DestoryModule();
                        AutoUIEventManager.UIConfirmEvent.Unsubscribe(OnUIConfirmEvent);
                        status = EGroupStatus.idle;
                        return;
                }
            }
        }
        public static  void OnUIConfirmEvent(object sender,UIConfirmArgs args)
        {
            status = EGroupStatus.confirm;
        }
    }
}