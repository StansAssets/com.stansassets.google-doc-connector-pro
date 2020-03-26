using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    public class Sample
    {
        const string k_SpreadsheetId = "19Bs5Ts6OBXh7SFNdI3W0ZK-BrNiCHVt10keUBwHX2fc";
        const string k_RangeName = "ywsb82myrvvs";

        public void SampleMethod()
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(k_SpreadsheetId);
            spreadsheet = spreadsheet ?? GoogleDocConnector.CreateSpreadsheet(k_SpreadsheetId);
            spreadsheet.Load();

            var sheet = spreadsheet.GetOrCreateSheet(0);
            Debug.Log(sheet.GetCell(3, 0));

            List<object> range = sheet.GetRange(k_RangeName);
            var builder = new StringBuilder($"NamedRange Id:{k_RangeName} Data:");
            foreach (var obj in range)
            {
                builder.Append(obj);
                builder.Append(",");
            }
            Debug.Log(builder);
        }
    }
}
