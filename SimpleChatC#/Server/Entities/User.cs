using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                if (value.Length < 6 || !ContainsLetter(value) || !ContainsNumber(value))
                    throw new UnsecurePasswordException();

                Salt = BCryptHelper.GenerateSalt();
                password = BCryptHelper.HashPassword(value, Salt);
            }
        }
        public string Salt { get; private set; }

        public User(string hashedPassword, string salt)
        {
            this.password = hashedPassword;
            this.Salt = salt;
        }

        public User(User user)
        {
            this.Id = user.Id;
            this.password = user.password;
            this.Salt = user.Salt;
            this.Username = user.Username;
        }

        public User()
        {

        }

        private bool ContainsLetter(string text)
        {
            return Regex.Matches(text, @"[a-zA-Z]").Count > 0;
        }

        private bool ContainsNumber(string text)
        {
            return text.Any(Char.IsDigit);
        }
    }
}