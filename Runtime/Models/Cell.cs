using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// The Spreadsheet Cell.
    /// </summary>
    [Serializable]
    public class Cell : ICellPointer
    {
        /// <summary>
        /// Cell Row. Index starts from `0`.
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Cell Column Index starts from `0`.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Cell Name.
        /// For example "A1" / "B20" 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Cell Value representation.
        /// </summary>
        public CellValue Value { get; private set; }
        
       /* internal DataState DataState => m_DataState;
        DataState m_DataState = DataState.Default;

        internal void SetDataState(DataState state)
        {
            m_DataState = state;
        }*/

        internal Cell(int row, int column)
        {
            Row = row;
            Column = column;
            Name = CellNameUtility.GetCellName(row, column);
        }

        [JsonConstructor]
        internal Cell(int row, int column, CellValue value)
            : this(row, column)
        {
            Value = value;
        }

        /// <exception cref="ArgumentException">The method returns an error if the column name is empty</exception>
        internal Cell(string name)
        {
            var cell = CellNameUtility.GetCellPointer(name);
            Row = cell.Row;
            Column = cell.Column;
            Name = name;
        }

        internal Cell(string cell, CellValue cellValue)
            : this(cell)
        {
            Value = cellValue;
        }

        internal void SetValue(CellValue cellValue)
        {
            Value = cellValue;
        }

        /// <summary>
        /// See <see cref="CellValue.GetValue"/> for more info.
        /// </summary>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted value.</returns>
        public T GetValue<T>()
        {
            return Value.GetValue<T>();
        }
    }

   /* enum DataState
    {
        Default,
        Updated,
        Created
    }*/
}
