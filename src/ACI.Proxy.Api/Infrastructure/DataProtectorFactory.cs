using Microsoft.AspNetCore.DataProtection;

namespace ACI.Proxy.Api.Infrastructure
{
    public class DataProtectorFactory
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public DataProtectorFactory(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public IDataProtector CreateProtector(string purpose)
        {
            return _dataProtectionProvider.CreateProtector(purpose);
        }
    }
}
