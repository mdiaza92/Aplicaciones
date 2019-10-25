using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Identity;

namespace WS_HyJ.Seguridad
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
        ApplicationUser GetUserId(string username);
    }
}
