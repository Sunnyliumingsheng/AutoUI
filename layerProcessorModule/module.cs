namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 模块通过简单的组合就可以让processor能够,每个类中只存放回调即可
    public interface IProcessorModule
    {
        public void DestoryModule();
        
    }
}