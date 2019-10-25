using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WS_HyJ.Models.Requests
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }


        [Required]
        public string Password { get; set; }
    }
}
