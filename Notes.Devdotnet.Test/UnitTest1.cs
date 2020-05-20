using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Xunit;

namespace Notes.Devdotnet.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //_appSettings.JWT.JwtSecurityKey = str
            var str = "asdasd";

            var strBase64=Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            var key = Base64UrlEncoder.DecodeBytes(strBase64);
            var signingKey = new SymmetricSecurityKey(key);


            //var key = Base64UrlEncoder.DecodeBytes(str);

            //var key = Convert.FromBase64String(str);

            //var signingKey = new SymmetricSecurityKey(key);


            Assert.Equal("1", "1");
        }
    }
}
