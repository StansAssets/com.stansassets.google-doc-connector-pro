using System.Collections.Generic;

namespace StansAssets.GoogleDoc
{
    public class RowData
    {
        readonly List<DataCell> m_Cells = new List<DataCell>();
        public IEnumerable<DataCell> Cells => m_Cells;

        public void AddCell(DataCell cell)
        {
            m_Cells.Add(cell);
        }
    }
}
