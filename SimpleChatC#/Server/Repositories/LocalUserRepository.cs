using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;
using Server.Entities;

namespace Server.Repositories
{
    /// <summary>
    /// Should only be used while testing.
    /// </summary>
    public class LocalUserRepository : IUserRepository
    {
        private List<User> users;
        private int nextId;

        public LocalUserRepository()
        {
            users = new List<User>();
            nextId = 1;

            Register(new User { Password="admin1",Username="admin"});
            Console.WriteLine(Login("admin","admin")!=null);
        }

        public List<User> Users
        {
            get
            {
                List<User> copyUsers = new List<User>();

                foreach(User u in users)
                    copyUsers.Add(new User(u));

                return copyUsers;
            }
        }

        public void Delete(User user)
        {
            User foundUser = users.FirstOrDefault(u => u.Id == user.Id);
            users.Remove(foundUser);
        }

        public bool IsUsernameTaken(string username)
        {
            foreach (User u in users)
                if (u.Username == username)
                    return true;
            return false;
        }

        public User Login(string username, string password)
        {
            foreach(User u in users)
                if (u.Username == username && BCryptHelper.CheckPassword(password, u.Password))
                    return new User(u);
            return null;
        }

        public User Register(User user)
        {
            if (user == null || user.Username == null || user.Password == null)
                throw new Exception();

            if (IsUsernameTaken(user.Username))
                throw new UsernameIsTakenException();

            user.Id = nextId++;

            users.Add(new User(user));

            return new User(user);
        }

        public User Update(User user)
        {
            User foundUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (foundUser == null)
                return null;

            users.Remove(foundUser);
            users.Add(new User(user));

            return new User(user);
        }
    }
}