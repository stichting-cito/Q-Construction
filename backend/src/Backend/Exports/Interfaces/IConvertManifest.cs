using Citolab.QConstruction.Backend.Exports.QTI.CustomClasses;

namespace Citolab.QConstruction.Backend.Exports.Interfaces
{
    /// <summary>
    ///     Convert manifest
    /// </summary>
    public interface IConvertManifest
    {
        /// <summary>
        ///     For now pass all the items and create a test.
        ///     In Q-Construction we dont know the entity test.
        ///     But to have a test can be handy to play in a player, otherwise its just an export method to an other construction
        ///     tool
        /// </summary>
        /// <param name="manifestItems"></param>
        /// <returns></returns>
        string Convert(ManifestItems manifestItems);
    }
}