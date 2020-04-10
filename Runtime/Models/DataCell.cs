namespace StansAssets.GoogleDoc
{
    public class DataCell : Cell
    {
        readonly object m_Data;
        public object Data => m_Data;

        public DataCell(int row, int column, object data) : base(row, column)
        {
            m_Data = data;
        }
    }
}
