using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Repositories
{
    public class RepositoryFactory
    {
        private static IUserRepository userRepository;

        public static IUserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new LocalUserRepository();
                return userRepository;
            }
        }
    }
}