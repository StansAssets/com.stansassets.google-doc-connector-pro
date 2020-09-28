using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace StansAssets.GoogleDoc.Samples
{
    public class GoogleDocSample : MonoBehaviour
    {
        public const string SpreadsheetId = "1b_qGZuE5iy9fkK0QoXMObEigJPhuz7OZu27DDbEvUOo";
        List<string> m_NamedRanges = new List<string>();
        [FormerlySerializedAs("PanelID")]
        public GameObject PanelId;
        public Dropdown Dropdown;
        List<GameObject> Balls;

        // Start is called before the first frame update
        void Start()
        {
            try
            {
                var spreadsheet = GoogleDocConnector.GetSpreadsheet(SpreadsheetId);
                var sheet = spreadsheet.GetSheet("Sample");
                m_NamedRanges = sheet.NamedRanges.Select(n => n.Name).ToList();
                m_NamedRanges.Sort();
            }
            catch
            {
                PanelId.SetActive(true);
            }
            
            var whiteBall = GameObject.Find("WhiteBall");
            Balls.Add(whiteBall);
            var blackBall = GameObject.Find("BlackBall");
            Balls.Add(blackBall);
            for (var index = 0; index < 5; index++)
            {
                Balls.Add(Instantiate(blackBall));
            }

            Dropdown.ClearOptions();
            Dropdown.AddOptions(m_NamedRanges);
            Dropdown.onValueChanged.AddListener(DropdownChange);
            DropdownChange(0);
        }

        void DropdownChange(int value)
        {
            var spreadsheet = GoogleDocConnector.GetSpreadsheet(SpreadsheetId);
            var sheet = spreadsheet.GetSheet("Sample");
            var cells = sheet.GetNamedRangeValues<int>(m_NamedRanges[value]);

            var xIndex = 0;
            var yIndex = 1;
            foreach (var ball in Balls)
            {
                ball.transform.position = new Vector3(cells[xIndex], cells[yIndex], ball.transform.position.z);
                xIndex += 2;
                yIndex += 2;
            }
        }
        
    }
}
