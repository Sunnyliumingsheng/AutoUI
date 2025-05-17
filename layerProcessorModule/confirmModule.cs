namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class ConfirmModule : IProcessorModule
    {
        public ConfirmUI confirmUI;
        public void DestoryModule()
        {
            confirmUI.Destroy();
        }
        public ConfirmModule()
        {
            this.confirmUI = new ConfirmUI();
            AutoUIBoard.PutEndLine(confirmUI);
        }
    }
}