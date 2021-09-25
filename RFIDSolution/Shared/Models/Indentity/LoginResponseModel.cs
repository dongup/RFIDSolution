using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Indentity
{
    public class LoginResponseModel
    {
        public LoginResponseModel()
        {

        }

        public string Token { get; set; }

        public DateTime ValidTo { get; set; }

        public UserModel User { get; set; }
    }
}
