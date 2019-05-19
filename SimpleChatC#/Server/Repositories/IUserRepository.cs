using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Repositories
{
    public interface IUserRepository
    {
        List<User> Users { get; }
        User Login(string username, string password);
        User Register(User user);
        User Update(User user);
        bool IsUsernameTaken(string username);
        void Delete(User user);
    }
}