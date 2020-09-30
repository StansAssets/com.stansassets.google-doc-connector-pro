using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace StansAssets.GoogleDoc.Samples
{
    public class GoogleDocSample : MonoBehaviour
    {
        public const string SpreadsheetId = "1b_qGZuE5iy9fkK0QoXMObEigJPhuz7OZu27DDbEvUOo";
        List<string> m_NamedRanges = new List<string>();
        [FormerlySerializedAs("PanelID")]
        public GameObject PanelId;
        public Dropdown Dropdown;
        public GameObject WhiteBall;
        public GameObject BlackBall;
        readonly List<GameObject> m_Balls = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            if (!GoogleDocConnector.HasSpreadsheet(SpreadsheetId))
            {
                PanelId.SetActive(true);
                return;
            }

            var spreadsheet = GoogleDocConnector.GetSpreadsheet(SpreadsheetId);
            if (!spreadsheet.HasSheet("Sample"))
            {
                PanelId.SetActive(true);
                return;
            }

            var sheet = spreadsheet.GetSheet("Sample");
            m_NamedRanges = sheet.NamedRanges.Select(n => n.Name).ToList();
            m_NamedRanges.Sort();

            m_Balls.Add(WhiteBall);
            m_Balls.Add(BlackBall);
            for (var index = 0; index < 5; index++)
            {
                m_Balls.Add(Instantiate(BlackBall));
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
            foreach (var ball in m_Balls)
            {
                ball.transform.position = new Vector3(cells[xIndex], cells[yIndex], ball.transform.position.z);
                xIndex += 2;
                yIndex += 2;
            }
        }
    }
}
