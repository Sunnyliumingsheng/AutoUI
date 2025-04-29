

using System;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    public class AutoUIException : Exception
    {
        public string message;
        public AutoUIException(string message){
            this.message="AutoUI Error:"+message;
         }
        public override string ToString(){
            return message;
        }
    }










}