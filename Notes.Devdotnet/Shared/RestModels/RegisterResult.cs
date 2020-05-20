using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Devdotnet.Shared.RestModels
{
    public class RegisterResult
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
