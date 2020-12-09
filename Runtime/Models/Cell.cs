using System;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public int Row { get; }

        /// <summary>
        /// Cell Column Index starts from `0`.
        /// </summary>
        [JsonIgnore]
        public int Column { get; }

        /// <summary>
        /// Cell Name.
        /// For example "A1" / "B20" 
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; }

        /// <summary>
        /// Cell Value representation.
        /// </summary>
        [JsonProperty("v")]
        public CellValue Value { get; private set; }

        internal Cell(int row, int column)
        {
            Row = row;
            Column = column;
            Name = CellNameUtility.GetCellName(row, column);
        }
        
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
        
        [JsonConstructor]
        internal Cell(string name, CellValue value)
            : this(name)
        {
            Value = value;
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
}
