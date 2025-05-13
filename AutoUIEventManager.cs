

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIEventManager
    {
        public static readonly EventPublisher<GUINotSelectSpriteEventArgs> GUINotSelectSpriteEvent = new EventPublisher<GUINotSelectSpriteEventArgs>();
        public static readonly EventPublisher<GUIChooseNewRectTransformArgs> GUIChooseNewRectTransformEvent = new EventPublisher<GUIChooseNewRectTransformArgs>();
        public static readonly EventPublisher<GUIConfirmArgs> GUIConfirmEvent = new EventPublisher<GUIConfirmArgs>();
        public static readonly EventPublisher<GUIManySpriteCandidateArgs> GUIManySpriteCandidateEvent= new EventPublisher<GUIManySpriteCandidateArgs>(); 
        public static readonly EventPublisher<GUIOneCertainSpriteArgs> GUIOneCertainSpriteEvent = new EventPublisher<GUIOneCertainSpriteArgs>();
    }


    public class EventPublisher<T> where T : EventArgs
    {
        private event EventHandler<T> _event;

        public void Publish(object sender, T args)
        {
            _event?.Invoke(sender, args);
        }

        public void Subscribe(EventHandler<T> callback)
        {
            _event += callback;
        }

        public void Unsubscribe(EventHandler<T> callback)
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

    public class GUIChooseNewRectTransformArgs : EventArgs
    {
        public ERectTransformMode mode;
        public GUIChooseNewRectTransformArgs(ERectTransformMode mode)
        {
            this.mode = mode;
        }
    }
    public class GUIConfirmArgs:EventArgs{
       public bool confirm;
       public bool cut;
    }

    public class GUIManySpriteCandidateArgs:EventArgs{
        public string newPath;
        public GUIManySpriteCandidateArgs(string newPath){
            this.newPath=newPath;
        }
    }
    public class GUIOneCertainSpriteArgs:EventArgs{
        public Sprite sprite;
        public GUIOneCertainSpriteArgs(Sprite sprite){
            this.sprite=sprite; 
        }
    }




}