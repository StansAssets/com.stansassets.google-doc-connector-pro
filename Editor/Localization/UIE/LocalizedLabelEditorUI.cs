using System;
using System.Collections.Generic;
using System.Linq;
using StansAssets.Foundation.UIElements;
using StansAssets.GoogleDoc.Localization;
using StansAssets.Plugins.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc
{
    public class LocalizedLabelEditorUI : BaseTab
    {
        readonly VisualElement m_Root;
        VisualElement m_ListLang;
        HelpBox m_ErrorHelpBox;

        readonly LocalizedLabel m_LocalizedLabel;
        readonly SerializedObject m_SerializedObject;
        readonly SerializedProperty m_SectionProperty;

        public LocalizedLabelEditorUI(LocalizedLabel localizedLabel, SerializedObject serializedObject)
            : base($"{GoogleDocConnectorPackage.UILocalizationPath}/LocalizedLabelEditorUI")
        {
            m_LocalizedLabel = localizedLabel;
            m_SerializedObject = serializedObject;
            m_SectionProperty = serializedObject.FindProperty("m_Token.m_Section");
            m_Root = this.Q<VisualElement>("LocalizedLabelEditorRoot");

            GoogleDocConnectorLocalization.SpreadsheetIdChanged += Bind;
            try
            {
                LocalizationClient.Default.OnLanguageChanged += UpdateLocalization;
            }
            catch (Exception exception)
            {
                UpdateLocalizationError(exception.Message);
                return;
            }

            Bind();
        }

        void Bind()
        {
            m_Root.Clear();
            PropertyField("Token Id", "m_Token.m_TokenId");
            try
            {
                var values = LocalizationClient.Default.Sections;
                if (!values.Contains(m_LocalizedLabel.m_Token.Section))
                {
                    m_SectionProperty.stringValue = values.First();
                    m_SerializedObject.ApplyModifiedProperties();
                }

                PropertyPopup("Section", "m_Token.m_Section", LocalizationClient.Default.Sections);
            }
            catch (Exception exception)
            {
                m_Root.Clear();
                UpdateLocalizationError(exception.Message);
                return;
            }

            PropertyPopup("Text Type", "m_Token.m_TextType", Enum.GetNames(typeof(TextType)).ToList());
            PropertyField("Prefix", "m_Token.m_Prefix");
            PropertyField("Suffix", "m_Token.m_Suffix");
            InitErrorHelpBox();
            var labelLang = new Label() { text = "Available Languages:" };
            labelLang.AddToClassList("header-lang");
            m_Root.Add(labelLang);
            m_ListLang = new VisualElement();
            m_ListLang.AddToClassList("list-lang");
            m_Root.Add(m_ListLang);
            SelectedLang(LocalizationClient.Default.CurrentLanguage);

            var so = new SerializedObject(m_LocalizedLabel);
            m_Root.Bind(so);
        }

        void PropertyField(string propertyName, string bindingPath)
        {
            var propertyField = new TextField(propertyName) { bindingPath = bindingPath };
            propertyField.RegisterCallback<KeyUpEvent>((ev) =>
            {
                UpdateLocalization();
            });
            m_Root.Add(propertyField);
        }

        void PropertyPopup(string propertyName, string bindingPath, List<string> values)
        {
            var propertyField = new PopupField<string>(propertyName, values, 0) { bindingPath = bindingPath };
            propertyField.RegisterCallback<MouseDownEvent>(ev =>
            {
                schedule.Execute(UpdateLocalization).StartingIn(5);
            });
            m_Root.Add(propertyField);
        }

        void UpdateLocalization()
        {
            try
            {
                UpdateLocalizationError(string.Empty);
                m_LocalizedLabel.UpdateLocalization();
            }
            catch (Exception ex)
            {
                UpdateLocalizationError(ex.Message);
            }
        }

        void UpdateLocalizationError(string error)
        {
            if (m_ErrorHelpBox == null)
            {
                InitErrorHelpBox();
            }

            if (string.IsNullOrEmpty(error))
            {
                m_ErrorHelpBox.style.display = DisplayStyle.None;
                return;
            }

            m_ErrorHelpBox.Text = error;
            m_ErrorHelpBox.style.display = DisplayStyle.Flex;
        }

        void SelectedLang(string langNew)
        {
            var languages = LocalizationClient.Default.Languages;
            if (langNew != LocalizationClient.Default.CurrentLanguage)
            {
                LocalizationClient.Default.SetLanguage(langNew);
            }

            if (languages.Any())
            {
                m_ListLang.Clear();
                foreach (var lang in languages)
                {
                    var but = new Button { text = $"{lang}" };
                    but.AddToClassList(lang == langNew ? "lang-element-selected" : "lang-element");
                    but.clicked += () =>
                    {
                        SelectedLang(lang);
                    };
                    m_ListLang.Add(but);
                }
            }
        }

        void InitErrorHelpBox()
        {
            m_ErrorHelpBox = new HelpBox { MessageType = MessageType.Error };
            m_ErrorHelpBox.style.display = DisplayStyle.None;
            m_ErrorHelpBox.AddToClassList("error-message");
            m_Root.Add(m_ErrorHelpBox);
        }
    }
}
