namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Property wit version
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyVersion<T>
    {
        /// <summary>
        ///     Version property
        /// </summary>
        public PropertyVersion()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="version"></param>
        /// <param name="value"></param>
        public PropertyVersion(int version, T value)
        {
            Version = version;
            Value = value;
        }

        /// <summary>
        ///     Version number.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        ///     The versioned property.
        /// </summary>
        public T Value { get; set; }
    }
}