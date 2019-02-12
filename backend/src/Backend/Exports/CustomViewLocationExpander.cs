using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Citolab.QConstruction.Backend.Exports
{
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            return new[]
            {
                "/Exports/Views/2_1/{0}.cshtml",
                "/Exports/Views/2_2/{0}.cshtml",
                "/Exports/Views/QB_BackOffice/{0}.cshtml"
            }; // add `.Union(viewLocations)` to add default locations
        }
    }
}