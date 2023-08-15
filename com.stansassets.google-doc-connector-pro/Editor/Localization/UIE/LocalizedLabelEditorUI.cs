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

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEventHandler, TrickleDown.TrickleDown);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEventHandler, TrickleDown.TrickleDown);
        }

        ~LocalizedLabelEditorUI()
        {
            UnregisterCallback<AttachToPanelEvent>(OnAttachToPanelEventHandler, TrickleDown.TrickleDown);
            UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEventHandler, TrickleDown.TrickleDown);
        }

        void OnAttachToPanelEventHandler(AttachToPanelEvent e)
        {
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

        void OnDetachFromPanelEventHandler(DetachFromPanelEvent e)
        {
            GoogleDocConnectorLocalization.SpreadsheetIdChanged -= Bind;
            try
            {
                LocalizationClient.Default.OnLanguageChanged -= UpdateLocalization;
            }
            catch (Exception ex)
            {
                UpdateLocalizationError(ex.Message);
            }
        }

        void Bind()
        {
            m_Root.Clear();
            try
            {
                CheckLocalizationCacheFile();
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
            try
            {
                if (m_TokenPopupField != null && m_Root.Contains(m_TokenPopupField))
                {
                    m_TokenPopupField.Clear();
                    m_Root.Remove(m_TokenPopupField);
                }

                Spreadsheet localizationSpreadsheet = GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId);
                Sheet newSheet = localizationSpreadsheet.GetSheet(m_SectionProperty.stringValue);

                var columnValues = newSheet.GetColumnValues<string>(0);
                columnValues.RemoveAt(0);
                if (columnValues.All(x => x == string.Empty))
                {
                    throw new InvalidOperationException("There are no filled tokens on the selected sheet");
                }

                string selectedTokenName = m_TokenProperty.stringValue;
                if (!columnValues.Contains(selectedTokenName))
                {
                    selectedTokenName = columnValues.First();
                }

                PropertyPopup(out m_TokenPopupField, "Token Id", k_TokenIdPropertyPath, columnValues, OnTokenPopupChanged);
                m_Root.Insert(1, m_TokenPopupField);

                OnTokenPopupChanged(selectedTokenName);
            }
            catch (Exception e)
            {
                UpdateLocalizationError(e.Message);
            }
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

        void PropertyPopup(out PopupField<string> createdPopupField, string propertyName, string bindingPath, List<string> values, Action<string> onValueChanged = null)
        {
            createdPopupField = new PopupField<string>(propertyName, values, 0) { bindingPath = bindingPath };
            createdPopupField.RegisterCallback<ChangeEvent<string>>(ev => onValueChanged?.Invoke(ev.newValue));
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
            InitErrorHelpBox();

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
            if (m_Root.Contains(m_ErrorHelpBox))
            {
                m_Root.Remove(m_ErrorHelpBox);
                m_ErrorHelpBox.Clear();
            }

            m_ErrorHelpBox = new HelpBox { MessageType = MessageType.Error };
            m_ErrorHelpBox.style.display = DisplayStyle.None;
            m_ErrorHelpBox.AddToClassList("error-message");
            m_Root.Add(m_ErrorHelpBox);
        }

        void OnSectionPopupChanged(string newValue)
        {
            m_SectionPopupField.value = newValue;
            UpdateSectionProperty(newValue);
            CreateListToken();
        }

        void OnTokenPopupChanged(string newValue)
        {
            m_TokenPopupField.value = newValue;
            UpdateTokenProperty(newValue);
            UpdateLocalization();
        }

        void UpdateSectionProperty(string newValue)
        {
            m_SerializedObject.Update();
            m_SectionProperty.stringValue = newValue;
            m_SerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(m_LocalizedLabel);
        }

        void UpdateTokenProperty(string newValue)
        {
            m_SerializedObject.Update();
            m_TokenProperty.stringValue = newValue;
            m_SerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(m_LocalizedLabel);
        }

        void DisplayTokenPopupField(DisplayStyle displayStyle)
        {
            if (m_TokenPopupField == null) return;
            m_TokenPopupField.style.display = displayStyle;
        }

        void CheckLocalizationCacheFile()
        {
            if (string.IsNullOrEmpty(GoogleDocConnectorLocalization.SpreadsheetId)) return;

            Spreadsheet localizationSpreadsheet = GoogleDocConnector.GetSpreadsheet(GoogleDocConnectorLocalization.SpreadsheetId);
            if (!localizationSpreadsheet.IsSpreadsheetFileExist())
            {
                throw new InvalidOperationException("No cached localization file");
            }
        }
    }
}
