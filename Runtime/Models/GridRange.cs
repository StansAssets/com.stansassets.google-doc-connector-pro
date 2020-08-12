using System;
using System.Collections.Generic;
using System.Linq;

namespace StansAssets.GoogleDoc
{
    public class GridRange
    {
        /// <summary>
        /// The end column (exclusive) of the range, or not set if unbounded.
        /// </summary>
        public int EndColumnIndex { get; } = 0;
        /// <summary>
        /// The end row (exclusive) of the range, or not set if unbounded.
        /// </summary>
        public int EndRowIndex { get; } = 0;
        /// <summary>
        /// The start column (inclusive) of the range, or not set if unbounded.
        /// </summary>
        public int StartColumnIndex { get; } = 0;
        /// <summary>
        /// The start row (inclusive) of the range, or not set if unbounded.
        /// </summary>
        public int StartRowIndex { get; } = 0;

        public GridRange()
        {
            
        }

        /// <summary>
        /// example: "A1:B2"
        /// </summary>
        public GridRange(string name)
        {
            var cells = name.Split(':');
            if (cells.Length != 2)
            {
                throw new ArgumentException($"Range name '{name}' should be like this 'A1:B2' 'A:B' '1:2'");
            }

            var startCell = new Cell (cells[0]);
            var endCell = new Cell (cells[1]);
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
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="direction"></param>
        public GridRange(int start, int end, int direction)
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
