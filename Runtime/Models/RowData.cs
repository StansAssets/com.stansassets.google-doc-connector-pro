using System.Collections.Generic;

namespace StansAssets.GoogleDoc
{
    public class RowData
    {
        readonly List<Cell> m_Cells = new List<Cell>();
        public IEnumerable<Cell> Cells => m_Cells;

        public void AddCell(Cell cell)
        {
            m_Cells.Add(cell);
        }
    }
}
