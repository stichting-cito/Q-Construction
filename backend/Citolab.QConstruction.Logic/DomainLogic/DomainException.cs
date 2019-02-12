using System;

namespace Citolab.QConstruction.Logic.DomainLogic
{
    public class DomainException : Exception
    {
        public DomainException(string message, bool isMalformed) : base(message)
        {
            IsMalformed = isMalformed;
        }

        /// <summary>
        ///     The cause of the exception is a malformed request. If not,
        ///     the cause is a non-existing resource that's addressed in the originating request.
        /// </summary>
        public bool IsMalformed { get; set; }
    }
}