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
            if (Target.MoveUp)
            {
                var sel = Selection.activeGameObject;
                var targetComponent = sel.GetComponents<LocalizedLabel>().Last();
                var count = sel.GetComponents(typeof(Component)).ToList().IndexOf(targetComponent);
                for (var pos = count; pos > 1; pos--)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(targetComponent);
                }
                Target.MoveUp = false;
            }

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
            SelectedLang();
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
