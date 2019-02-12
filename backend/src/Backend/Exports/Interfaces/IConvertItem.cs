using Citolab.QConstruction.Model;

namespace Citolab.QConstruction.Backend.Exports.Interfaces
{
    /// <summary>
    ///     Convert item
    /// </summary>
    internal interface IConvertItem
    {
        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string Convert(Item item);
    }
}