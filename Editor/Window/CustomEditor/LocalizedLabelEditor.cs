using System;
using System.Linq;
using StansAssets.GoogleDoc.Localization;
using UnityEngine;
using UnityEditor;

namespace StansAssets.GoogleDoc
{
    [CustomEditor(typeof(LocalizedLabel))]
    [CanEditMultipleObjects]
    public class LocalizedLabelEditor : Editor
    {
        SerializedProperty m_Token;
        SerializedProperty m_Section;
        SerializedProperty m_TextType;
        SerializedProperty m_Prefix;
        SerializedProperty m_Suffix;
        string m_ErrorMessage = string.Empty;
        bool m_ChangeCurrentLang = true;

        void OnEnable()
        {
            m_Token = serializedObject.FindProperty("Token");
            m_Section = serializedObject.FindProperty("Section");
            m_TextType = serializedObject.FindProperty("TextType");
            m_Prefix = serializedObject.FindProperty("Prefix");
            m_Suffix = serializedObject.FindProperty("Suffix");
            var t = (LocalizedLabel)target;
            t.Lang = t.Lang ?? LocalizationClient.Default.CurrentLanguage;
            LocalizationClient.Default.OnLanguageChanged += () =>
            {
                t.Lang = LocalizationClient.Default.CurrentLanguage;
                t.UpdateLocalizationWithLang();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Token);
            var sectionPopup = EditorGUILayout.Popup(new GUIContent() { text = "Section" }, LocalizationClient.Default.Sheets.IndexOf(m_Section.stringValue),
                LocalizationClient.Default.Sheets.ToArray());
            EditorGUILayout.PropertyField(m_TextType);
            EditorGUILayout.PropertyField(m_Prefix);
            EditorGUILayout.PropertyField(m_Suffix);

            if (!m_ErrorMessage.Equals(string.Empty))
            {
                EditorGUILayout.HelpBox(m_ErrorMessage, MessageType.Error);
            }
            
            EditorGUILayout.Separator();
            m_ChangeCurrentLang = EditorGUILayout.Toggle("Change language for current label", m_ChangeCurrentLang);
            SelectedLang();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                m_ErrorMessage = string.Empty;
                var t = (LocalizedLabel)target;
                if (sectionPopup != LocalizationClient.Default.Sheets.IndexOf(m_Section.stringValue))
                {
                    t.Section = LocalizationClient.Default.Sheets[sectionPopup];
                }
                
                try
                {
                    t.UpdateLocalizationWithLang();
                }
                catch (Exception ex)
                {
                    m_ErrorMessage = ex.Message;
                }
            }
        }

        void SelectedLang()
        {
            EditorGUILayout.LabelField("Language:");
            var t = (LocalizedLabel)target;
            var loc = LocalizationClient.Default;
            var currentLang = (m_ChangeCurrentLang) ? t.Lang : loc.CurrentLanguage;
            if (loc.Languages.Any())
            {
                EditorGUILayout.BeginHorizontal();
                foreach (var lang in loc.Languages)
                {
                    var style = new GUIStyle(GUI.skin.button);
                    if (lang == currentLang)
                        style.normal.textColor = Color.cyan;
                    if (GUILayout.Button(lang, style))
                    {
                        if (m_ChangeCurrentLang)
                        {
                            t.Lang = lang;
                        }
                        else
                        {
                            loc.SetLanguage(lang);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
