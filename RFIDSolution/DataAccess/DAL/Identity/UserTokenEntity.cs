using Microsoft.AspNetCore.Identity;

namespace RFIDSolution.Shared.DAL.Entities.Identity
{
    public class UserTokenEntity : IdentityUserToken<int>
    {
        public bool IS_DELETED { get; set; }
    }
}
