using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Notes.Devdotnet.Shared.RestModels;
using Notes.Devdotnet.Server.Services;
using Notes.Devdotnet.Server.Helper;

namespace Notes.Devdotnet.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthenticateService _authenticationJWTService;

        public LoginController(IConfiguration configuration,
                               SignInManager<IdentityUser> signInManager, IAuthenticateService authenticationJWTService)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _authenticationJWTService = authenticationJWTService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        { 
            if(MainHelper.IsValidEmail(login.LoginOrEmail))
            {
                var resultEmail = await _signInManager.UserManager.FindByEmailAsync(login.LoginOrEmail);
                if (resultEmail != null) login.LoginOrEmail = resultEmail.UserName;else
                    return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
            }
            //
            var result = await _signInManager.PasswordSignInAsync(login.LoginOrEmail, login.Password, false, false);
            if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });
            var strToken = _authenticationJWTService.GetTokenStr(login.LoginOrEmail);
            return Ok(new LoginResult { Successful = true, Token = strToken });
        }
    }
}
