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
    [SerializeField]  Dropdown m_ValueDropdown;
    [SerializeField]  Dropdown m_LangDropdown;
    [SerializeField]  Dropdown m_RangeDropdown;
    [SerializeField]  InputField m_TokenInputField;
    [SerializeField]  InputField m_RowsInputField;
    [SerializeField]  InputField m_ColumsInputField;
    [SerializeField]  Text m_ValueText;
    [SerializeField]  Transform m_ParametrsParent;
    [SerializeField]  Transform m_ValueParent;
    [SerializeField]  string m_Id;

    private readonly List<string> m_Tokens = new List<string>() {"size", "weight", "bounce","colR","colB","colG"};//, "bounce","colR","colB","colG" };
    private readonly List<string> m_CellRanges = new List<string>() {"B2:C5","C6:D8" };
    List<string> m_NamedRanges = new List<string>();
    List<string> m_DropdownValues = new List<string>();
    Example m_Example;
    Spreadsheet m_Spreadsheet;
    LocalizationClient m_Client;
    GameObject m_Blueprint;

   
    void Start()
    {
        m_Example = FindObjectOfType<Example>();
        m_Spreadsheet =GoogleDocConnector.GetSpreadsheet(m_Id);
        var sheet = m_Spreadsheet.GetSheet("sheet");
        m_NamedRanges = sheet.NamedRanges.Select(n => n.Name).ToList();
      
        
        m_ValueDropdown.ClearOptions();
        m_ValueDropdown.AddOptions(m_NamedRanges);

        m_Blueprint = m_ParametrsParent.GetChild(0).gameObject;
        m_Blueprint.transform.SetParent(FindObjectOfType<Canvas>().transform); 
        m_Blueprint.SetActive((false));

        m_Client = LocalizationClient.Default;
        m_LangDropdown.ClearOptions();
        m_LangDropdown.AddOptions(m_Client.Languages);
       
        m_LangDropdown.onValueChanged.AddListener(PopulateEntries);
        m_ValueDropdown.onValueChanged.AddListener(PopulateValues);
        m_TokenInputField.onEndEdit.AddListener(AddToken);
        m_RowsInputField.onEndEdit.AddListener(GetRow);
        m_ColumsInputField.onEndEdit.AddListener(GetColumn);
        m_RangeDropdown.onValueChanged.AddListener(GetCellsInRange);
        
        PopulateEntries(0);
        PopulateValues(0);
       
    }

    private void OnDisable()
    {
        m_LangDropdown.onValueChanged.RemoveAllListeners();
        m_ValueDropdown.onValueChanged.RemoveAllListeners();
        m_TokenInputField.onEndEdit.RemoveAllListeners();
        m_RowsInputField.onEndEdit.RemoveAllListeners();
        m_ColumsInputField.onEndEdit.RemoveAllListeners();
        m_RangeDropdown.onValueChanged.RemoveAllListeners();
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
        var sheet = m_Spreadsheet.GetSheet("sheet"); 
        if (int.TryParse(value, out numericValue))
        {
            if (numericValue >= sheet.Rows.Count())
            {
                m_ValueText.text = string.Empty;
                throw new Exception("Entered number bigger them number of Rows");
            }
               
          
            var rowValues = sheet.GetRowValues<string>(numericValue);
            m_ValueText.text = string.Empty;
            m_ValueText.text = "Row №" + $"{numericValue} :"+"\n";
            foreach (var rowValue in rowValues)
            {
                m_ValueText.text+=" "+rowValue;
            }
        }
        else
        {
            m_ValueText.text = string.Empty;
            throw new Exception("Pls enter a number of a row you want to get");
        }
    }

    private void GetCellsInRange(int value)
    {
        if (value == 0)
        {
            m_ValueText.text = string.Empty;
            return;
        }
        var sheet = m_Spreadsheet.GetSheet("sheet");
        var cells = sheet.GetRange(m_CellRanges[value-1]);
        m_ValueText.text = string.Empty;
        foreach (var cell in cells)
        {
            m_ValueText.text += $" {cell.Value.FormattedValue}";
        }
    }
    
    private void GetColumn(string value)
    {
        if (value == string.Empty) return;
        int numericValue;
        var sheet = m_Spreadsheet.GetSheet("sheet");
       
        if (int.TryParse(value, out numericValue))
        {
           var columnValues = sheet.GetColumnValues<string>(numericValue);
          
           m_ValueText.text = string.Empty;
           m_ValueText.text = "Column №" + $"{numericValue} :";
           foreach (var rowValue in columnValues)
           {
               m_ValueText.text+="\n"+rowValue;
           }
        }
        else
        {
            var columnValues = sheet.GetColumnValues<string>(value);
            m_ValueText.text = string.Empty;
            m_ValueText.text = "Column №" + $"{numericValue} :";
            foreach (var rowValue in columnValues)
            {
                m_ValueText.text+="\n"+rowValue;
            }
        }
    }
    
    private void AddToken(string token)
    {
        if (token == string.Empty) return;
        
        var newToken = token;
        var newLoc = m_Client.GetLocalizedString(newToken);
        m_Tokens.Add(newToken);
        m_TokenInputField.text = String.Empty;
        PopulateEntries(m_LangDropdown.value);
        PopulateValues(m_ValueDropdown.value);
    }
    private void PopulateEntries(int index)
    {
        for (int i = 0; i < m_ParametrsParent.childCount; i++)
        {
            Destroy((m_ParametrsParent.GetChild(i).gameObject));
        }
        
        m_Client.SetLanguage(m_Client.Languages[index]);
        
        foreach (var token in m_Tokens)
        {
            var newEntry=  Instantiate(m_Blueprint, m_ParametrsParent);
            Text entryText = newEntry.GetComponent<Text>();
            entryText.text = $"  {token} : {m_Client.GetLocalizedString(token)}";
            entryText.enabled = true;
            newEntry.SetActive(true);
           
        }
        
    }

    private void PopulateValues(int index)
    {
        for (int i = 0; i < m_ValueParent.childCount; i++)
        {
            Destroy((m_ValueParent.GetChild(i).gameObject));
        }
        var sheet = m_Spreadsheet.GetSheet("sheet"); 
        //var cells = sheet.GetNamedRangeValues<string>(m_NamedRanges[index]);
        var values =sheet.GetNamedRangeValues<float>(m_NamedRanges[index]);
        m_Example.SetValues(values);
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
            var newEntry=  Instantiate(m_Blueprint, m_ValueParent);
            Text entryText = newEntry.GetComponent<Text>();
            entryText.text = values[i].ToString("N");
            entryText.enabled = true;
            newEntry.SetActive(true);
        }
        
        
    }
    
    
}
