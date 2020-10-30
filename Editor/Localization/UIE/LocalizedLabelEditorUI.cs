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
        
        public LocalizedLabelEditorUI(LocalizedLabel localizedLabel)
            : base($"{GoogleDocConnectorPackage.UILocalizationPath}/LocalizedLabelEditorUI")
        {
            m_LocalizedLabel = localizedLabel;
            m_Root = this.Q<VisualElement>("LocalizedLabelEditorRoot");
            GoogleDocConnectorLocalization.SpreadsheetIdChanged += Bind;
            try
            {
                LocalizationClient.Default.OnLanguageChanged += () =>
                {
                    Bind();
                    UpdateLocalization("", "");
                };
            }
            catch
            {
                UpdateLocalizationError("There are errors in LocalizationClient, more detailed on Localization Tab");
                return;
            }
            Bind();
        }

        void Bind()
        {
            m_Root.Clear();
            PropertyField("Token Id", m_LocalizedLabel.m_Token.Token);
            try
            {
                var values = LocalizationClient.Default.Sheets;
                if (!values.Contains(m_LocalizedLabel.m_Token.Section))
                {
                    m_LocalizedLabel.m_Token.SectionSet(values.First());
                }
                PropertyPopup("Section", m_LocalizedLabel.m_Token.Section, LocalizationClient.Default.Sheets);
            }
            catch
            {
                m_Root.Clear();
                UpdateLocalizationError("There are errors in LocalizationClient, more detailed on Localization Tab");
                return;
            }
            PropertyPopup("Text Type", m_LocalizedLabel.m_Token.TextType.ToString(), Enum.GetNames(typeof(TextType)).ToList());
            PropertyField("Prefix", m_LocalizedLabel.m_Token.Prefix);
            PropertyField("Suffix", m_LocalizedLabel.m_Token.Suffix);
            InitErrorHelpBox();
            var labelLang = new Label() { text= "Available Languages:"};
            labelLang.AddToClassList("header-lang");
            m_Root.Add(labelLang);
            m_ListLang = new VisualElement();
            m_ListLang.AddToClassList("list-lang");
            m_Root.Add(m_ListLang);
            SelectedLang(LocalizationClient.Default.CurrentLanguage);
        }

        void PropertyField(string nameProperty, string property)
        {
            var propertyField = new TextField(nameProperty) { value = property };
            propertyField.RegisterCallback<ChangeEvent<string>>((ev) =>
            {
                UpdateLocalization(nameProperty, ev.newValue);
            });
            m_Root.Add(propertyField);
        }
        void PropertyPopup(string nameProperty, string property, List<string> values)
        {
            var propertyField = new PopupField<string>(nameProperty, values, 0) { value = property };
            propertyField.RegisterCallback<ChangeEvent<string>>((ev) =>
            {
                UpdateLocalization(nameProperty, ev.newValue);
            });
            m_Root.Add(propertyField);
        }

        void UpdateLocalization(string nameProperty, string newValue)
        {
            switch (nameProperty)
            {
                case "Token Id":
                    m_LocalizedLabel.m_Token.TokenSet(newValue);
                    break;
                case "Section":
                    m_LocalizedLabel.m_Token.SectionSet(newValue);
                    break;
                case "Text Type":
                    m_LocalizedLabel.m_Token.TextTypeSet((TextType)Enum.Parse(typeof(TextType), newValue));
                    break;
                case "Prefix":
                    m_LocalizedLabel.m_Token.PrefixSet(newValue);
                    break;
                case "Suffix":
                    m_LocalizedLabel.m_Token.SuffixSet(newValue);
                    break;
            }
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
