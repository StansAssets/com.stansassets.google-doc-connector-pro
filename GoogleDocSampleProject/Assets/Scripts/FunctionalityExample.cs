using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StansAssets.GoogleDoc;
using StansAssets.GoogleDoc.Example;
using StansAssets.GoogleDoc.Localization;
using UnityEngine;
using UnityEngine.UI;

public class FunctionalityExample : MonoBehaviour
{
    
    [Header("UI references")]
    [SerializeField]  Dropdown m_valueDropdown;
    [SerializeField]  Dropdown m_langDropdown;
    [SerializeField]  Dropdown m_rangeDropdown;
    [SerializeField]  InputField m_tokenInputField;
    [SerializeField]  InputField m_rowsInputField;
    [SerializeField]  InputField m_columsInputField;
    [SerializeField]  Text m_valueText;
    [SerializeField]  Transform m_parametrsParent;
    [SerializeField]  Transform m_valueParent;
    [SerializeField]  string m_id;

    private readonly List<string> m_Tokens = new List<string>() {"size", "weight", "bounce","colR","colB","colG"};//, "bounce","colR","colB","colG" };
    private readonly List<string> m_CellRanges = new List<string>() {"B2:C5","C6:D8" };
    List<string> m_NamedRanges = new List<string>();
    List<string> m_dropdownValues = new List<string>();
    Example m_example;
    Spreadsheet m_spreadsheet;
    LocalizationClient m_Client;
    GameObject m_blueprint;

   
    void Start()
    {
        m_example = FindObjectOfType<Example>();
        m_spreadsheet =GoogleDocConnector.GetSpreadsheet(m_id);
        var sheet = m_spreadsheet.GetSheet("sheet");
        m_NamedRanges = sheet.NamedRanges.Select(n => n.Name).ToList();
      
        
        m_valueDropdown.ClearOptions();
        m_valueDropdown.AddOptions(m_NamedRanges);

        m_blueprint = m_parametrsParent.GetChild(0).gameObject;
        m_blueprint.transform.SetParent(FindObjectOfType<Canvas>().transform); 
        m_blueprint.SetActive((false));

        m_Client = LocalizationClient.Default;
        m_langDropdown.ClearOptions();
        m_langDropdown.AddOptions(m_Client.Languages);
       
        m_langDropdown.onValueChanged.AddListener(PopulateEntries);
        m_valueDropdown.onValueChanged.AddListener(PopulateValues);
        m_tokenInputField.onEndEdit.AddListener(AddToken);
        m_rowsInputField.onEndEdit.AddListener(GetRow);
        m_columsInputField.onEndEdit.AddListener(GetColumn);
        m_rangeDropdown.onValueChanged.AddListener(GetCellsInRange);
        
        PopulateEntries(0);
        PopulateValues(0);
       
    }

    private void OnDisable()
    {
        m_langDropdown.onValueChanged.RemoveAllListeners();
        m_valueDropdown.onValueChanged.RemoveAllListeners();
        m_tokenInputField.onEndEdit.RemoveAllListeners();
        m_rowsInputField.onEndEdit.RemoveAllListeners();
        m_columsInputField.onEndEdit.RemoveAllListeners();
        m_rangeDropdown.onValueChanged.RemoveAllListeners();
    }
    // private void OnDestroy()
    // {
    //     m_langDropdown.onValueChanged.RemoveAllListeners();
    //     m_valueDropdown.onValueChanged.RemoveAllListeners();
    //     m_tokenInputField.onEndEdit.RemoveAllListeners();
    //     m_rowsInputField.onEndEdit.RemoveAllListeners();
    //     m_columsInputField.onEndEdit.RemoveAllListeners();
    //     m_rangeDropdown.onValueChanged.RemoveAllListeners();
    // }

    private void GetRow(string value)
    {
        int numericValue;
        var sheet = m_spreadsheet.GetSheet("sheet"); 
        if (int.TryParse(value, out numericValue))
        {
            if (numericValue >= sheet.Rows.Count())
            {
                m_valueText.text = string.Empty;
                throw new Exception("Entered number bigger them number of Rows");
            }
               
          
            var rowValues = sheet.GetRowValues<string>(numericValue);
            m_valueText.text = string.Empty;
            m_valueText.text = "Row №" + $"{numericValue} :"+"\n";
            foreach (var rowValue in rowValues)
            {
                m_valueText.text+=" "+rowValue;
            }
        }
        else
        {
            m_valueText.text = string.Empty;
            throw new Exception("Pls enter a number of a row you want to get");
        }
    }

    private void GetCellsInRange(int value)
    {
        if (value == 0)
        {
            m_valueText.text = string.Empty;
            return;
        }
        var sheet = m_spreadsheet.GetSheet("sheet");
        var cells = sheet.GetRange(m_CellRanges[value-1]);
        m_valueText.text = string.Empty;
        foreach (var cell in cells)
        {
            m_valueText.text += $" {cell.Value.FormattedValue}";
        }
    }
    
    private void GetColumn(string value)
    {
        if (value == string.Empty) return;
        int numericValue;
        var sheet = m_spreadsheet.GetSheet("sheet");
       
        if (int.TryParse(value, out numericValue))
        {
           var columnValues = sheet.GetColumnValues<string>(numericValue);
          
           m_valueText.text = string.Empty;
           m_valueText.text = "Column №" + $"{numericValue} :";
           foreach (var rowValue in columnValues)
           {
               m_valueText.text+="\n"+rowValue;
           }
        }
        else
        {
            var columnValues = sheet.GetColumnValues<string>(value);
            m_valueText.text = string.Empty;
            m_valueText.text = "Column №" + $"{numericValue} :";
            foreach (var rowValue in columnValues)
            {
                m_valueText.text+="\n"+rowValue;
            }
        }
    }
    
    private void AddToken(string token)
    {
        if (token == string.Empty) return;
        
        var newToken = token;
        var newLoc = m_Client.GetLocalizedString(newToken);
        m_Tokens.Add(newToken);
        m_tokenInputField.text = String.Empty;
        PopulateEntries(m_langDropdown.value);
        PopulateValues(m_valueDropdown.value);
    }
    private void PopulateEntries(int index)
    {
        for (int i = 0; i < m_parametrsParent.childCount; i++)
        {
            Destroy((m_parametrsParent.GetChild(i).gameObject));
        }
        
        m_Client.SetLanguage(m_Client.Languages[index]);
        
        foreach (var token in m_Tokens)
        {
            var newEntry=  Instantiate(m_blueprint, m_parametrsParent);
            Text entryText = newEntry.GetComponent<Text>();
            entryText.text = $"  {token} : {m_Client.GetLocalizedString(token)}";
            entryText.enabled = true;
            newEntry.SetActive(true);
           
        }
        
    }

    private void PopulateValues(int index)
    {
        for (int i = 0; i < m_valueParent.childCount; i++)
        {
            Destroy((m_valueParent.GetChild(i).gameObject));
        }
        var sheet = m_spreadsheet.GetSheet("sheet"); 
        //var cells = sheet.GetNamedRangeValues<string>(m_NamedRanges[index]);
        var values =sheet.GetNamedRangeValues<float>(m_NamedRanges[index]);
        m_example.SetValues(values);
        // foreach (var value in values)
        // {
        //     var newEntry=  Instantiate(m_blueprint, m_valueParent);
        //     Text entryText = newEntry.GetComponent<Text>();
        //     entryText.text = value.ToString("N");
        //     entryText.enabled = true;
        //     newEntry.SetActive(true);
        //
        // }

        for (int i = 0; i < m_Tokens.Count; i++)
        {
            var newEntry=  Instantiate(m_blueprint, m_valueParent);
            Text entryText = newEntry.GetComponent<Text>();
            entryText.text = values[i].ToString("N");
            entryText.enabled = true;
            newEntry.SetActive(true);
        }
        
        
    }

    
}
