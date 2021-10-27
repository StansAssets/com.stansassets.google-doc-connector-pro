using System;
using System.Collections.Generic;
using System.Linq;
using StansAssets.Foundation.Patterns;
using UnityEngine;
using UnityEngine.UI;
using StansAssets.GoogleDoc.Localization;


namespace StansAssets.GoogleDoc.Example
{
    public class Example : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField] Dropdown m_ValueDropdown;
        [SerializeField] Dropdown m_LangDropdown;
        [SerializeField] Dropdown m_RangeDropdown;
        [SerializeField] InputField m_TokenInputField;
        [SerializeField] InputField m_RowsInputField;
        [SerializeField] InputField m_ColumnsInputField;
        [SerializeField] InputField m_RangeInputField;
        [SerializeField] Text m_ValueText;
        [SerializeField] Transform m_ParametersParent;
        [SerializeField] Transform m_ValueParent;
        [SerializeField] string m_Id;
        [Header("Sphere spawn reference")]
        [SerializeField] GameObject m_Origin;
        [SerializeField] Transform m_OriginContainer;
        List<float> m_Values = new List<float>();
        List<string> m_Tokens = new List<string>();
        readonly List<string> m_CellRanges = new List<string>() {"B2:C5", "C6:D7"};
        List<string> m_NamedRanges = new List<string>();
        Example m_Example;
        Spreadsheet m_Spreadsheet;
        LocalizationClient m_Client;
        GameObject m_Blueprint;
        int m_RowsCount;
        ObjectPool<SphereController> m_SpherePool;
        float m_Scale = 1f;
        int m_CurrentLanguageIndex;
        float m_SpawnDelay;
        float m_Time;

        void Awake()
        {
            m_SpherePool = new ObjectPool<SphereController>(
                () =>
                {
                    var go = Instantiate(m_Origin, m_OriginContainer);
                    go.transform.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
                    go.transform.position = m_Origin.transform.position;
                    go.SetActive(true);
                    var sphereController = go.GetComponent<SphereController>();
                    sphereController.Release += () => { m_SpherePool.Release(sphereController); };
                    return sphereController;
                },
                sphereController =>
                {
                    sphereController.gameObject.SetActive(true);
                },
                sphereController =>
                {
                    sphereController.gameObject.SetActive(false);
                    sphereController.transform.position = m_Origin.transform.position;
                }
            );
        }

        void Start()
        {
            m_Spreadsheet = GoogleDocConnector.GetSpreadsheet(m_Id);
            var sheet = m_Spreadsheet.GetSheet("BallConfig");
            var langSheet = m_Spreadsheet.GetSheet("Localization");
            m_RowsCount = sheet.Rows.Count();
            m_Tokens = new List<string>(langSheet.GetColumnValues<string>(0));
            m_Tokens.RemoveAt(0);
            m_NamedRanges = sheet.NamedRanges.Select(n => n.Name).ToList();
            m_ValueDropdown.ClearOptions();
            m_ValueDropdown.AddOptions(m_NamedRanges);
            m_Blueprint = m_ParametersParent.GetChild(0).gameObject;
            m_Blueprint.transform.SetParent(GetComponent<Canvas>().transform);
            m_Blueprint.SetActive((false));
            m_Client = LocalizationClient.Default;
            m_LangDropdown.ClearOptions();
            m_LangDropdown.AddOptions(m_Client.Languages);
            m_LangDropdown.onValueChanged.AddListener(PopulateEntries);
            m_ValueDropdown.onValueChanged.AddListener(PopulateValues);
            m_TokenInputField.onEndEdit.AddListener(AddToken);
            m_RowsInputField.onValueChanged.AddListener(GetRow);
            m_ColumnsInputField.onValueChanged.AddListener(GetColumn);
            m_RangeDropdown.onValueChanged.AddListener(GetCellsInRange);
            m_RangeInputField.onEndEdit.AddListener(GetValuesInRange);
            PopulateEntries(0);
            PopulateValues(0);
        }

        void OnDestroy()
        {
            m_LangDropdown.onValueChanged.RemoveListener(PopulateEntries);
            m_ValueDropdown.onValueChanged.RemoveListener(PopulateValues);
            m_TokenInputField.onEndEdit.RemoveListener(AddToken);
            m_RowsInputField.onEndEdit.RemoveListener(GetRow);
            m_ColumnsInputField.onEndEdit.RemoveListener(GetColumn);
            m_RangeDropdown.onValueChanged.RemoveListener(GetCellsInRange);
            m_RangeInputField.onValueChanged.RemoveListener(GetValuesInRange);
        }

        void Update()
        {
            m_Time += Time.deltaTime;
            if (m_Time > m_SpawnDelay)
            {
                m_Time = 0;
                InstantiateSphere();
            }
        }

        void InstantiateSphere()
        {
            var sphere = m_SpherePool.Get();
            sphere.SetUpBall(m_Values);
        }

        void GetRow(string value)
        {
            if (value.Equals(string.Empty))
            {
                m_ValueText.text = string.Empty;
                return;
            }

            int numericValue;
            var sheet = m_Spreadsheet.GetSheet("BallConfig");
            if (int.TryParse(value, out numericValue))
            {
                if (numericValue >= m_RowsCount)
                {
                    m_ValueText.text = "Entered number is bigger then the number of Rows";
                    return;
                }

                var rowValues = sheet.GetRowValues<string>(numericValue);
                m_ValueText.text = string.Empty;
                m_ValueText.text = "Row №" + $"{numericValue} :" + "\n";
                foreach (var rowValue in rowValues)
                {
                    m_ValueText.text += " " + rowValue;
                }
            }
        }

        void GetCellsInRange(int value)
        {
            if (value == 0)
            {
                m_ValueText.text = string.Empty;
                return;
            }

            var sheet = m_Spreadsheet.GetSheet("BallConfig");
            var cells = sheet.GetRange(m_CellRanges[value - 1]);
            m_ValueText.text = string.Empty;
            foreach (var cell in cells)
            {
                m_ValueText.text += $" {cell.Value.FormattedValue}";
            }
        }

        void GetColumn(string value)
        {
            if (value.Equals(string.Empty))
            {
                m_ValueText.text = string.Empty;
                return;
            }

            int numericValue;
            var sheet = m_Spreadsheet.GetSheet("BallConfig");
            if (int.TryParse(value, out numericValue))
            {
                var columnValues = sheet.GetColumnValues<string>(numericValue);
                if (columnValues.Count == 0)
                {
                    m_ValueText.text = $"Spreadsheet doesnt contain column № {numericValue}";
                    return;
                }

                m_ValueText.text = string.Empty;
                m_ValueText.text = "Column №" + $"{numericValue} :";
                foreach (var rowValue in columnValues)
                {
                    m_ValueText.text += "\n" + rowValue;
                }
            }
            else
            {
                var columnValues = sheet.GetColumnValues<string>(value);
                if (columnValues.Count == 0)
                {
                    m_ValueText.text = $"Spreadsheet doesnt contain column {value.ToUpperInvariant()}";
                    return;
                }

                m_ValueText.text = string.Empty;
                m_ValueText.text = "Column " + $"{value.Substring(0, 1).ToUpperInvariant()} :";
                foreach (var columnValue in columnValues)
                {
                    m_ValueText.text += "\n" + columnValue;
                }
            }
        }

        void AddToken(string token)
        {
            if (token.Equals(string.Empty))
            {
                return;
            }

            if (!m_Tokens.Contains(token))
            {
                m_Tokens.Add(token);
                m_TokenInputField.text = String.Empty;
                PopulateEntries(m_LangDropdown.value);
                PopulateValues(m_ValueDropdown.value);
            }
            else
            {
                m_TokenInputField.text = String.Empty;
            }
        }

        void PopulateEntries(int index)
        {
            for (int i = 0; i < m_ParametersParent.childCount; i++)
            {
                Destroy((m_ParametersParent.GetChild(i).gameObject));
            }

            m_Client.SetLanguage(m_Client.Languages[index]);
            foreach (var token in m_Tokens)
            {
                var newEntry = Instantiate(m_Blueprint, m_ParametersParent);
                var entryText = newEntry.GetComponent<Text>();
                entryText.text = $"  {token} : {m_Client.GetLocalizedString(token)}";
                entryText.enabled = true;
                newEntry.SetActive(true);
            }
        }

        void PopulateValues(int index)
        {
            for (int i = 0; i < m_ValueParent.childCount; i++)
            {
                Destroy((m_ValueParent.GetChild(i).gameObject));
            }

            var sheet = m_Spreadsheet.GetSheet("BallConfig");
            m_Values = sheet.GetNamedRangeValues<float>(m_NamedRanges[index]);
            m_SpawnDelay = m_Values[m_Values.Count - 1];
            for (int i = 0; i < m_Tokens.Count; i++)
            {
                var newEntry = Instantiate(m_Blueprint, m_ValueParent);
                var entryText = newEntry.GetComponent<Text>();
                entryText.text = m_Values[i].ToString("");
                entryText.enabled = true;
                newEntry.SetActive(true);
            }
        }

        void GetValuesInRange(string range)
        {
            if (range.Equals(string.Empty))
            {
                m_ValueText.text = string.Empty;
                return;
            }

            var sheet = m_Spreadsheet.GetSheet("BallConfig");
            var cells = sheet.GetRange(range);
            m_ValueText.text = string.Empty;
            foreach (var cell in cells)
            {
                m_ValueText.text += $" {cell.Value.FormattedValue}";
            }
        }
        
    }
}