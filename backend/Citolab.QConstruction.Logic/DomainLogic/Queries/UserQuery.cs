using System;
using Citolab.QConstruction.Model;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Queries
{
    /// <summary>
    ///     User Query
    /// </summary>
    public class UserQuery : IRequest<User>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        public UserQuery(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// </summary>
        public Guid Id { get; set; }
    }
}