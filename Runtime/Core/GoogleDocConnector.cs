namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Google Doc Connector Package API access point.
    /// </summary>
    public static class GoogleDocConnector
    {
        /// <summary>
        /// Get <see cref="Spreadsheet"/> by it's id, if a spreadsheet with such id was added into the project using
        /// the editor settings UI. Otherwise `null`.
        /// </summary>
        /// <param name="id">An id of the spreadsheet</param>
        /// <returns>
        /// A <see cref="Spreadsheet"/> object if a spreadsheet with such id was added into the project using
        /// the editor settings UI. Otherwise `null`.
        /// </returns>
        public static Spreadsheet GetSpreadsheet(string id)
        {
            return GoogleDocConnectorSettings.Instance.GetSpreadsheet(id);
        }
    }
}
