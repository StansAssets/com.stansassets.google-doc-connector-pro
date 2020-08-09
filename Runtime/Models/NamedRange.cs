﻿using System.Collections.Generic;

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
        
        //TODO add GridRange
        //https://developers.google.com/resources/api-libraries/documentation/sheets/v4/csharp/latest/classGoogle_1_1Apis_1_1Sheets_1_1v4_1_1Data_1_1GridRange.html
        // public GridRange Range { get; }

        internal NamedRange(string id, string name)
        {
            Id = id;
            Name = name;
        }

        internal void SetCells(IEnumerable<ICellPointer> cells)
        {
            m_Cells = cells;
        }
    }
}
