using Citolab.QConstruction.Model;
using Citolab.QConstruction.Backend.Exports.Interfaces;
using Citolab.QConstruction.Backend.Exports.QTI.CustomClasses;

namespace Citolab.QConstruction.Backend.Exports.QTI
{
    /// <summary>
    ///     Converts item to an QTI Item
    /// </summary>
    public class ChoiceInteraction : IConvertItem
    {
        private readonly ViewRender _renderer;
        private readonly string _version;

        /// <summary>
        ///     Choice Interaction
        /// </summary>
        public ChoiceInteraction(ViewRender renderer, string version)
        {
            _renderer = renderer;
            _version = version;
        }

        /// <summary>
        ///     Convert item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string Convert(Item item)
            => _renderer.Render("ChoiceInteraction", _version, new MultipleChoiceItem(item));
    }
}