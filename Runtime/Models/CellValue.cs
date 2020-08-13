using System;
using System.ComponentModel;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// The kinds of value that a cell in a spreadsheet can have.
    /// </summary>
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
        /// Represents a formula. 
        /// </summary>
        public string StringValue { get; }
        

        internal CellValue(string formattedValue, string formulaValue, string stringValue)
        {
            FormattedValue = formattedValue;
            FormulaValue = formulaValue;
            StringValue = stringValue;
        }

        /// <summary>
        /// Converts Cell <see cref="StringValue"/> to the specified type.
        /// Some special cases:
        /// * If <see cref="T"/> is a non-primitive serializable value the JsonUtility is used to make object from string.
        /// </summary>
        /// <typeparam name="T">Type you want to convert a value to.</typeparam>
        /// <returns>Converted value.</returns>
        public T GetValue<T>()
        {
            try
            {
                var parser = TypeDescriptor.GetConverter(typeof(T));
                if (parser != null)
                {
                    return (T)parser.ConvertFromString(StringValue);
                }
                return JsonUtility.FromJson<T>(StringValue);
            }
            catch
            {
               return JsonUtility.FromJson<T>(StringValue);
            }
        }
    }
}
