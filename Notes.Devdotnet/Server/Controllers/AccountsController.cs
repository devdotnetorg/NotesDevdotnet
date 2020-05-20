using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Notes.Devdotnet.Server.Helper;
using Notes.Devdotnet.Server.Services;
using Notes.Devdotnet.Shared.RestModels;

namespace Notes.Devdotnet.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        public AccountsController(UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] RegisterModel model)
        {
            if(!_appSettings.AnyoneCanRegister)
            {
                //TODO: проверить регистрацию только для администратора
                if (!User.Identity.IsAuthenticated)
                {
                    if(!User.IsInRole("admin"))
                    {
                        IEnumerable<string> errors = new string[] { "Close Register" };
                        return Ok(new RegisterResult { Successful = false, Errors = errors });
                    }
                }
            }
            //
            var newUser = new IdentityUser { UserName = model.Login, Email = model.Email };           
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return Ok(new RegisterResult { Successful = false, Errors = errors });
            }
            return Ok(new RegisterResult { Successful = true });
        }
    }
}
