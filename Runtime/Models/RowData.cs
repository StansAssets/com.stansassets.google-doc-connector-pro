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

       /* internal void UpdateCell(int position, CellValue cellValue)
        {
            if (position >=0 && position < m_Cells.Count)
            {
                m_Cells[position].SetValue(cellValue);
                m_Cells[position].SetDataState(DataState.Updated);
            }
        }*/
    }
}
