using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Model;
using Citolab.Repository;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Queries
{
    /// <summary>
    ///     User Query Handler
    /// </summary>
    public class UserQueryHandlers : IRequestHandler<UserQuery, User>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Wishlist> _wishlistRepository;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryFactory"></param>
        public UserQueryHandlers(IRepositoryFactory repositoryFactory)
        {
            _userRepository = repositoryFactory.GetRepository<User>();
            _wishlistRepository = repositoryFactory.GetRepository<Wishlist>();
        }

        /// <summary>
        ///     Handles the query
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<User> Handle(UserQuery message)
        {
            return Task.Run(() =>
            {
                var user = _userRepository.GetAsync(message.Id).Result;
                //.AsQueryable().FirstOrDefault(d => d.Id == message.Id);
                if (user == null) return null;
                if (user.UserType == UserType.RestrictedManager || user.UserType == UserType.Manager || user.UserType == UserType.Admin)
                    user.AllowedWishlists =
                        _wishlistRepository.AsQueryable()
                            .Select(w => new KeyValue { Id = w.Id, Value = w.Title })
                            .ToArray();
                if (user.SelectedWishlist == null && user.AllowedWishlists.Any())
                    user.SelectedWishlist = user.AllowedWishlists.FirstOrDefault();
                return user;
            });
        }

        public Task<User> Handle(UserQuery request, CancellationToken cancellationToken) => Task.Run(() =>
            {
                var user = _userRepository.GetAsync(request.Id).Result;
                if (user == null) return null;
                if (user.UserType == UserType.Admin)
                {
                    user.AllowedWishlists = _wishlistRepository
                        .AsQueryable()
                        .Select(w => new KeyValue { Id = w.Id, Value = w.Title })
                        .ToArray();
                }
                if ((user.SelectedWishlist == null && user.AllowedWishlists != null) || 
                    (user.AllowedWishlists != null && user.SelectedWishlist != null && user.AllowedWishlists.All(a => a.Id != user.SelectedWishlist.Id)))
                    user.SelectedWishlist = user.AllowedWishlists.FirstOrDefault();
                return user;
            });
    }
}