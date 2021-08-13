using Microsoft.AspNetCore.Identity;

namespace RFIDSolution.WebAdmin.DAL.Entities.Identity
{
    public class UserTokenEntity : IdentityUserToken<int>
    {
        public bool IsDeleted { get; set; }
    }
}
