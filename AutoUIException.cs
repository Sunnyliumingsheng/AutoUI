

using System;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    public class AutoUIException : Exception
    {
        public bool isError = true;
        public string warnning;
        public AutoUIException(string message)
            : base("AutoUI Error: " + message)
        {
            this.warnning = message;
        }
        public AutoUIException(string message,bool notError):base(message) {
            isError=false;
        }

        public override string ToString()
        {
            // 使用 base.ToString() 保留原始堆栈信息
            return base.ToString();
        }
        public string WarnningMessage(){
            return this.warnning;
        }
    }









}