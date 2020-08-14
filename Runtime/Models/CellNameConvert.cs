using System;
using System.Linq;

namespace StansAssets.GoogleDoc
{
    static class CellNameConvert
    {
        /// <exception cref="ArgumentException">The method returns an error if the column name is empty</exception>
        internal static void ConvertCellNameToNumbers(string name, out int row, out int column)
        {
            name = String.Concat(name.Where(c => !Char.IsWhiteSpace(c) || !Char.IsPunctuation(c) || !Char.IsSeparator(c) || !Char.IsSymbol(c)));

            row = default;
            column = default;

            //Split row number and column number
            var strRow = String.Concat(name.Where(Char.IsDigit));
            var strColumn = String.Concat(name.Where(Char.IsLetter));
            if (String.IsNullOrEmpty(strRow))
            {
                throw new ArgumentException($"The column name is blank. Cell name should be like this 'A1' 'B2'");
            }
            else if (String.IsNullOrEmpty(strColumn))
            {
                throw new ArgumentException($"The row name is blank. Cell name should be like this 'A1' 'B2'");
            }

            //Convert name to row number
            if (Int32.TryParse(strRow, out row))
            {
                row -= 1; //Cell Row are zero-based
            }

            //Convert name to column number
            column = ColumnNameToNumber(strColumn);

            column -= 1; //Cell Column are zero-based
        }

        internal static void ConvertCellNumbersToName(int row, int column, out string name)
        {
            name = default;
            row += 1;
            column += 1;

            //Convert column number to name
            var strColumn = ColumnNumberToName(column);

            //Return
            name = strColumn + row;
        }

        internal static void ConvertCellNameToNumbers(string name, out int? row, out int? column)
        {
            name = String.Concat(name.Where(c => !Char.IsWhiteSpace(c) || !Char.IsPunctuation(c) || !Char.IsSeparator(c) || !Char.IsSymbol(c)));

            row = null;
            column = null;

            //Split row number and column number
            var strRow = String.Concat(name.Where(Char.IsDigit));
            var strColumn = String.Concat(name.Where(Char.IsLetter));

            //Convert name to row number
            if (!String.IsNullOrEmpty(strRow) && Int32.TryParse(strRow, out var rrow))
            {
                row = rrow - 1; //Cell Row are zero-based
            }

            //Convert name to column number
            if (!String.IsNullOrEmpty(strColumn))
            {
                column = ColumnNameToNumber(strColumn);
                column -= 1; //Cell Column are zero-based
            }
        }

        internal static void ConvertCellNumbersToName(int? row, int? column, out string name)
        {
            name = default;
            var strColumn = String.Empty;
            var strRow = String.Empty;

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
            name = strColumn + strRow;
        }

        static int ColumnNameToNumber(string strColumn)
        {
            var column = 0;
            strColumn = strColumn.ToUpper();
            var pow = 1;
            for (int i = strColumn.Length - 1; i >= 0; i--)
            {
                column += (strColumn[i] - 'A' + 1) * pow;
                pow *= 26;
            }

            return column;
        }

        static string ColumnNumberToName(int column)
        {
            var strColumn = String.Empty;
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
