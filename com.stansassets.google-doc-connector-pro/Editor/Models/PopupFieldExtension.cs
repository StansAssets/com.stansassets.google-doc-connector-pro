using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;

namespace StansAssets.GoogleDoc.Editor
{
    public static class PopupFieldExtension
    {
        public static void RefreshChoices<T>(this PopupField<T> popupField, List<string> options)
        {
            popupField.SetPrivateFieldValue("m_Choices", options);
        }

        static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }

            if (fi == null) throw new ArgumentOutOfRangeException(nameof(propName), $"Field {propName} was not found in Type {obj.GetType().FullName}");
            fi.SetValue(obj, val);
        }
    }
}
