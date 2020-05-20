using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Devdotnet.Server.Services
{
    /// <summary>
    /// Интерфейс сервиса Аутентификации
    /// </summary>
    public interface IAuthenticateService
    {

        public SymmetricSecurityKey signingKey { get;}
        public JwtSecurityToken GetToken(string login);
        public string GetTokenStr(string login);
    }
}
