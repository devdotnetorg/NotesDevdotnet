using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Devdotnet.Shared.RestModels
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
    }
}
