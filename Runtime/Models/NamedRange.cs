using System.Collections.Generic;

namespace StansAssets.GoogleDoc
{
    public class NamedRange
    {
        readonly string m_Id;
        public string Id => m_Id;

        readonly string m_Name;
        public string Name => m_Name;

        List<Cell> m_Cells = new List<Cell>();
        public IEnumerable<Cell> Cells => m_Cells;

        public NamedRange(string id, string name)
        {
            m_Id = id;
            m_Name = name;
        }

        internal void SetCells(List<Cell> cells)
        {
            m_Cells = cells;
        }
    }
}
