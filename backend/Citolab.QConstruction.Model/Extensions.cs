using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Citolab.QConstruction.Model
{
    public static class Extensions
    {

        /// <summary>
        ///     Get latest version of version property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="versionedProperty"></param>
        /// <returns></returns>
        public static T Latest<T>(this VersionedProperty<T> versionedProperty)
        {
            var latest = versionedProperty?.LastOrDefault();
            return latest != null ? latest.Value : default(T);
        }
    }
}
