using System.Collections.Generic;
using System.Linq;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// A named range.
    /// </summary>
    public class NamedRange
    {
        /// <summary>
        /// The Id of the named range.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The name of the named range.
        /// </summary>
        public string Name { get; }

        IEnumerable<ICellPointer> m_Cells = new List<ICellPointer>();

        /// <summary>
        /// The cells inside the named range.
        /// </summary>
        public IEnumerable<ICellPointer> Cells => m_Cells;

        /// <summary>
        /// First and last points of the range
        /// </summary>
        public GridRange Range { get; private set; } = new GridRange();

        internal NamedRange(string id, string name)
        {
            Id = id;
            Name = name;
        }

        internal void SetCells(IEnumerable<ICellPointer> cells)
        {
            m_Cells = cells.ToList();
            Range = new GridRange(m_Cells.First().Row, m_Cells.First().Column, m_Cells.Last().Row, m_Cells.Last().Column);
        }
    }
}
