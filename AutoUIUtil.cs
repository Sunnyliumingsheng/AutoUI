namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIUtil
    {
        public static float PSTextSizeToUnityTMPFontSize(int psTextSize)
        {
            // 这是一个经验系数，请随时调整
            return psTextSize*AutoUIConfig.config.text.fontScale;
        }
    }
}