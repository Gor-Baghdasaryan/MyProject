using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyProject.Models;

namespace MyProject.DataModel
{
    public class LoginModel
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
