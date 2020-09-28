using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// A sheet in a spreadsheet.
    /// </summary>
    [Serializable]
    public class Sheet: SheetMetadata
    {


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

        /*internal DataState DataState => m_DataState;
        DataState m_DataState = DataState.Default;

        internal void SetDataState(DataState state)
        {
            m_DataState = state;
        }*/

        [JsonConstructor]
        internal Sheet(int id, string name):base(id, name)
        {
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
        public bool HasNamedRange(string name)
        {
            return m_NamedRanges.Exists(n => name.Equals(n.Name));
        }

        /// <summary>
        /// Returns NamedRange with provided name
        /// </summary>
        /// <param name="name">Name of Named Range to search for</param>
        /// <returns>NamedRange if the element with provided name exists, otherwise null</returns>
        public NamedRange GetNamedRange(string name)
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
        /// Attention: When transferring information, the site api ignores empty cells (but the lines remain unchanged, even if all its cells are empty).
        /// Therefore, the reference to the cell name will differ from the name on the site (for the cell name to match the name on the site, the table must be filled with data without blank cells)
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <returns>Cell objects or `null` if cell wasn't found.</returns>
        public Cell GetCell(string name)
        {
            try
            {
                var cellNew = new Cell(name);
                if (cellNew.Row < 0 || cellNew.Row > m_Rows.Count)
                    return null;
                return m_Rows[cellNew.Row].Cells.FirstOrDefault(cell => cell.Column == cellNew.Column);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
        
        /// <summary>
        /// Gets converted cell value from specified row & col. See <see cref="CellValue.GetValue"/> for more info.
        /// </summary>
        /// <param name="row">Row index. Index starts from 0 </param>
        /// <param name="column">Column index. Index starts from 0 </param>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted value.</returns>
        public T GetCellValue<T>(int row, int column)
        {
            return GetCell(row, column).GetValue<T>();
        }
        
        /// <summary>
        /// Gets converted cell value  by name. For example "A1" or "B5". See <see cref="CellValue.GetValue"/> for more info.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted value.</returns>
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
        
        /// <summary>
        /// Returns all the cells converted values in the row. See <see cref="CellValue.GetValue"/> for more info.
        /// </summary>
        /// <param name="row">Row index. Index starts from 0</param>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted cells value</returns>
        public List<T> GetRowValues<T>(int row)
        {
            return GetRow(row).Select(cell => cell.GetValue<T>()).ToList();
        }

        /// <summary>
        /// Returns all the cells in the column. 
        /// </summary>
        /// <param name="column">Column index. Index starts from 0</param>
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
        /// <param name="name">The name of the column.</param>
        /// <returns>Cells List.</returns>
        public List<Cell> GetColumn(string name)
        {
            if (name.Equals(string.Empty))
                return new List<Cell>();

            CellNameUtility.ConvertCellNameToPositions(name, out _, out var column);

            return GetColumn(column ?? default);
        }
        
        /// <summary>
        /// Returns all the converted cells value in the column. See <see cref="CellValue.GetValue"/> for more info.
        /// </summary>
        /// <param name="column">Column index. Index starts from 0</param>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted cells value</returns>
        public List<T> GetColumnValues<T>(int column)
        {
            return GetColumn(column).Select(cell => cell.GetValue<T>()).ToList();
        }
        
        /// <summary>
        /// Returns all the converted cells value by name. For example "A" or "B". See <see cref="CellValue.GetValue"/> for more info.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted cells value</returns>
        public List<T> GetColumnValues<T>(string name)
        {
            return GetColumn(name).Select(cell => cell.GetValue<T>()).ToList();
        }

        /// <summary>
        /// Returns all the cells in the range.
        /// </summary>
        /// <param name="range">range consist of 2 point(start of range and end of range)</param>
        /// <returns>Cells List.</returns>
        /// <exception cref="ArgumentException">Range row indices out of range of sheet rows</exception>
        public List<Cell> GetRange(GridRange range)
        {
            var rowData = new List<Cell>();

            if (range.StartRowIndex == null && range.EndRowIndex == null)
            {
                if (range.StartColumnIndex == null || range.EndColumnIndex == null)
                {
                    throw new ArgumentException("Range column indices out of range of sheet rows");
                }

                foreach (var row in m_Rows)
                {
                    rowData.AddRange(row.Cells.Where(cell => cell.Column >= range.StartColumnIndex && cell.Column <= range.EndColumnIndex));
                }
            }
            else if (range.StartColumnIndex == null && range.EndColumnIndex == null)
            {
                if (range.StartRowIndex == null || range.EndRowIndex == null)
                {
                    throw new ArgumentException("Range row indices out of range of sheet rows");
                }

                if (range.StartRowIndex >= 0 && range.StartRowIndex < m_Rows.Count && range.EndRowIndex < m_Rows.Count)
                {
                    for (var i = (int)range.StartRowIndex; i <= (int)range.EndRowIndex; i++)
                    {
                        rowData.AddRange(m_Rows[i].Cells);
                    }
                }
            }
            else
            {
                if (range.StartRowIndex == null || range.EndRowIndex == null)
                {
                    throw new ArgumentException("Range row indices out of range of sheet rows");
                }

                if (range.StartColumnIndex == null || range.EndColumnIndex == null)
                {
                    throw new ArgumentException("Range column indices out of range of sheet rows");
                }

                if (range.StartRowIndex >= 0 && range.StartRowIndex < m_Rows.Count && range.EndRowIndex < m_Rows.Count)
                {
                    for (var i = (int)range.StartRowIndex; i <= (int)range.EndRowIndex; i++)
                    {
                        rowData.AddRange(m_Rows[i].Cells.Where(cell => cell.Column >= range.StartColumnIndex && cell.Column <= range.EndColumnIndex));
                    }
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
            try
            {
                var range = new GridRange(name);
                return GetRange(range);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
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
        
        /// <summary>
        ///  Returns all the converted cells value of the requested Named Range. See <see cref="CellValue.GetValue"/> for more info.
        /// </summary>
        /// <param name="name">Name of the requested Named Range</param>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Returns all the converted cells</returns>
        public List<T> GetNamedRangeValues<T>(string name)
        {
            return GetNamedRangeCells(name).Select(cell => cell.GetValue<T>()).ToList();
        }

        /*/// <summary>
        /// Updates the value for a cell, if any, or create a new cell with that value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="cellValue"></param>
        public void UpdateCell(int row, int column, CellValue cellValue)
        {
            if (row < 0 || column < 0)
            {
                throw new Exception("Row and Column must be greater than or equal to zero");
            }

            if (GetCell(row, column) != null)
            {
                m_Rows[row].UpdateCell(column, cellValue);
                return;
            }

            while (m_Rows.Count <= row)
            {
                m_Rows.Add(new RowData());
            }

            for (var columnIndex = m_Rows[row].Cells.Count(); columnIndex < column; columnIndex++)
            {
                m_Rows[row].AddCell(new Cell(row, columnIndex));
            }

            var cell = new Cell(row, column, cellValue);
            cell.SetDataState(DataState.Updated);
            m_Rows[row].AddCell(cell);
        }*/
    }
}
