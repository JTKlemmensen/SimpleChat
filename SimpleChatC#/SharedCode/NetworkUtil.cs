using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class NetworkUtil
    {
        private static RSACryptoServiceProvider rsa;

        public static string InsertEscape(string text)
        {
            return text.Replace("/", "//").Replace(",","/,");
        }

        public static List<string> RemoveEscape(string message)
        {
            List<string> tokens = new List<string>();

            char[] chars = message.ToCharArray();

            string token = "";
            for(int i=0;i<chars.Count(); i++)
            {
                if(chars[i] == '/')
                {
                    i++;
                    if(chars[i] == '/')
                        token += '/';
                    else if(chars[i] == ',')
                        token += ',';
                }
                else if(chars[i]==',')
                {
                    tokens.Add(token);
                    token = "";
                }
                else
                    token += chars[i];
            }

            tokens.Add(token);

            return tokens;
        }
    }
}