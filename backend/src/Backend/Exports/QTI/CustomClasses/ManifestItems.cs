using System;
using System.Collections.Generic;
using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Backend.Exports.QTI.CustomClasses
{
    /// <summary>
    ///     Used to fill the manifest
    /// </summary>
    public class ManifestItems
    {
        /// <summary>
        ///     List of depencies for items
        /// </summary>
        public Dictionary<string, HashSet<string>> Dependencies;

        public string TestId { get; set; }

        /// <summary>
        ///     Items
        /// </summary>
        public IList<Item> Items { get; set; }

        /// <summary>
        ///     Media files:
        /// </summary>
        public IList<string> Media { get; set; }

        /// <summary>
        ///     Css Files
        /// </summary>
        public IList<string> Css { get; set; }
    }
}