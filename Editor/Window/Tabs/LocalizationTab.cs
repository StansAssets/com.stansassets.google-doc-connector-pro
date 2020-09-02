#if UNITY_2019_4_OR_NEWER || UNITY_2020_2_OR_NEWER
using StansAssets.Plugins.Editor;

namespace StansAssets.GoogleDoc
{
    public class LocalizationTab : BaseTab
    {
        public LocalizationTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/LocalizationTab")
        {
            
        }
    }
}
#endif
