using System;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     A screening can have 0-* feedbacks. A feedback is referenced to a screenings item
    /// </summary>
    public class Feedback
    {
        /// <summary>
        ///     Reference to screening item
        /// </summary>
        public Guid ScreeningItemId { get; set; }

        /// <summary>
        ///     Feedback filled in by the screener.
        /// </summary>
        public string Value { get; set; }
    }
}