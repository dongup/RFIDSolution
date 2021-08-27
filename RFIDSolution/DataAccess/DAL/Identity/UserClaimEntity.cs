using Microsoft.AspNetCore.Identity;

namespace RFIDSolution.Shared.DAL.Entities.Identity
{
    public class UserClaimEntity : IdentityUserClaim<int>
    {
        public bool IsDeleted { get; set; }
    }
}
