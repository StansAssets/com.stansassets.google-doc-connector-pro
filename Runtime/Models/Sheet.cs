using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// A sheet in a spreadsheet.
    /// </summary>
    [Serializable]
    public class Sheet
    {
        [SerializeField]
        string m_Name;

        /// <summary>
        /// The name of the sheet.
        /// </summary>
        public string Name => m_Name;

        [SerializeField]
        int m_Id;

        /// <summary>
        /// The ID of the sheet.
        /// </summary>
        public int Id => m_Id;

        List<NamedRange> m_NamedRanges = new List<NamedRange>();

        /// <summary>
        /// The named ranges defined in a sheet.
        /// </summary>
        public IEnumerable<NamedRange> NamedRanges => m_NamedRanges;

        /// <summary>
        /// Sheet Rows are zero-based
        /// </summary>
        public IEnumerable<RowData> Rows => m_Rows;
        List<RowData> m_Rows = new List<RowData>();

        internal bool NamedRangeFoldOutUIState = false;

        internal Sheet(int id, string name)
        {
            m_Id = id;
            m_Name = name;
        }

        internal void CleanupRows()
        {
            m_Rows.Clear();
        }

        internal void AddRow(RowData row)
        {
            m_Rows.Add(row);
        }

        internal void SetRows(List<RowData> rows)
        {
            m_Rows = rows;
        }

        /// <summary>
        /// Determines whether an element is in the sheet
        /// </summary>
        /// <param name="name">Name of Named Range to search</param>
        /// <returns>True if the element is in the sheet; otherwise, false</returns>
        internal bool HasNamedRange(string name)
        {
            return m_NamedRanges.Exists(n => name.Equals(n.Name));
        }

        /// <summary>
        /// Returns NamedRange with provided name
        /// </summary>
        /// <param name="name">Name of Named Range to search for</param>
        /// <returns>NamedRange if the element with provided name exists, otherwise null</returns>
        internal NamedRange GetNamedRange(string name)
        {
            return m_NamedRanges.FirstOrDefault(n => name.Equals(n.Name));
        }

        internal NamedRange CreateNamedRange(string id, string name)
        {
            var namedRange = new NamedRange(id, name);
            m_NamedRanges.Add(namedRange);
            return namedRange;
        }

        /// <summary>
        /// Gets cell from specified row & col.
        /// </summary>
        /// <param name="row">Row index. Index starts from 0 </param>
        /// <param name="column">Column index. Index starts from 0 </param>
        /// <returns>Cell objects or `null` if cell wasn't found.</returns>
        public Cell GetCell(int row, int column)
        {
            if (row >= 0 && row < m_Rows.Count)
            {
                var r = m_Rows[row];
                if (column < r.Cells.Count())
                    return r.Cells.ElementAt(column);
            }

            return null;
        }

        /// <summary>
        /// Get sell by name. For example "A1" or "B5"
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <returns>Cell objects or `null` if cell wasn't found.</returns>
        public Cell GetCell(string name)
        {
            var cellNew = new Cell(name);
            if (cellNew.Row < 0 || cellNew.Row > m_Rows.Count)
                return null;
            return m_Rows[cellNew.Row].Cells.FirstOrDefault(cell => cell.Column == cellNew.Column);
        }

        public T GetCellValue<T>(int row, int column)
        {
            return GetCell(row, column).GetValue<T>();
        }

        public T GetCellValue<T>(string name)
        {
            return GetCell(name).GetValue<T>();
        }

        /// <summary>
        /// Returns all the cells in the row.
        /// </summary>
        /// <param name="row">Row index. Index starts from 0</param>
        /// <returns>Cells List.</returns>
        public List<Cell> GetRow(int row)
        {
            var rowData = new List<Cell>();
            if (row >= 0 && row < m_Rows.Count)
            {
                rowData.AddRange(m_Rows[row].Cells);
            }

            return rowData;
        }

        public List<T> GetRowValues<T>(int row)
        {
            return GetRow(row).Select(cell => cell.GetValue<T>()).ToList();
        }
        
        /// <summary>
        /// Returns all the cells in the column.
        /// </summary>
        /// <param name="column">The name of the column</param>
        /// <returns>Cells List.</returns>
        public List<Cell> GetColumn(int column)
        {
            var rowData = new List<Cell>();
            foreach (var row in m_Rows)
            {
                rowData.AddRange(row.Cells.Where(cell => cell.Column == column));
            }

            return rowData;
        }

        /// <summary>
        /// Returns all the cells in the column.
        /// </summary>
        /// <param name="name">Column index. Index starts from 0</param>
        /// <returns>Cells List.</returns>
        public List<Cell> GetColumn(string name)
        {
            if (name.Equals(string.Empty))
                return new List<Cell>();

            var index = 0;
            for (var i = name.Length; i > 0; i--)
            {
                index += char.ToUpper(name[i - 1]) - 65;
            }

            return GetColumn(index);
        }

        public List<T> GetColumnValues<T>(int column)
        {
            return GetColumn(column).Select(cell => cell.GetValue<T>()).ToList();
        }

        public List<T> GetColumnValues<T>(string name)
        {
            return GetColumn(name).Select(cell => cell.GetValue<T>()).ToList();
        }
        /// <summary>
        /// Returns all the cells in the range.
        /// </summary>
        /// <param name="range">range consist of 2 point(start of range and end of range)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Range row indices out of range of sheet rows</exception>
        public List<Cell> GetRange(GridRange range)
        {
            var rowData = new List<Cell>();
            if (range.StartRowIndex < -1 || range.EndRowIndex >= m_Rows.Count)
            {
                throw new ArgumentException("Range row indices out of range of sheet rows");
            }

            if (range.StartRowIndex == -1 && range.EndRowIndex == -1)
            {
                for (var i = 0; i > m_Rows.Count; i++)
                {
                    rowData.AddRange(m_Rows[i].Cells.Where(cell => cell.Column >= range.StartColumnIndex - 1 && cell.Column <= range.EndColumnIndex));
                }
            }
            else if (range.StartColumnIndex == -1 && range.EndColumnIndex == -1)
            {
                for (var i = range.StartRowIndex; i >= range.EndRowIndex; i++)
                {
                    rowData.AddRange(m_Rows[i].Cells);
                }
            }
            else
            {
                for (var i = range.StartRowIndex; i >= range.EndRowIndex; i++)
                {
                    rowData.AddRange(m_Rows[i].Cells.Where(cell => cell.Column >= range.StartColumnIndex - 1 && cell.Column <= range.EndColumnIndex));
                }
            }

            return rowData;
        }

        /// <summary>
        /// Returns all the cells in the range.
        /// <list type="bullet">
        ///<listheader>
        /// <term>Example</term>
        /// </listheader>
        /// <item> <term>A1:B2</term></item>
        /// <item> <term>A:B</term></item>
        /// <item> <term>1:2</term></item>
        ///</list>
        /// </summary>
        /// <param name="name">The name of the range</param>
        /// <returns>Cells List.</returns>
        public List<Cell> GetRange(string name)
        {
            var range = new GridRange(name);
            GetRange(name);
            return GetRange(range);
        }

        /// <summary>
        /// Returns a list of <see cref="Cell"/> objects of the requested Named Range.
        /// </summary>
        /// <param name="name">Name of the requested Named Range</param>
        public List<Cell> GetNamedRangeCells(string name)
        {
            var range = GetNamedRange(name);
            return range is null
                ? new List<Cell>()
                : range.Cells.Select(cell => GetCell(cell.Row, cell.Column)).ToList();
        }

        public List<T> GetNamedRangeValues<T>(string name)
        {
            return GetNamedRangeCells(name).Select(cell => cell.GetValue<T>()).ToList();
        }
    }
}
