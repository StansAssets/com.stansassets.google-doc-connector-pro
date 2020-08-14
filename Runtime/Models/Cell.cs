using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// The Spreadsheet Cell.
    /// </summary>
    public class Cell : ICellPointer
    {
        /// <summary>
        /// Cell Row are zero-based
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Cell Column are zero-based
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
        public CellValue Value { get; }

        internal Cell(int row, int column)
        {
            Row = row;
            Column = column;
            CellNameConvert.ConvertCellNumbersToName(row, column, out var name);
            Name = name;
        }

        internal Cell(int row, int column, CellValue cellValue)
            : this(row, column)
        {
            Value = cellValue;
        }

        /// <exception cref="ArgumentException">The method returns an error if the column name is empty</exception>
        internal Cell(string cell)
        {
            CellNameConvert.ConvertCellNameToNumbers(cell, out var row, out int column);
            Row = row;
            Column = column;
            Name = cell;
        }

        internal Cell(string cell, CellValue cellValue)
            : this(cell)
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
}
