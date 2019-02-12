using Citolab.QConstruction.Backend.Exports.QTI.CustomClasses;

namespace Citolab.QConstruction.Backend.Exports.Interfaces
{
    /// <summary>
    ///     Interface for converting tests
    /// </summary>
    public interface IConvertTest
    {
        /// <summary>
        ///     Convert to qti test
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        string Convert(QtiTest test);
    }
}