using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Notes.Devdotnet.Shared.RestModels
{
    public class LoginModel
    {
        [Required]
        public string LoginOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
