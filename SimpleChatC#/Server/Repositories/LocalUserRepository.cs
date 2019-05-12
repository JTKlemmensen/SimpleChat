using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;
using DevOne.Security.Cryptography.BCrypt;

namespace Server.Repositories
{
    public class LocalUserRepository : IUserRepository
    {
        private List<User> users;
        private static readonly Object user_lock = new Object();
        private int nextId;

        public LocalUserRepository()
        {
            users = new List<User>();
             nextId = 1;
        }

        //TODO should make a hardcopy of all the users and make a new list
        public List<User> Users => users;

        public void DeleteUser(User user)
        {
            lock(user_lock)
                for(int i=0;i<users.Count;i++)
                    if(users[i].Id == user.Id)
                    {
                        users.RemoveAt(i);
                        break;
                    }
        }

        public User Login(string username, string password)
        {
            for (int i = 0; i < users.Count; i++)
                if(users[i].Username==username && BCryptHelper.CheckPassword(password,users[i].Password))
                    return users[i];

            return null;
        }

        public User SaveUser(User user)
        {
            // Check if a user with the username already exists, if so then check if they have the same id
                // Check if user id is already registered
                    // update existing one
                    // else add the new user

            throw new NotImplementedException();
        }
    }
}