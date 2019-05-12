using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        //TODO Set should hash the password and set the salt
        public string Password { get; set; }
        public string Salt { get; private set; }
    }
}