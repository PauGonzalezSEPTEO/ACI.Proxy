using ACI.HAM.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ACI.HAM.Core.Data
{
    public class BaseContextFactory : IDbContextFactory<BaseContext>
    {
        private readonly IDbContextFactory<BaseContext> _pooledContextFactory;
        private readonly UserRepository _userRepository;

        public BaseContextFactory(IDbContextFactory<BaseContext> pooledContextFactory, UserRepository userRepository)
        {
            _pooledContextFactory = pooledContextFactory;
            _userRepository = userRepository;
        }

        public BaseContext CreateDbContext()
        {
            var context = _pooledContextFactory.CreateDbContext();
            context.GetUserName = _userRepository.GetUserName;
            return context;
        }
    }
}
