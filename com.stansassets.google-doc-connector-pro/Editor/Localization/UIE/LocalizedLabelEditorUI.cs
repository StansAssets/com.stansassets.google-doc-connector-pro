using System;
using System.Collections.Generic;
using System.Linq;
using StansAssets.GoogleDoc.Localization;
using StansAssets.Plugins.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using HelpBox = StansAssets.Foundation.UIElements.HelpBox;

namespace StansAssets.GoogleDoc.Editor
{
    class LocalizedLabelEditorUI : BaseTab
    {
        readonly VisualElement m_Root;
        VisualElement m_ListLang;
        HelpBox m_ErrorHelpBox;
        PopupField<string> m_SectionPopupField;
        PopupField<string> m_TokenPopupField;

        readonly LocalizedLabel m_LocalizedLabel;
        readonly SerializedObject m_SerializedObject;
        readonly SerializedProperty m_SectionProperty;
        readonly SerializedProperty m_TokenProperty;

        const string k_SectionPropertyPath = "m_Token.m_Section";
        const string k_TokenIdPropertyPath = "m_Token.m_TokenId";

        public LocalizedLabelEditorUI(LocalizedLabel localizedLabel, SerializedObject serializedObject)
            : base($"{GoogleDocConnectorPackage.UILocalizationPath}/LocalizedLabelEditorUI")
        {
            m_LocalizedLabel = localizedLabel;
            m_SerializedObject = serializedObject;
            m_SectionProperty = serializedObject.FindProperty(k_SectionPropertyPath);
            m_TokenProperty = serializedObject.FindProperty(k_TokenIdPropertyPath);
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
            try
            {
                CreateListSection();
                CreateListToken();
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

        void CreateListSection()
        {
            var values = LocalizationClient.Default.Sections;
            string selectedSectionName = m_SectionProperty.stringValue;
            if (!values.Contains(selectedSectionName))
            {
                selectedSectionName = values.First();
            }

            UpdateSectionProperty(selectedSectionName);
            PropertyPopup(out m_SectionPopupField, "Section", k_SectionPropertyPath, LocalizationClient.Default.Sections, OnSectionPopupChanged);
            m_Root.Add(m_SectionPopupField);
        }

        void CreateListToken()
        {
            Spreadsheet localizationSpreadsheet = GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId);
            Sheet newSheet = localizationSpreadsheet.GetSheet(m_SectionProperty.stringValue);

            var columnValues = newSheet.GetColumnValues<string>(0);
            columnValues.RemoveAt(0);

            string selectedTokenName = m_TokenProperty.stringValue;
            if (!columnValues.Contains(selectedTokenName))
            {
                selectedTokenName = columnValues.First();
            }

            UpdateTokenProperty(selectedTokenName);

            PropertyPopup(out m_TokenPopupField, "Token Id", k_TokenIdPropertyPath, columnValues, OnTokenPopupChanged);
            m_Root.Add(m_TokenPopupField);
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

        void PropertyPopup(out PopupField<string> createdPopupField, string propertyName, string bindingPath, List<string> values, Action<ChangeEvent<string>> onEventChanged)
        {
            createdPopupField = new PopupField<string>(propertyName, values, 0) { bindingPath = bindingPath };
            createdPopupField.RegisterCallback<ChangeEvent<string>>(ev => onEventChanged?.Invoke(ev));
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
                DisplayTokenPopupField(DisplayStyle.Flex);
                return;
            }

            m_ErrorHelpBox.Text = error;
            m_ErrorHelpBox.style.display = DisplayStyle.Flex;
            DisplayTokenPopupField(DisplayStyle.None);
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

        void OnSectionPopupChanged(ChangeEvent<string> changeEvent)
        {
            m_SectionPopupField.value = changeEvent.newValue;
            UpdateSectionProperty(changeEvent.newValue);
            RefreshChoices();
        }

        void OnTokenPopupChanged(ChangeEvent<string> changeEvent)
        {
            m_TokenPopupField.value = changeEvent.newValue;
            UpdateTokenProperty(changeEvent.newValue);
            RefreshChoices();
        }

        void UpdateSectionProperty(string newValue)
        {
            m_SerializedObject.Update();
            m_SectionProperty.stringValue = newValue;
            m_SerializedObject.ApplyModifiedProperties();
        }

        void UpdateTokenProperty(string newValue)
        {
            m_SerializedObject.Update();
            m_TokenProperty.stringValue = newValue;
            m_SerializedObject.ApplyModifiedProperties();
        }

        void RefreshChoices()
        {
            try
            {
                Spreadsheet localizationSpreadsheet = GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId);
                Sheet newSheet = localizationSpreadsheet.GetSheet(m_SectionProperty.stringValue);
                var newChoices = newSheet.GetColumnValues<string>(0);

                newChoices.RemoveAt(0);
                if (newChoices.All(x => x == string.Empty))
                {
                    throw new InvalidOperationException("There are no filled tokens on the selected sheet");
                }

                m_TokenPopupField.RefreshChoices(newChoices);

                string choice = m_TokenProperty.stringValue;
                if (!newChoices.Contains(choice) || string.IsNullOrEmpty(choice))
                {
                    choice = newChoices.First();
                }

                m_TokenPopupField.value = choice;
                m_TokenProperty.stringValue = choice;
                UpdateTokenProperty(choice);
                UpdateLocalization();
            }
            catch (Exception e)
            {
                UpdateLocalizationError(e.Message);
            }
        }

        void DisplayTokenPopupField(DisplayStyle displayStyle)
        {
            if (m_TokenPopupField == null) return;
            m_TokenPopupField.style.display = displayStyle;
        }
    }
}
