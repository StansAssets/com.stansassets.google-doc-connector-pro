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
            ConvertCellNumbersToName(row, column, out var name);
            Name = name;
        }

        internal Cell(int row, int column, CellValue cellValue)
            : this(row, column)
        {
            Value = cellValue;
        }

        internal Cell(string cell)
        {
            cell = String.Concat(cell.Where(c => !Char.IsWhiteSpace(c) || !Char.IsPunctuation(c) || !Char.IsSeparator(c) || !Char.IsSymbol(c)));
            ConvertCellNameToNumbers(cell, out var row, out var column);

            Row = row;
            Column = column;
            Name = cell;
        }

        void ConvertCellNameToNumbers(string name, out int row, out int column)
        {
            row = default;
            column = default;

            //Split row number and column number
            var strRow = String.Concat(name.Where(Char.IsDigit));
            var strColumn = String.Concat(name.Where(Char.IsLetter));

            //Convert name to row number
            if (Int32.TryParse(strRow, out row))
            {
                row -= 1;
            }

            //Convert name to column number
            strColumn = strColumn.ToUpper();
            var pow = 1;
            for (int i = strColumn.Length - 1; i >= 0; i--)
            {
                column += (strColumn[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            column -= 1; //Cell Column are zero-based
        }

        void ConvertCellNumbersToName(int row, int column, out string name)
        {
            name = default;
            row += 1;
            column += 1;

            //Convert column number to name
            var strColumn = String.Empty;
            var mod = 0;
            while (column > 0)
            {
                mod = (column - 1) % 26;
                strColumn = (char)(65 + mod) + strColumn;
                column = (int)((column - mod) / 26);
            }

            //Return
            name = strColumn + row;
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
