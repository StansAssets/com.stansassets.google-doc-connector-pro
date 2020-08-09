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
        }

        internal Cell(int row, int column, CellValue cellValue):this(row, column)
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
