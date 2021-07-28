using System;
using Newtonsoft.Json;
using UnityEngine;

namespace StansAssets.GoogleDoc
{
    /// <summary>
    /// Sheet metadata
    /// </summary>
    [Serializable]
    public class SheetMetadata
    {
        [SerializeField]
        protected string m_Name;

        /// <summary>
        /// The name of the sheet.
        /// </summary>
        public string Name => m_Name;

        [SerializeField]
        protected int m_Id;

        /// <summary>
        /// The ID of the sheet.
        /// </summary>
        public int Id => m_Id;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The ID of the sheet.</param>
        /// <param name="name">The name of the sheet.</param>
        [JsonConstructor]
        public SheetMetadata(int id, string name)
        {
            m_Id = id;
            m_Name = name;
        }
    }
}
