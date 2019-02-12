using Citolab.QConstruction.Backend.Exports.Interfaces;
using Citolab.QConstruction.Backend.Exports.QTI.CustomClasses;

namespace Citolab.QConstruction.Backend.Exports.QTI
{
    /// <summary>
    ///     Creates a test base on a list of items
    /// </summary>
    public class Manifest : IConvertManifest
    {
        private readonly ViewRender _renderer;
        private readonly string _version;

        /// <summary>
        ///     Test
        /// </summary>
        public Manifest(ViewRender renderer, string version)
        {
            _renderer = renderer;
            _version = version;
        }

        /// <summary>
        ///     Convert or in this case create a test
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public string Convert(ManifestItems items) => _renderer.Render("Manifest", _version, items).Trim();
    }
}