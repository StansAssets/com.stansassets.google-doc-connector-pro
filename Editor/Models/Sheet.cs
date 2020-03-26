using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    [Serializable]
    public class Sheet
    {
        [SerializeField]
        string m_Name;
        public string Name => m_Name;

        [SerializeField]
        int m_Id;
        public int Id => m_Id;

        List<NamedRange> m_NamedRanges = new List<NamedRange>();
        public IEnumerable<NamedRange> NamedRanges => m_NamedRanges;

        List<RowData> m_Rows = new List<RowData>();
        public IEnumerable<RowData> Rows => m_Rows;

        public Sheet(int id)
        {
            m_Id = id;
        }

        internal void SetName(string name)
        {
            m_Name = name;
        }

        internal void AddRow(RowData row)
        {
            m_Rows.Add(row);
        }

        internal NamedRange GetOrCreateNamedRange(string id)
        {
            foreach (var range in m_NamedRanges)
            {
                if (id.Equals(range.Id))
                {
                    return range;
                }
            }

            var namedRange = new NamedRange(id);
            m_NamedRanges.Add(namedRange);
            return namedRange;
        }

        public object GetCell(int row, int column)
        {
            if (row < m_Rows.Count)
            {
                var r = m_Rows[row];
                if (column < r.Cells.Count())
                    return r.Cells.ElementAt(column).Data;
            }

            return null;
        }

        public List<object> GetRange(string id)
        {
            List<object> data = new List<object>();
            var range = GetOrCreateNamedRange(id);
            foreach (var cell in range.Cells)
                data.Add(GetCell(cell.Row, cell.Column));

            return data;
        }

        // public T Cell<T>(string name)
        // {
        //     return default;
        // }
        //
        // public List<T> GetColumn<T>(string name)
        // {
        //     var tex = Cell<Texture2D>("A1");
        //     return default;
        // }
        // public List<T> GetColumn<T>(int index)
        // {
        //     return default;
        // }
        // public List<T> GetRow<T>(int index)
        // {
        //     return default;
        // }
        // public List<T> GetRange<T>(string name)
        // {
        //     return default;
        // }
    }
}
