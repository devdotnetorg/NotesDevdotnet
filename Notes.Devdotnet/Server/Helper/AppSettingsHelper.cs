using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Devdotnet.Server.Helper
{
    /// <summary>
    /// Настройки приложения.
    /// </summary>
    public class AppSettings
    {
        /// <value>Тип базы данных</value>
        public string UseDB { get; set; }
        /// <value>Строка подключения к базе данных</value>
        public string DBConection { get; set; }
        /// <value>JWT</value>
        public JWTSettings JWT { get; set; }
        /// <value>Любой может регистрироваться</value>
        public bool AnyoneCanRegister { get; set; }
    }

    public class JWTSettings
    {
        /// <value>Секретный ключ Jwt</value>
        public string JwtSecurityKey { get; set; }
        /// <value>JwtIssuer</value>
        public string JwtIssuer { get; set; }
        /// <value>JwtAudience</value>
        public string JwtAudience { get; set; }
        /// <value>Время жизни токена в часах</value>
        public int JwtExpiryInHours { get; set; }
    }

        
}
