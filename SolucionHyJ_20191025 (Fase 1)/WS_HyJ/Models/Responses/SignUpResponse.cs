using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS_HyJ.Models.Responses
{
    public class SignUpResponse
    {
        public SignUpResponse(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        public string UserName { get; private set; }

        public string Email { get; private set; }
    }
}
