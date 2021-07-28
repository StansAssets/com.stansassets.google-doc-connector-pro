using System;
using Newtonsoft.Json;

namespace StansAssets.GoogleDoc
{
    public class GridRange
    {
        /// <summary>
        /// The end column of the range, property are zero-based or null if unbounded.
        /// </summary>
        [JsonIgnore]
        public int? EndColumnIndex { get; }
        /// <summary>
        /// The end row of the range, property are zero-based or null if unbounded.
        /// </summary>
        [JsonIgnore]
        public int? EndRowIndex { get; }
        /// <summary>
        /// The start column of the range, property are zero-based or null if unbounded.
        /// </summary>
        [JsonIgnore]
        public int? StartColumnIndex { get; }
        /// <summary>
        /// The start row of the range, property are zero-based or null if unbounded.
        /// </summary>
        [JsonIgnore]
        public int? StartRowIndex { get; }
        
        /// <summary>
        /// Name of the range. For example A1:B2, A:B, 1:2, etc.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; private set; }

        public GridRange() { }

        /// <summary>
        /// A range on a sheet. All indexes are zero-based. Start and end index is inclusive.
        /// Attention: When transferring information, the site api ignores empty cells (but the lines remain unchanged, even if all its cells are empty).
        /// Therefore, the reference to the cell name will differ from the name on the site (for the cell name to match the name on the site, the table must be filled with data without blank cells)
        /// <list type="bullet">
        ///<listheader>
        /// <term>Example</term>
        /// </listheader>
        /// <item><term>A1:B2</term></item>
        /// <item><term>A:B</term></item>
        /// <item><term>1:2</term></item>
        ///</list>
        /// </summary>
        /// <param name="name">The name of the range</param>
        /// <exception cref="ArgumentException">Range name must consist of 2 point</exception>
        [JsonConstructor]
        public GridRange(string name)
        {
            var cells = name.Split(':');
            if (cells.Length != 2)
            {
                throw new ArgumentException($"Range name '{name}' should be like this 'A1:B2' 'A:B' '1:2'");
            }

            CellNameUtility.ConvertCellNameToPositions(cells[0], out var row1, out var column1);
            CellNameUtility.ConvertCellNameToPositions(cells[1], out var row2, out var column2);
            if ((row1 == null && row2 == null) || row1 < row2)
            {
                EndRowIndex = row2;
                StartRowIndex = row1;
            }
            else
            {
                if (row1 == null || row2 == null)
                {
                    throw new ArgumentException($"Range name '{name}' should be like this 'A1:B2' 'A:B' '1:2'");
                }
                StartRowIndex = row2;
                EndRowIndex = row1;
            }

            if ((column1 == null && column2 == null) || column1 < column2)
            {
                StartColumnIndex = column1;
                EndColumnIndex = column2;
            }
            else
            {
                if (column1 == null || column2 == null)
                {
                    throw new ArgumentException($"Range name '{name}' should be like this 'A1:B2' 'A:B' '1:2'");
                }
                EndColumnIndex = column1;
                StartColumnIndex = column2;
            }
            
            SetName();
        }

        /// <summary>
        /// A range on a sheet. All indexes are zero-based. Start and end index is inclusive.
        /// </summary>
        /// <param name="startRowIndex">number of first row</param>
        /// <param name="startColumnIndex">number of first column</param>
        /// <param name="endRowIndex">number of last row</param>
        /// <param name="endColumnIndex">number of column column</param>
        public GridRange(int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        {
            EndColumnIndex = endColumnIndex;
            EndRowIndex = endRowIndex;
            StartColumnIndex = startColumnIndex;
            StartRowIndex = startRowIndex;
            
            SetName();
        }
        
        internal GridRange(int? startRowIndex, int? startColumnIndex, int? endRowIndex, int? endColumnIndex)
        {
            EndColumnIndex = endColumnIndex;
            EndRowIndex = endRowIndex;
            StartColumnIndex = startColumnIndex;
            StartRowIndex = startRowIndex;
            
            SetName();
        }

        /// <summary>
        /// A range on a sheet. All indexes are zero-based. Start and end index is inclusive.
        /// </summary>
        /// <param name="start">start of range </param>
        /// <param name="end">end of range </param>
        /// <param name="direction">the direction is 0 - rows, otherwise 1 - columns; default is 0</param>
        public GridRange(int start, int end, int direction = 0)
        {
            if (direction == 0)
            {
                EndColumnIndex = end;
                StartColumnIndex = start;
            }
            else
            {
                EndRowIndex = end;
                StartRowIndex = start;
            }
            SetName();
        }

        void SetName()
        {
            var cell1 = CellNameUtility.GetCellNameForRange(StartRowIndex, StartColumnIndex);
            var cell2 = CellNameUtility.GetCellNameForRange(EndRowIndex, EndColumnIndex);
            Name = $"{cell1}:{cell2}";
        }

    }
}
