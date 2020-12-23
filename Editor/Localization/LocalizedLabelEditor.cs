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

        void OnEnable()
        {
            m_Token = serializedObject.FindProperty("m_Token.m_TokenId");
            m_Section = serializedObject.FindProperty("m_Token.m_Section");
            m_TextType = serializedObject.FindProperty("m_Token.m_TextType");
            m_Prefix = serializedObject.FindProperty("m_Token.m_Prefix");
            m_Suffix = serializedObject.FindProperty("m_Token.m_Suffix");
        }

        LocalizedLabel Target => (LocalizedLabel)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Token);
            var sectionPopup = 0;
            try
            {
                sectionPopup = EditorGUILayout.Popup(new GUIContent() { text = "Section" }, LocalizationClient.Default.Sheets.IndexOf(m_Section.stringValue),
                    LocalizationClient.Default.Sheets.ToArray());
            }
            catch
            {
                EditorGUILayout.Popup(new GUIContent() { text = "Section" }, 0, new[] { "" });
                m_ErrorMessage = "Error in the localization client";
            }

            EditorGUILayout.PropertyField(m_TextType);
            EditorGUILayout.PropertyField(m_Prefix);
            EditorGUILayout.PropertyField(m_Suffix);

            if (!m_ErrorMessage.Equals(string.Empty))
            {
                EditorGUILayout.HelpBox(m_ErrorMessage, MessageType.Error);
            }
            else
            {
                EditorGUILayout.Separator();
                SelectedLang();
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                m_ErrorMessage = string.Empty;
                if (sectionPopup != LocalizationClient.Default.Sheets.IndexOf(m_Section.stringValue))
                {
                    m_Section.stringValue = LocalizationClient.Default.Sheets[sectionPopup];
                    serializedObject.ApplyModifiedProperties();
                }

                try
                {
                    Target.UpdateLocalization();
                }
                catch (Exception ex)
                {
                    m_ErrorMessage = ex.Message;
                }
            }
        }

        void SelectedLang()
        {
            EditorGUILayout.LabelField("Available Languages:");
            var localizationClient = LocalizationClient.Default;
            if (localizationClient.Languages.Any())
            {
                EditorGUILayout.BeginHorizontal();
                foreach (var lang in localizationClient.Languages)
                {
                    var style = new GUIStyle(GUI.skin.button);
                    if (lang == localizationClient.CurrentLanguage)
                        style.normal.textColor = Color.cyan;

                    if (GUILayout.Button(lang, style))
                    {
                        localizationClient.SetLanguage(lang);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
