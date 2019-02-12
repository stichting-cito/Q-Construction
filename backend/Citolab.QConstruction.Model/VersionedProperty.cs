using System.Collections.Generic;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Versioned property of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VersionedProperty<T> : List<PropertyVersion<T>>
    {
    }
}