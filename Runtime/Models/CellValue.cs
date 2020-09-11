using System;
using Newtonsoft.Json;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// The kinds of value that a cell in a spreadsheet can have.
    /// </summary>
    [Serializable]
    public class CellValue
    {
        /// <summary>
        /// The formatted value of the cell. This is the value as it's shown to the user.
        /// </summary>
        public string FormattedValue { get; }

        /// <summary>
        /// Represents a formula. 
        /// </summary>
        public string FormulaValue { get; }

        /// <summary>
        /// Represents a value in string format. 
        /// </summary>
        public string StringValue { get; }

        public CellValue() { }

        [JsonConstructor]
        public CellValue(string formattedValue, string formulaValue, string stringValue)
        {
            FormattedValue = formattedValue;
            FormulaValue = formulaValue;
            StringValue = stringValue;
        }

        /// <summary>
        /// Converts Cell <see cref="StringValue"/> to the specified type.
        /// Some special cases:
        /// * If <see cref="T"/> is a non-primitive serializable value the <see cref="JsonUtility"/> is used to make object from string.
        /// </summary>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted value.</returns>
        public T GetValue<T>()
        {
            if (typeof(T) == typeof(string))	
            {	
                return (T)(object)(StringValue);	
            }
            return GoogleDocConnector.TypeConvertor.HasConvertor<string, T>()
                ? GoogleDocConnector.TypeConvertor.Convert<string, T>(StringValue)
                : JsonUtility.FromJson<T>(StringValue);
        }
    }
}
