using StansAssets.Foundation.Editor;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class TestTab : BaseTab
    {
        public TestTab()
            : base($"{GoogleDocConnectorPackage.WindowTabsPath}/TestTab.uxml")
        {
            var connectBtn = this.Q<Button>("connect-btn");
            connectBtn.clicked += () =>
            {
                var sample = new Sample();
                sample.SampleMethod();
            };
        }
    }
}
