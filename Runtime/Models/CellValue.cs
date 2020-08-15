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

        /// <summary>
        /// Represents a formula. 
        /// </summary>
        public float? NumberValue { get; }

        /// <summary>
        /// Represents a formula. 
        /// </summary>
        public bool? BoolValue { get; }

        internal CellValue(string formattedValue, string formulaValue, string stringValue, double? numberValue, bool? boolValue)
        {
            FormattedValue = formattedValue;
            FormulaValue = formulaValue;
            StringValue = stringValue;
            if (numberValue != null) NumberValue = (float)numberValue;
            BoolValue = boolValue;
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
            if (BoolValue != null)
            {
                return ConvertValue<T>(BoolValue.ToString());
            }

            if (NumberValue != null)
            {
                if (typeof(T) == typeof(int))
                {
                    var number = (int)NumberValue;
                    return ConvertValue<T>(number.ToString());
                }
                return ConvertValue<T>(NumberValue.ToString());
            }

            return ConvertValue<T>(StringValue ?? FormattedValue);
        }

        T ConvertValue<T>(string s)
        {
            try
            {
                return (T)Convert.ChangeType(s, typeof(T));
            }
            catch
            {
                return JsonUtility.FromJson<T>(s);
            }
        }
    }
}
