using UnityEngine;

namespace StansAssets.GoogleDoc.Localization
{
    public interface ILocalizationToken
    {
        string Token { get; }
        string Section { get; }
        TextType TextType { get; }
        string Prefix { get; }
        string Suffix { get; }
    }
}
