using System;
using System.Collections.Generic;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// The Spreadsheet Cell.
    /// </summary>
    public class Cell : ICellPointer
    {
        /// <summary>
        /// Cell Row
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Cell Column
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
            Name = $"{row}";//TODO доделать
        }

        internal Cell(int row, int column, CellValue cellValue)
            : this(row, column)
        {
            Value = cellValue;
        }

        internal Cell(string cell)
        {
            var row = 0;
            var column = 0;
            var digits = new List<int>();
            for (int i = 0; i < cell.Length; i++)
            {
                var c = cell[i];
                var r = 0;
                if (Char.IsDigit(c))
                {
                    digits.Add(c - '0');
                }
                else
                {
                    column += char.ToUpper(c) - 65;
                }
            }

            var count = digits.Count - 1;
            for (int i = 0; i <= count; i++)
            {
                var dozens = (count == i) ? 1 : 10 * (count - i);
                row += digits[i] * dozens;
            }

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
