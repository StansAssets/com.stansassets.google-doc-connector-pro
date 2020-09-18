using System;

namespace StansAssets.GoogleDoc
{
    static class GoogleDocConnectorEditor
    {
        internal static Action s_SpreadsheetsChange = delegate {  };
        public static Spreadsheet CreateSpreadsheet(string id)
        {
            var spr = GoogleDocConnectorSettings.Instance.CreateSpreadsheet(id);
            s_SpreadsheetsChange();
            return spr;
        }

        public static void RemoveSpreadsheet(string id)
        {
            GoogleDocConnectorSettings.Instance.RemoveSpreadsheet(id);
            s_SpreadsheetsChange();
        }
    }
}
