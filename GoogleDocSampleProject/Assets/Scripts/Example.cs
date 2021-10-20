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
        [SerializeField] Button m_changeLanguage;
        [SerializeField] Text m_currentLanguage;
        [SerializeField] Text m_languagesArea;
        [SerializeField] Text m_wordsArea;
        [SerializeField] GameObject m_origin;
        [SerializeField] Transform m_originContainer;
         //test
       
        
        private List<float> _values = new List<float>();
        //tes
        ObjectPool<SphereController> m_spherePool;
        LocalizationClient m_client;
        const float MINUTE = 60;
        int m_objectsCount = 20; //TODO from the google doc
        float m_scale = 1f;//TODO from the google doc
        int m_currentLanguageIndex;
        int m_totalSpawnedCount;
        float m_spawnDelay;
        float m_time;

        void Awake()
        {
            m_spherePool = new ObjectPool<SphereController>(
                () => {
                    var go = Instantiate(m_origin, m_originContainer);
                    go.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
                    go.transform.position = m_origin.transform.position;
                    go.SetActive(true);
                    var sphereController = go.GetComponent<SphereController>();
                    sphereController.Release += () => { m_spherePool.Release(sphereController); };
                    return sphereController;
                },
                sphereController => {
                    sphereController.gameObject.SetActive(true);
                },
                sphereController => {
                    sphereController.gameObject.SetActive(false);
                    sphereController.transform.position = m_origin.transform.position;
                }
            );
        }

        void Start()
        {
          /*  m_client = LocalizationClient.Default;
            m_changeLanguage.onClick.AddListener(ChangeLanguage);
            PrintExistedLanguages();
            PrintLocalizedToken();
            GetConfig();*/
            m_spawnDelay = MINUTE / m_objectsCount;
        }
        void Update()
        {
            m_time += Time.deltaTime;
            if (m_time > m_spawnDelay/* && totalSpawnedCount != m_count*/) {
                m_time = 0;
                InstantiateSphere();
            }
        }

        private void InstantiateSphere() {
          var sphere=  m_spherePool.Get();
           // var spreadsheet = GoogleDocConnector.GetSpreadsheet(_id);
           // var sheet = spreadsheet.GetSheet("sheet");
           // var values = sheet.GetNamedRangeValues<int>("Default");
          sphere.SetUpBall(_values);
            m_totalSpawnedCount++;
           /* if (totalSpawnedCount == m_count) {
                totalSpawnedCount = 0;
            }*/
        }

        void GetConfig() {
            //TODO parse config sheet
            // var a = m_client.Sections["Config"];
        }

        void ChangeLanguage()
        {
            if (m_client.Languages.Count == 0)
            {
                Debug.LogError($"Please add at least one language into the Stan's Assets -> Google Doc Connector -> Settings");
                return;
            }

            m_currentLanguageIndex++;
            if (m_currentLanguageIndex > m_client.Languages.Count - 1)
            {
                m_currentLanguageIndex = 0;
            }

            m_client.SetLanguage(m_client.Languages[m_currentLanguageIndex]);
            m_currentLanguage.text = m_client.Languages[m_currentLanguageIndex];
            PrintLocalizedToken();
        }

        void PrintExistedLanguages()
        {
            m_languagesArea.text = CombineWords(m_client.Languages);
            m_currentLanguageIndex = 0;
            m_currentLanguage.text = m_client.Languages[0];
        }

        void PrintLocalizedToken()
        {
            m_wordsArea.text = m_client.GetLocalizedString("table_token");
        }

        string CombineWords(IEnumerable<string> words) =>
            words.Aggregate(string.Empty, (word, next) => string.Concat(word, next, "\n"));
        
        //test
        public void SetValues(List<float> values)
        {
            if (_values.Count > 0)
            {
                _values.Clear();
            }

            for (int i = 0; i < values.Count; i++)
            {
                
                _values.Add(values[i]);
            }

            m_spawnDelay = _values.Last();
        }
    }
}