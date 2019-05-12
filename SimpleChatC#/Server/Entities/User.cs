using System;
using System.Collections.Generic;
using System.Text;
using DevOne.Security.Cryptography.BCrypt;

namespace Server.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        private string password;
        public string Password {
            get => password;
            set
            {
                Salt = BCryptHelper.GenerateSalt();
                password = BCryptHelper.HashPassword(value, Salt);
            }
        }
        public string Salt { get; private set; }
    }
}