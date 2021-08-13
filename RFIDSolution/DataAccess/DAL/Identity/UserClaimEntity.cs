using Microsoft.AspNetCore.Identity;

namespace RFIDSolution.WebAdmin.DAL.Entities.Identity
{
    public class UserClaimEntity : IdentityUserClaim<int>
    {
        public bool IsDeleted { get; set; }
    }
}
