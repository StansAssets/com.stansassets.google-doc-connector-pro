using UnityEngine;
using UnityEngine.Serialization;

namespace StansAssets.GoogleDoc.Localization
{
    public class ILocalizationLable: MonoBehaviour
    {
        [SerializeField]
       public string Token = "token";

        [SerializeField]
        public string Section = default;

        [SerializeField]
        public TextType TextType = TextType.Default;

        [SerializeField]
        public string Prefix = default;

        [SerializeField]
        public string Suffix = default;
    }
}
