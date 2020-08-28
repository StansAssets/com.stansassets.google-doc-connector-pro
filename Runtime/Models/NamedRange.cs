﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

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
        public string Id { get; }

        /// <summary>
        /// The name of the named range.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The cells inside the named range.
        /// </summary>
        public IEnumerable<ICellPointer> Cells => m_Cells;
        IEnumerable<Cell> m_Cells = new List<Cell>();

        /// <summary>
        /// First and last points of the range
        /// </summary>
        public GridRange Range { get; private set; } = new GridRange();
        
        [JsonConstructor]
        internal NamedRange(string id, string name, IEnumerable<Cell> cells, GridRange range)
        {
            Id = id;
            Name = name;
            m_Cells = cells.ToList();
            Range = range;
        }
        
        internal NamedRange(string id, string name)
        {
            Id = id;
            Name = name;
        }

        internal void SetCells(IEnumerable<Cell> cells)
        {
            m_Cells = cells.ToList();
            Range = new GridRange(m_Cells.First().Row, m_Cells.First().Column, m_Cells.Last().Row, m_Cells.Last().Column);
        }

        internal void SetCells(IEnumerable<Cell> cells, GridRange range)
        {
            m_Cells = cells.ToList();
            Range = range;
        }
    }
}
