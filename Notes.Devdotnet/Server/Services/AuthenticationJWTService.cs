using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notes.Devdotnet.Server.Helper;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Devdotnet.Server.Services
{
    public class AuthenticationJWTService : IAuthenticateService
    {
        private readonly AppSettings _appSettings;
        public AuthenticationJWTService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            //Get  SymmetricSecurityKey
            //TODO: убрать после теста JWT
            //var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT.JwtSecurityKey));
            //var key = Base64UrlEncoder.DecodeBytes(_appSettings.JWT.JwtSecurityKey);
            //var signingKey = new SymmetricSecurityKey(key);
            var strBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(_appSettings.JWT.JwtSecurityKey));
            var key = Base64UrlEncoder.DecodeBytes(strBase64);
            this._signingKey = new SymmetricSecurityKey(key);
        }

        private readonly SymmetricSecurityKey _signingKey;
        public SymmetricSecurityKey signingKey { get => _signingKey;}

        /// <summary>
        /// Получить токен
        /// </summary>
        /// <returns>
        /// Токен
        /// </returns>
        /// <param name="login">Login пользователя
        /// </param>
        public JwtSecurityToken GetToken(string login)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, login) };
            var creds = new SigningCredentials(this.signingKey, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddHours(_appSettings.JWT.JwtExpiryInHours);
            var claimsIdentity =
               new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                   ClaimsIdentity.DefaultRoleClaimType);
            var token = new JwtSecurityToken(
                issuer: _appSettings.JWT.JwtIssuer,
                audience: _appSettings.JWT.JwtAudience,
                claims: claimsIdentity.Claims,
                expires: expiry,
                signingCredentials: creds
            );
            //
            return token;
        }

        public string GetTokenStr(string login)=> new JwtSecurityTokenHandler().WriteToken(this.GetToken(login));        
    }
}
