using System;
using System.Linq;

namespace StansAssets.GoogleDoc
{
    static class CellNameUtility
    {
        /// <exception cref="ArgumentException">The method returns an error if the column name is empty</exception>
        internal static ICellPointer GetCellPointer(string name)
        {
            name = string.Concat(name.Where(c => !char.IsWhiteSpace(c) || !char.IsPunctuation(c) || !char.IsSeparator(c) || !char.IsSymbol(c)));

            var column = 0;

            //Split row number and column number
            var strRow = string.Concat(name.Where(char.IsDigit));
            var strColumn = string.Concat(name.Where(char.IsLetter));
            if (string.IsNullOrEmpty(strRow))
            {
                throw new ArgumentException($"The column name is blank. Cell name should be like this 'A1' 'B2'");
            }
            else if (string.IsNullOrEmpty(strColumn))
            {
                throw new ArgumentException($"The row name is blank. Cell name should be like this 'A1' 'B2'");
            }

            //Convert name to row number
            if (int.TryParse(strRow, out var row))
            {
                row -= 1; //Cell Row index must starts from `0`
            }

            //Convert name to column number
            column = ColumnNameToNumber(strColumn);

            column -= 1; //Cell Column index must starts from `0`
            return new Cell(row, column);
        }

        internal static string GetCellName(int row, int column)
        {
            row += 1;
            column += 1;

            //Convert column number to name
            var strColumn = ColumnNumberToName(column);

            //Return
            return strColumn + row;
        }

        internal static string GetCellNameForRange(int? row, int? column)
        {
            var strColumn = string.Empty;
            var strRow = string.Empty;
            
            if (row != null)
                strRow = (row + 1).ToString();

            if (column != null)
            {
                column += 1;

                //Convert column number to name
                strColumn = ColumnNumberToName(column ?? 0);
            }

            //Return
            return strColumn + strRow;
        }

        internal static void ConvertCellNameToPositions(string name, out int? row, out int? column)
        {
            name = string.Concat(name.Where(c => !char.IsWhiteSpace(c) || !char.IsPunctuation(c) || !char.IsSeparator(c) || !char.IsSymbol(c)));

            row = null;
            column = null;

            //Split row number and column number
            var strRow = string.Concat(name.Where(char.IsDigit));
            var strColumn = string.Concat(name.Where(char.IsLetter));

            //Convert name to row number
            if (!string.IsNullOrEmpty(strRow) && int.TryParse(strRow, out var rowParse))
            {
                row = rowParse - 1; //Cell Row index must starts from `0`.
            }

            //Convert name to column number
            if (!string.IsNullOrEmpty(strColumn))
            {
                column = ColumnNameToNumber(strColumn);
                column -= 1; //Cell Column index must starts from `0`.
            }
        }

        internal static string ConvertCellPositionsToName(int? row, int? column)
        {
            var strColumn = string.Empty;
            var strRow = string.Empty;

            //Convert column number to name
            if (row != null)
            {
                row += 1;
                strRow = row.ToString();
            }

            //Convert column number to name
            if (column != null)
            {
                column += 1;
                strColumn = ColumnNumberToName((int)column);
            }

            //Return
            return strColumn + strRow;
        }

        static int ColumnNameToNumber(string strColumn)
        {
            var column = 0;
            strColumn = strColumn.ToUpper();
            var pow = 1;
            for (var i = strColumn.Length - 1; i >= 0; i--)
            {
                column += (strColumn[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            return column;
        }

        static string ColumnNumberToName(int column)
        {
            var strColumn = string.Empty;
            while (column > 0)
            {
                var mod = (column - 1) % 26;
                strColumn = (char)(65 + mod) + strColumn;
                column = (int)((column - mod) / 26);
            }

            return strColumn;
        }
    }
}
