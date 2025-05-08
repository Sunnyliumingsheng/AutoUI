

using System;
using JetBrains.Annotations;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIEventManager
    {
        public static readonly GUINotSelectSpriteEventPublisher GUINotSelectSpriteEvent = new GUINotSelectSpriteEventPublisher();
    }


    public class GUINotSelectSpriteEventPublisher : EventArgs
    {
        public event EventHandler<GUINotSelectSpriteEventArgs> _event;
        public void Publish(object sender, GUINotSelectSpriteEventArgs e)
        {
            _event?.Invoke(sender, e);
        }
        public void Subscribe(EventHandler<GUINotSelectSpriteEventArgs> callback)
        {
            _event += callback;
        }
        public void Unsubscribe(EventHandler<GUINotSelectSpriteEventArgs> callback)
        {
            _event -= callback;
        }
    }
    public class GUINotSelectSpriteEventArgs : EventArgs
    {
        public bool SkipImportImage;
        public string ImageSrcPath;
        public string ImageDestPath;
        public GUINotSelectSpriteEventArgs(bool skipImportImage, string imageSrcPath, string imageDestPath)
        {
            SkipImportImage = skipImportImage;
            ImageSrcPath = imageSrcPath;
            ImageDestPath = imageDestPath;
        }
    }






}