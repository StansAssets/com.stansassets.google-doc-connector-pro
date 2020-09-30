using System;
using StansAssets.Foundation;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Google Doc Connector Package API access point.
    /// </summary>
    public static class GoogleDocConnector
    {
        const string k_GoogleSpreadsheetsBaseUrl = "https://docs.google.com/spreadsheets/d/";
        
        /// <summary>
        /// Get <see cref="Spreadsheet"/> by it's id, if a spreadsheet with such id was added into the project using
        /// the editor settings UI. Otherwise `null`.
        /// </summary>
        /// <param name="id">An id of the spreadsheet.</param>
        /// <returns>
        /// A <see cref="Spreadsheet"/> object if a spreadsheet with such id was added into the project using
        /// the editor settings UI. Otherwise `null`.
        /// </returns>
        public static Spreadsheet GetSpreadsheet(string id)
        {
            return GoogleDocConnectorSettings.Instance.GetSpreadsheet(id);
        }
        
        /// <summary>
        /// Spreadsheet exists by it's id 
        /// </summary>
        /// <param name="id">An id of the spreadsheet.</param>
        /// <returns>True if the spreadsheet exists; otherwise false</returns>
        public static bool HasSpreadsheet(string id)
        {
            return GoogleDocConnectorSettings.Instance.HasSpreadsheet(id);
        }

        /// <summary>
        /// Returns absolute web url for the spreadsheet.
        /// </summary>
        /// <param name="id">An id of the spreadsheet.</param>
        /// <returns>String url value</returns>
        public static string GetSpreadsheetWebUrl(string id)
        {
            return $"{k_GoogleSpreadsheetsBaseUrl}{id}";
        }

        static TypeConversionInstance s_TypeConvertor;
        internal static TypeConversionInstance TypeConvertor
        {
            get
            {
                if (s_TypeConvertor == null)
                {
                    s_TypeConvertor = new TypeConversionInstance();
                    s_TypeConvertor.Register<string, byte>(Convert.ToByte);
                    s_TypeConvertor.Register<string, short>(Convert.ToInt16);
                    s_TypeConvertor.Register<string, int>(Convert.ToInt32);
                    s_TypeConvertor.Register<string, long>(Convert.ToInt64);

                    s_TypeConvertor.Register<string, ushort>(Convert.ToUInt16);
                    s_TypeConvertor.Register<string, uint>(Convert.ToUInt32);
                    s_TypeConvertor.Register<string, ulong>(Convert.ToUInt64);

                    s_TypeConvertor.Register<string, bool>(Convert.ToBoolean);

                    s_TypeConvertor.Register<string, float>(Convert.ToSingle);
                    s_TypeConvertor.Register<string, double>(Convert.ToDouble);
                    s_TypeConvertor.Register<string, decimal>(Convert.ToDecimal);
                    s_TypeConvertor.Register<string, DateTime>(CustomConvertDateTime);
                }

                return s_TypeConvertor;
            }
        }

        internal static DateTime CustomConvertDateTime(string value)
        {
            var startDate = new DateTime(1899, 12, 30);
            int.TryParse(value, out var days);
            var date = startDate.AddDays(days);
            return (date);	
        }
    }
}
