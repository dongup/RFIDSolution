using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Indentity;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Service
{
    public interface IAuthService
    {
        public Task<ResponseModel<LoginResponseModel>> Login(LoginModel login);
        public Task LogOut();
    }
}