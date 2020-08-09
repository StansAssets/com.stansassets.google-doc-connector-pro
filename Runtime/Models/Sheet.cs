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

        List<RowData> m_Rows = new List<RowData>();
        public IEnumerable<RowData> Rows => m_Rows;
        
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
        /// <param name="row">Row index. Index starts from 0 //TODO is it starts from 0 or 1 </param>
        /// <param name="column">Column index. Index starts from 0 //TODO is it starts from 0 or 1 </param>
        /// <returns>Cell objects or `null` if cell wasn't found.</returns>
        public Cell GetCell(int row, int column)
        {
            if (row < m_Rows.Count)
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
            throw new NotImplementedException();
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
        /// <param name="row">Row index. Index starts from 0 //TODO is it starts from 0 or 1 </param>
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

        public List<Cell> GetColumn(int column)
        {
            var rowData = new List<Cell>();
            foreach (var row in m_Rows)
            {
                rowData.AddRange(row.Cells.Where(cell => cell.Column == column));
            }
            return rowData;
        }

        public List<Cell> GetColumn(string name)
        {
            throw new NotImplementedException();
        }
        
        public List<T> GetColumnValues<T>(int column)
        {
            return GetColumn(column).Select(cell => cell.GetValue<T>()).ToList();
        }
        
        public List<T> GetColumnValues<T>(string name)
        {
            return GetColumn(name).Select(cell => cell.GetValue<T>()).ToList();
        }
        
        //TODO GetRange(GridRange range)

        /// <summary>
        /// example: "A1B2" +  other formulas??
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Cell> GetRange(string name)
        {
            throw new NotImplementedException();
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
