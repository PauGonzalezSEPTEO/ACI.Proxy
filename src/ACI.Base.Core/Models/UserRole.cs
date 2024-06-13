using Microsoft.AspNetCore.Identity;

namespace ACI.Base.Core.Models
{
    public class UserRole : IdentityUserRole<string>, IAuditable
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
