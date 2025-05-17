using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class ConfirmUI : UIBase
    {
        public override void Content()
        {
            if (GUILayout.Button("确认"))
            {
                AutoUIEventManager.UIConfirmEvent.Publish(this, new UIConfirmArgs(true));
            }
        }
    }
}