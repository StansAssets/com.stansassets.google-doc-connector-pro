namespace StansAssets.GoogleDoc
{
    public class Cell
    {
        readonly int m_Row;
        public int Row => m_Row;

        readonly int m_Column;
        public int Column => m_Column;

        public Cell(int row, int column)
        {
            m_Row = row;
            m_Column = column;
        }
    }
}
