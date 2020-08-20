using System;
using System.ComponentModel;
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
            if (typeof(T) == typeof(string))
            {
                return (T)(object)StringValue;
            }

            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
            {
                return (T)(object)Convert.ToByte(StringValue);
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)Convert.ToInt32(StringValue);
            }

            if (typeof(T) == typeof(long))
            {
                return (T)(object)Convert.ToInt64(StringValue);
            }

            if (typeof(T) == typeof(short))
            {
                return (T)(object)Convert.ToInt16(StringValue);
            }

            if (typeof(T) == typeof(uint))
            {
                return (T)(object)Convert.ToUInt32(StringValue);
            }

            if (typeof(T) == typeof(ulong))
            {
                return (T)(object)Convert.ToUInt64(StringValue);
            }

            if (typeof(T) == typeof(ushort))
            {
                return (T)(object)Convert.ToUInt16(StringValue);
            }

            if (typeof(T) == typeof(bool))
            {
                return (T)(object)Convert.ToBoolean(StringValue);
            }

            if (typeof(T) == typeof(char))
            {
                return (T)(object)Convert.ToChar(StringValue);
            }

            if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
            {
                return (T)(object)Convert.ToDouble(StringValue);
            }

            if (typeof(T) == typeof(decimal))
            {
                return (T)(object)Convert.ToDecimal(StringValue);
            }
            
            if (typeof(T) == typeof(DateTime))
            {
                return (T)(object)Convert.ToDateTime(StringValue);
            }

            return JsonUtility.FromJson<T>(StringValue);
        }
    }
}
