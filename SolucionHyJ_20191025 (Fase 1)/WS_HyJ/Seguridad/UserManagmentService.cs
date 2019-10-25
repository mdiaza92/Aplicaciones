using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Identity;

namespace WS_HyJ.Seguridad
{
    public class UserManagmentService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserManagmentService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public bool IsValidUser(string userName, string password)
        {
            var user = _userManager.FindByNameAsync(userName).Result;

            var validate = _signInManager.UserManager.CheckPasswordAsync(user, password);

            return validate.Result;
        }

        public ApplicationUser GetUserId(string username)
        {
            return _userManager.FindByNameAsync(username).Result;
        }
    }
}
