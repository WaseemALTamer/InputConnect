using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.Structures
{
    public class PasswordKey{
        public string Token { get; set; }
        public byte[]? Salt { get; set; }
        public byte[]? Key { get; set; }


        public PasswordKey(string token) { 
            Token = token;
        }


    }
}
