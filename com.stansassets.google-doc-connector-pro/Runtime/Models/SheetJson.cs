using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Sheet for storage in cache
    /// </summary>
    [Serializable]
    class SheetJson
    {
        /// <summary>
        /// The name of the sheet.
        /// </summary>
        [JsonProperty("n")]
        internal string Name;

        /// <summary>
        /// The ID of the sheet.
        /// </summary>
        [JsonProperty("i")]
        internal int Id;

        /// <summary>
        /// The named ranges defined in a sheet.
        /// </summary>
        [JsonProperty("r")]
        internal IEnumerable<NamedRange> NamedRanges = new List<NamedRange>();

        /// <summary>
        /// Sheet Cells
        /// </summary>
        [JsonProperty("с")]
        internal IEnumerable<Cell> Cells = new List<Cell>();

        internal SheetJson() { }

        internal SheetJson(Sheet sheet)
        {
            Id = sheet.Id;
            Name = sheet.Name;
            NamedRanges = sheet.NamedRanges;
            Cells = sheet.Rows.SelectMany(x => x.Cells);
        }

        internal Sheet ConvertToSheet()
        {
            var sheet = new Sheet(Id, Name);
            sheet.SetNamedRanges(NamedRanges.ToList());
            var rows = new List<RowData>();
            for (var i = 0; i < Cells.LastOrDefault()?.Row + 1; i++)
            {
                rows.Add(new RowData());
            }
            foreach (var cell in Cells)
            {
                rows[cell.Row].AddCell(cell);
            }

            sheet.SetRows(rows);

            return sheet;
        }
    }
}
