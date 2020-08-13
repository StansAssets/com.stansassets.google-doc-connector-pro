using System;

namespace StansAssets.GoogleDoc
{
    public class GridRange
    {
        /// <summary>
        /// The end column of the range, or -1 if unbounded.
        /// </summary>
        public int EndColumnIndex { get; } = -1;
        /// <summary>
        /// The end row of the range, or -1 if unbounded.
        /// </summary>
        public int EndRowIndex { get; } = -1;
        /// <summary>
        /// The start column of the range, or -1 if unbounded.
        /// </summary>
        public int StartColumnIndex { get; } = -1;
        /// <summary>
        /// The start row of the range, or -1 if unbounded.
        /// </summary>
        public int StartRowIndex { get; } = -1;

        public GridRange() { }

        /// <summary>
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
        public GridRange(string name)
        {
            var cells = name.Split(':');
            if (cells.Length != 2)
            {
                throw new ArgumentException($"Range name '{name}' should be like this 'A1:B2' 'A:B' '1:2'");
            }

            var startCell = new Cell(cells[0]);
            var endCell = new Cell(cells[1]);
            if (startCell.Row > endCell.Row)
            {
                EndRowIndex = endCell.Row;
                StartRowIndex = startCell.Row;
            }
            else
            {
                StartRowIndex = endCell.Row;
                EndRowIndex = startCell.Row;
            }

            if (startCell.Column > endCell.Column)
            {
                StartColumnIndex = startCell.Column;
                EndColumnIndex = endCell.Column;
            }
            else
            {
                EndColumnIndex = startCell.Column;
                StartColumnIndex = endCell.Column;
            }
        }

        /// <summary>
        /// 
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
        }

        /// <summary>
        /// 
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
        }
    }
}
