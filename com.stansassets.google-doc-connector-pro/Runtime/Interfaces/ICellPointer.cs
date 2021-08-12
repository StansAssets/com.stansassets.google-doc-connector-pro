namespace StansAssets.GoogleDoc
{
    public interface ICellPointer
    {
        /// <summary>
        /// Cell row
        /// </summary>
        int Row { get; }
        
        /// <summary>
        /// Cell column
        /// </summary>
        int Column { get; }
    }
}
