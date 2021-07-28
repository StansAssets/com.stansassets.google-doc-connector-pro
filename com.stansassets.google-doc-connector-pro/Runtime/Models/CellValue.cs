using System;
using System.ComponentModel;
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
        [JsonProperty("v")]
        [DefaultValue("")]
        public string FormattedValue { get; }

        /// <summary>
        /// Represents a formula. 
        /// </summary>
        [JsonProperty("f")]
        [DefaultValue("")]
        public string FormulaValue { get; }

        /// <summary>
        /// Represents a value in string format. 
        /// </summary>
        [JsonProperty("s")]
        [DefaultValue("")]
        public string StringValue { get; }

        public CellValue()
        {
            FormattedValue = string.Empty;
            FormulaValue = string.Empty;
            StringValue = string.Empty;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattedValue">The formatted value of the cell. This is the value as it's shown to the user.</param>
        /// <param name="formulaValue">Represents a formula.</param>
        /// <param name="stringValue">Represents a value in string format.</param>
        [JsonConstructor]
        public CellValue(string formattedValue, string formulaValue, string stringValue)
        {
            FormattedValue = formattedValue ?? string.Empty;
            FormulaValue = formulaValue ?? string.Empty;
            StringValue = stringValue ?? string.Empty;
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
