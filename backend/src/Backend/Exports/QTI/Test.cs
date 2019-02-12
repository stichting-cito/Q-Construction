using Citolab.QConstruction.Backend.Exports.Interfaces;
using Citolab.QConstruction.Backend.Exports.QTI.CustomClasses;

namespace Citolab.QConstruction.Backend.Exports.QTI
{
    /// <summary>
    ///     Creates a test base on a list of items
    /// </summary>
    public class Test : IConvertTest
    {
        private readonly ViewRender _renderer;
        private readonly string _version;

        /// <summary>
        ///     Test
        /// </summary>
        public Test(ViewRender renderer, string version)
        {
            _renderer = renderer;
            _version = version;
        }

        /// <summary>
        ///     Convert or in this case create a test
        /// </summary>
        /// <returns></returns>
        public string Convert(QtiTest qtiTest) => _renderer.Render("Test", _version, qtiTest).Trim();
    }
}