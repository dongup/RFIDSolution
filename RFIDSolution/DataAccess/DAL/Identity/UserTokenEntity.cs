using Microsoft.AspNetCore.Identity;

namespace RFIDSolution.Shared.DAL.Entities.Identity
{
    public class UserTokenEntity : IdentityUserToken<int>
    {
        public bool IsDeleted { get; set; }
    }
}
