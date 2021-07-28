using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// A named range.
    /// </summary>
    [Serializable]
    public class NamedRange
    {
        /// <summary>
        /// The Id of the named range.
        /// </summary>
        [JsonProperty("i")]
        public string Id { get; }

        /// <summary>
        /// The name of the named range.
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; }
        
        /// <summary>
        /// The cells inside the named range.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<ICellPointer> Cells => m_Cells;
        IEnumerable<ICellPointer> m_Cells = new List<ICellPointer>();

        /// <summary>
        /// First and last points of the range
        /// </summary>
        [JsonProperty("r")]
        public GridRange Range { get; private set; } = new GridRange();
        
        [JsonConstructor]
        internal NamedRange(string id, string name, GridRange range)
        {
            Id = id;
            Name = name;
            Range = range;
        }
        
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

        internal void SetCells(IEnumerable<ICellPointer> cells, GridRange range)
        {
            m_Cells = cells.ToList();
            Range = range;
        }
    }
}
