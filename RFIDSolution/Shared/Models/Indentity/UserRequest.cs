using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Indentity
{
    public class UserRequest
    {
        public UserRequest()
        {

        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }

        public List<int> RoleIds = new List<int>();
    }
}
